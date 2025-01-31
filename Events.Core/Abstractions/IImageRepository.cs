namespace Events.Core.Abstractions
{
    public interface IImageRepository
    {
        Task<(string, IEnumerable<string>)> SaveEventImageAsync(string webRootPath, string rootDir, string fileName, Stream image);
        Task<(byte[], IEnumerable<string>)> GetEventImageAsync(string webRootPath, string rootDir, string fileName);
        Task<(bool, IEnumerable<string>)> DeleteEventImageAsync(string webRootPath, string rootDir, string fileName);
    }
}
