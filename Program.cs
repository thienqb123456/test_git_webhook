using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using ngaoda;
using ngaoda.Services;
using NLog;
using NLog.Extensions.Logging;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;

var webHostEnvironment = builder.Environment;

// Cấu hình Kestrel để thiết lập kích thước tối đa của request body
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 524288000; // 500 MB
});

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 524288000; // 500 MB
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 524288000;
});

// NLog configuration
LogManager.Configuration = new NLogLoggingConfiguration(configuration.GetSection("NLog"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(loggingBuilder =>
{
    //loggingBuilder.ClearProviders();
    loggingBuilder.AddNLog();
});


builder.Services.AddScoped(typeof(IImageProcessingService), typeof(ImageProcessingService));

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOrigins",
//        builder =>
//        {
//            builder.WithOrigins(
//                    "http://localhost:4000",
//                    "http://127.0.0.1:4000",
//                    "http://localhost:5500",
//                    "http://127.0.0.1:5500",
//                    "https://acp.vpress.vn",
//                    "https://vpress.vn",
//                    "https://portal.vpress.vn",
//                    "https://uat-acp.vpress.vn"
//                    )
//                       .AllowAnyHeader()
//                       .AllowAnyMethod()
//                       .AllowCredentials();

//        });
//});

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("",
//        builder =>
//        {
//            builder.AllowAnyOrigin()
//                .AllowAnyHeader()
//                .AllowAnyMethod();
//        });
//});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("*")  // Cho phép tất cả các nguồn gốc.
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

//// Configure the WebRootPath
//var newWebRootPath = "C:\\resources";
//builder.Environment.WebRootPath = newWebRootPath;
//builder.Environment.WebRootFileProvider = new PhysicalFileProvider(newWebRootPath);

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRewriter(new RewriteOptions()
            .Add(new CustomRewriteRule()));

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    // Cấu hình phần mở rộng tệp tin và loại MIME tương ứng
    ContentTypeProvider = new FileExtensionContentTypeProvider
    {
        // Thêm loại MIME cho tệp INDD
        Mappings = { [".indd"] = "application/x-indesign" }
    },
    //FileProvider = new PhysicalFileProvider(newWebRootPath),
    RequestPath = ""
    //RequestPath = "/resources"  // Đường dẫn URL để truy cập các tệp tĩnh
});
app.UseRouting();

//app.UseCors("AllowAll");

//app.UseCors();  // Thêm middleware CORS


// Trong Configure(IApplicationBuilder app, IWebHostEnvironment env)
//app.UseMiddleware<CustomResponseHeaderMiddleware>();

app.MapControllers();

app.Run();
