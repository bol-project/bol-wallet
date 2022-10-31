namespace BolWallet.Helpers;

public interface IViewModelToViewBinder
{
	void Bind<TViewModel, TView>()
		where TViewModel : class
		where TView : class;

	Type GetBoundViewType(Type viewModel);
}

public class ViewModelToViewBinder : IViewModelToViewBinder
{
	private readonly Dictionary<Type, Type> _viewModelToView;

	public ViewModelToViewBinder()
	{
		_viewModelToView = new Dictionary<Type, Type>();
	}

	public void Bind<TViewModel, TView>()
		where TViewModel : class
		where TView : class
	{
		_viewModelToView.Add(typeof(TViewModel), typeof(TView));
	}

	public Type GetBoundViewType(Type viewModel)
	{
		if (!_viewModelToView.TryGetValue(viewModel,out var boundView))
		{
			throw new KeyNotFoundException($"Unable to get bound view for {viewModel}");
		}
		return boundView;
	}
}