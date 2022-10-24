namespace BolWallet.Helpers;

public interface IViewModelToViewResolver
{
    Page Resolve<TViewModel>() where TViewModel : class;
}

public class ViewModelToViewResolver : IViewModelToViewResolver
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Type, Type> _viewModelToView;
    
    public ViewModelToViewResolver(IServiceProvider serviceProvider, Dictionary<Type,Type> viewModelToView)
    {
        _serviceProvider = serviceProvider;
        _viewModelToView = viewModelToView;
    }
    
    public Page Resolve<TViewModel>() where TViewModel : class
    {
        if (!_viewModelToView.TryGetValue(typeof(TViewModel),out var pageType))
        {
            throw new KeyNotFoundException($"Unable to resolve {typeof(TViewModel)}");
        }
        
        var page = (Page) _serviceProvider.GetRequiredService(pageType);

        return page;
    }

}