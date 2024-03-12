namespace BolWallet.Services.Abstractions;

public interface IFileDownloadService
{
    Task<byte[]> CreateZipFileAsync(IEnumerable<FileItem> files, CancellationToken cancellationToken = default);
    List<FileItem> CollectIndividualFilesForDownload(UserData userdata);
    Task SaveZipFileAsync(byte[] ediZipFiles, CancellationToken cancellationToken);
    Task DownloadDataAsync<T>(T data, string fileName, CancellationToken cancellationToken = default);
    List<FileItem> CollectCompanyFilesForDownload(UserData userdata);
    Task<byte[]> CreatePasswordProtectedZipFileAsync(
        IEnumerable<FileItem> files,
        string password,
        CancellationToken cancellationToken = default);
}
