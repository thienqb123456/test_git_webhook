namespace ngaoda.Services
{
    public interface IImageProcessingService
    {
        (byte[] fileContent, string contentType) ProcessImage(string path, int w = 680, int h = 0, int q = 100, int dpr = 1, string fit = "fill");
    }
}
