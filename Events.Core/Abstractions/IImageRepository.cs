namespace Events.Core.Abstractions
{
    public interface IImageRepository
    {
        Task<string> SaveEventImageAsync(string webRootPath, string rootDir, string fileName, Stream image);
        Task<byte[]> GetEventImageAsync(string webRootPath, string rootDir, string fileName);
        Task DeleteEventImageAsync(string webRootPath, string rootDir, string fileName);
    }
}
