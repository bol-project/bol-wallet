namespace BolWallet.Services.Abstractions;

public interface INavigationService
{
    public Task NavigateTo<TViewModel>(bool useAnimation = false, bool changeRoot = false) where TViewModel : class;

    public Task NavigateBack();
}
