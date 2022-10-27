namespace BolWallet.Helpers;

public interface IViewModelToViewResolver
{
	Page Resolve<TViewModel>() where TViewModel : class;
}

public class ViewModelToViewResolver : IViewModelToViewResolver
{
	private readonly IServiceProvider _serviceProvider;
	private readonly IViewModelToViewBinder _binder;
    
	public ViewModelToViewResolver(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
		_binder = _serviceProvider.GetRequiredService<IViewModelToViewBinder>();
	}
    
	public Page Resolve<TViewModel>() where TViewModel : class
	{
		try
		{
			var viewType = _binder.GetBoundViewType(typeof(TViewModel));
			
			var page = (Page) _serviceProvider.GetRequiredService(viewType);

			return page;
		}
		catch (Exception e)
		{
			throw new InvalidOperationException($"Unable to resolve {typeof(TViewModel)}", e);
		}
	}
}