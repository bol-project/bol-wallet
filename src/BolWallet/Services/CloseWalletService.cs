using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;

namespace BolWallet.Services;

public class CloseWalletService(
    ISecureRepository secureRepository,
    INavigationService navigationService,
    IMessenger messenger,
    ILogger<CloseWalletService> logger) : ICloseWalletService
{
    public async Task CloseWallet()
    {
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
        await navigationService.NavigateTo<PreloadViewModel>(changeRoot: true);
    }
}
