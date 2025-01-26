using Events.Core.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
namespace Events.Infrastructure.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly IDistributedCache _imageCache;
        public ImageRepository(IDistributedCache cache)
        {
            _imageCache = cache;
        }
        public async Task<(string, IEnumerable<string>)> SaveEventImageAsync(string webRootPath, string rootDir, string fileName, Stream image)
        {
            var uploadsDir = Path.Combine(webRootPath, rootDir);
            Directory.CreateDirectory(uploadsDir);
            var filePath = Path.Combine(uploadsDir, fileName);
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _imageCache.Remove(fileName);
                }
                using (var diskFile = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await image.CopyToAsync(diskFile);
                }
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };
                await _imageCache.SetAsync(fileName, File.ReadAllBytes(filePath), options);
                return (Path.Combine(rootDir, fileName), Enumerable.Empty<string>());
            }
            catch (Exception ex)
            {
                return (string.Empty, new List<string>() { ex.Message });
            }
        }
        public async Task<(byte[], IEnumerable<string>)> GetEventImageAsync(string webRootPath, string rootDir, string fileName)
        {
            try
            {
                var imageBytes = await _imageCache.GetAsync(fileName);
                if (imageBytes is not null)
                    return (imageBytes, Enumerable.Empty<string>());
                var filePath = Path.Combine(webRootPath, rootDir, fileName);
                if (File.Exists(filePath))
                {
                    byte[] fileBytes;
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        fileBytes = new byte[stream.Length];
                        await stream.ReadExactlyAsync(fileBytes.AsMemory(0,fileBytes.Length));
                    }
                    var options = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    };
                    await _imageCache.SetAsync(fileName, fileBytes, options);
                    return (fileBytes, Enumerable.Empty<string>());
                }
                return (Array.Empty<byte>(), new[] { "File doesn't exist." });
            }
            catch (Exception ex)
            {
                return (Array.Empty<byte>(), new[] { ex.Message });
            }
        }
    }
}
