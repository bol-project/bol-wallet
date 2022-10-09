namespace BolWallet.Services.Abstractions;

public interface INavigationService
{
    public Task NavigateToPage<T>(bool useAnimation = false) where T : Page;

    public Task NavigateBack();
}