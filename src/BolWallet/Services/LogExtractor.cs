using System.IO.Compression;
using BolWallet.Models.Messages;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;

namespace BolWallet.Services;

public class LogExtractor : ILogExtractor, IRecipient<SaveLogfileMessage>
{
    private readonly IMessenger _messenger;
    private readonly TimeProvider _timeProvider;
    private readonly IFileSystem _fileSystem;
    private readonly IFileSaver _fileSaver;
    private readonly ILogger _logger;

    public LogExtractor(
        IMessenger messenger,
        TimeProvider timeProvider,
        IFileSystem fileSystem,
        IFileSaver fileSaver,
        ILogger<LogExtractor> logger)
    {
        messenger.Register<SaveLogfileMessage>(this);
        
        _messenger = messenger;
        _timeProvider = timeProvider;
        _fileSystem = fileSystem;
        _fileSaver = fileSaver;
        _logger = logger;
    }

    public async Task ExtractLog(CancellationToken token = default)
    {
        var now = _timeProvider.GetUtcNow();

        var zipFileName = $"BolWalletLogs_{now:yyyyMMddHHmmss}.zip";
        var zipFilePath = Path.Combine(_fileSystem.CacheDirectory, zipFileName);
        DeleteFileIfExists(zipFilePath);

        var tempLog = $"BolWalletLogs_Temp_{now:yyyyMMddHHmmss}.log";
        var tempLogPath = Path.Combine(_fileSystem.CacheDirectory, tempLog);
        DeleteFileIfExists(tempLogPath);

        try
        {
            var logDirectory = Path.Combine(FileSystem.AppDataDirectory,
                "Logs",
                AppInfo.Current.Name,
                AppInfo.Current.VersionString);

            using var stream = new MemoryStream();

            using (var zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                foreach (var file in Directory.GetFiles(logDirectory))
                {
                    try
                    {
                        zip.CreateEntryFromFile(file, Path.GetFileName(file));
                    }
                    catch
                    {
                        File.Copy(file, tempLogPath);
                        zip.CreateEntryFromFile(tempLogPath, Path.GetFileName(file));
                    }
                }
            }

            using var fileStream = File.OpenRead(zipFilePath);
            var result = await _fileSaver.SaveAsync(zipFileName, fileStream, token);

            if (result.IsSuccessful)
            {
                await Toast.Make($"File '{zipFileName}' saved successfully!").Show(token);
                return;
            }

            _messenger.Send(new DisplayErrorMessage("An error occurred while saving the logfile", result.Exception));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting logs...");
            _messenger.Send(new DisplayErrorMessage("An error occurred while saving the logfile.", ex));
        }
        finally
        {
            DeleteFileIfExists(zipFilePath);
            DeleteFileIfExists(tempLogPath);
        }
    }

    public void Receive(SaveLogfileMessage message)
    {
        _logger.LogInformation("Log extraction requested...");

        MainThread.BeginInvokeOnMainThread(() => _ = ExtractLog());
    }

    private static void DeleteFileIfExists(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
