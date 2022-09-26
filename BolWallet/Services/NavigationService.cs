#nullable enable
namespace BolApp.Services;

public class NavigationService : INavigationService
{
	private readonly IServiceProvider _serviceProvider;

	public NavigationService(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	private static INavigation Navigation
	{
		get
		{
			var navigation = Application.Current?.MainPage?.Navigation;

			if (navigation is not null)
			{
				return navigation;
			}

			throw new ArgumentNullException();
		}
	}

	public Task NavigateToPage<T>(bool useAnimation = true) where T : Page
	{
		var page = ResolvePage<T>();

		if (page is not null)
		{
			return Navigation.PushAsync(page, useAnimation);
		}

		throw new InvalidOperationException($"Unable to resolve type {typeof(T).FullName}");
	}

	public Task NavigateBack()
	{
		if (Navigation.NavigationStack.Count > 1)
		{
			return Navigation.PopAsync();
		}

		throw new InvalidOperationException("No pages to navigate back to!");
	}

	private T? ResolvePage<T>() where T : Page => _serviceProvider.GetService<T>();
}