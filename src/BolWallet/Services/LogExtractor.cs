using System.IO.Compression;
using BolWallet.Models.Messages;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.Messaging;

namespace BolWallet.Services;

public class LogExtractor : ILogExtractor, IRecipient<SaveLogfileMessage>
{
    private readonly TimeProvider _timeProvider;
    private readonly IFileSaver _fileSaver;

    public LogExtractor(
        IMessenger messenger,
        TimeProvider timeProvider,
        IFileSaver fileSaver)
    {
        messenger.Register<SaveLogfileMessage>(this);
        
        _timeProvider = timeProvider;
        _fileSaver = fileSaver;
    }

    public async Task ExtractLog(CancellationToken token = default)
    {
        var zipFileName = $"BolWalletLogs_{_timeProvider.GetUtcNow():yyyyMMddHHmmss}.zip";
        var logDirectory = Path.Combine(FileSystem.AppDataDirectory,
            "Logs",
            AppInfo.Current.Name,
            AppInfo.Current.VersionString);

        using var stream = new MemoryStream();
        ZipFile.CreateFromDirectory(logDirectory, stream);
            
        var result = await _fileSaver.SaveAsync(zipFileName, stream, token);

        if (result.IsSuccessful)
        {
            await Toast.Make($"File '{zipFileName}' saved successfully!").Show(token);
        }
    }

    public void Receive(SaveLogfileMessage message)
    {
        MainThread.BeginInvokeOnMainThread(() => _ = ExtractLog());
    }
}
