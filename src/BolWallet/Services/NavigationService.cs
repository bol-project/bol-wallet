#nullable enable
using BolWallet.Helpers;

namespace BolWallet.Services;

public class NavigationService : INavigationService
{
    private readonly IViewModelToViewResolver _viewModelToViewResolver;

    public NavigationService(IViewModelToViewResolver viewModelToViewResolver)
    {
        _viewModelToViewResolver = viewModelToViewResolver;
    }

	private static INavigation Navigation
	{
		get
		{
			var navigation = Application.Current?.Windows[0].Page?.Navigation;

			if (navigation is not null)
			{
				return navigation;
			}

			throw new ArgumentNullException();
		}
	}

	public async Task NavigateTo<TViewModel>(bool useAnimation = true, bool changeRoot = false) where TViewModel : class
	{
        try
        {
            var page = _viewModelToViewResolver.Resolve<TViewModel>();

            if (changeRoot)
            {
                Navigation.InsertPageBefore(page, Navigation.NavigationStack[0]);
                await Navigation.PopToRootAsync(useAnimation);
                return;
            }
            
            await Navigation.PushAsync(page, useAnimation);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Unable to navigate to {typeof(TViewModel).FullName}", exception);
        }
    }

	public Task NavigateBack()
	{
		if (Navigation.NavigationStack.Count > 1)
		{
			return Navigation.PopAsync();
		}

		throw new InvalidOperationException("No pages to navigate back to!");
	}
}
