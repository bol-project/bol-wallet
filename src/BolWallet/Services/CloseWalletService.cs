using Microsoft.Extensions.Logging;

namespace BolWallet.Services;

public class CloseWalletService(
    ISecureRepository secureRepository,
    INavigationService navigationService,
    ILogger<CloseWalletService> logger) : ICloseWalletService
{
    public async Task CloseWallet()
    {
        logger.LogInformation("Close wallet requested...");
        
        var confirm = await App.Current.MainPage.DisplayAlert(
            "Close Wallet",
            "Closing your wallet will clear any saved wallet and/or account certification data!",
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
        
        await navigationService.NavigateTo<PreloadViewModel>(changeRoot: true);
    }
}
