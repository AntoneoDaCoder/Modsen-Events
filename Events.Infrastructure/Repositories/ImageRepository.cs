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
        public async Task<string> SaveEventImageAsync(string webRootPath, string rootDir, string fileName, Stream image)
        {
            var uploadsDir = Path.Combine(webRootPath, rootDir);
            Directory.CreateDirectory(uploadsDir);
            var filePath = Path.Combine(uploadsDir, fileName);
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
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };
            await _imageCache.SetAsync(fileName, File.ReadAllBytes(filePath), options);
            return Path.Combine(rootDir, fileName);
        }
        public async Task<byte[]> GetEventImageAsync(string webRootPath, string rootDir, string fileName)
        {
            var imageBytes = await _imageCache.GetAsync(fileName);
            if (imageBytes is not null)
                return imageBytes;
            var filePath = Path.Combine(webRootPath, rootDir, fileName);
            byte[] fileBytes;
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fileBytes = new byte[stream.Length];
                await stream.ReadExactlyAsync(fileBytes.AsMemory(0, fileBytes.Length));
            }
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };
            await _imageCache.SetAsync(fileName, fileBytes, options);
            return fileBytes;
        }
        public async Task DeleteEventImageAsync(string webRootPath, string rootDir, string fileName)
        {
            var filePath = Path.Combine(webRootPath, rootDir, fileName);
            if (File.Exists(filePath))
                File.Delete(filePath);
            var cachedImage = await _imageCache.GetAsync(fileName);
            if (cachedImage?.Length > 0)
                await _imageCache.RemoveAsync(fileName);
        }
    }
}
