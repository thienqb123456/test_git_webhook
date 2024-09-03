using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;

namespace ngaoda.Services
{
    public class ImageProcessingService(ILogger<ImageProcessingService> logger, IWebHostEnvironment webHostEnvironment) : IImageProcessingService
    {
        public (byte[] fileContent, string contentType) ProcessImage(string path, int w = 680, int h = 0, int q = 100, int dpr = 1, string fit = "fill")
        {
            try
            {
                var resourcePath = webHostEnvironment.WebRootPath;

                // Đường dẫn đến tệp hình ảnh gốc
                string imagePath = Path.Combine(resourcePath, path).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

                // Kiểm tra xem tệp hình ảnh có tồn tại không
                if (!File.Exists(imagePath)) throw new Exception("File is not Exist");

                // Xác định định dạng tệp hình ảnh dựa trên phần mở rộng
                IImageFormat format = Image.DetectFormat(imagePath) ?? throw new Exception("Unsupported file format.");

                // Load hình ảnh gốc
                using (Image image = Image.Load(imagePath))
                {
                    // Chỉnh sửa hình ảnh theo tham số fit
                    switch (fit.ToLowerInvariant())
                    {
                        case "fill":
                            image.Mutate(x => x.Resize(new ResizeOptions
                            {
                                Mode = ResizeMode.Crop,
                                Size = new Size(w * dpr, h * dpr)
                            }));
                            break;
                        case "contain":
                            image.Mutate(x => x.Resize(new ResizeOptions
                            {
                                Mode = ResizeMode.Pad,
                                Size = new Size(w * dpr, h * dpr)
                            }));

                            break;
                        default:
                            throw new Exception("Invalid fit parameter.");
                    }

                    // Lưu hình ảnh đã chỉnh sửa vào MemoryStream
                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        // Lưu hình ảnh dưới định dạng đã xác định
                        image.Save(outputStream, format);

                        // Đặt vị trí của MemoryStream về đầu
                        outputStream.Seek(0, SeekOrigin.Begin);

                        // Trả về hình ảnh đã xử lý dưới dạng FileResult
                        return (outputStream.ToArray(), format.DefaultMimeType);
                    }
                }

            }
            catch (Exception exception)
            {
                logger.LogError(exception, "ImageProcessingService.ProcessImage");
                throw;
            }

        }

    }
}

