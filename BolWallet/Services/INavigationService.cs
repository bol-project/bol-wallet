#nullable enable
namespace BolWallet.Services;

public interface INavigationService
{
	public Task NavigateToPage<T>(bool useAnimation = false) where T : Page;

	public Task NavigateBack();
}