using BolWallet.Models.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;

namespace BolWallet.Services;

public class CloseWalletService(
    ISecureRepository secureRepository,
    INavigationService navigationService,
    IMessenger messenger,
    ILogger<CloseWalletService> logger) : ICloseWalletService, IRecipient<WalletCleanupCompletedMessage>
{
    private readonly CancellationTokenSource _cts = new();
    
    public async Task CloseWallet()
    {
        if (!messenger.IsRegistered<WalletCleanupCompletedMessage>(this))
        {
            messenger.Register(this);
        }
        
        logger.LogInformation("Close wallet requested...");
        
        var confirm = await App.Current.MainPage.DisplayAlert(
            "Close Wallet",
            "Closing your wallet will clear all user data, ensure you have saved your wallet and certification files first!",
            "I understand, close my wallet!",
            "Cancel");

        if (!confirm)
        {
            logger.LogInformation("Close wallet request cancelled...");
            return;
        }
        
        logger.LogInformation("Close wallet request confirmed...");
        await secureRepository.RemoveAsync("userdata");
        logger.LogInformation("User Data cleaned up...");

        _ = messenger.Send(Constants.WalletClosedMessage);
        
        // Wait until cleanup is completed before navigating to preload.
        try
        {
            logger.LogInformation("Waiting until wallet services cleanup is complete...");
            await Task.Delay(TimeSpan.MaxValue, _cts.Token);
        }
        catch
        {
            logger.LogInformation("Wallet services cleanup completed...");
        }
        
        await navigationService.NavigateTo<PreloadViewModel>(changeRoot: true);
    }

    public void Receive(WalletCleanupCompletedMessage message)
    {
        _cts.CancelAfter(100);
    }
}
