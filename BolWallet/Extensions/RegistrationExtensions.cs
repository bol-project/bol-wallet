using BolWallet.Helpers;

namespace BolWallet.Extensions;

public static class RegistrationExtensions
{
	public static IServiceCollection RegisterViewAndViewModelSubsystem(this IServiceCollection services)
	{
		services.AddSingleton<IViewModelToViewBinder>(new ViewModelToViewBinder());
		services.AddSingleton<IViewModelToViewResolver, ViewModelToViewResolver>();
		
		services
			.BindViewToViewModel<MainPage, MainViewModel>()
			.BindViewToViewModel<CreateCodenamePage, CreateCodenameViewModel>();
		
		return services;
	}
	
	private static IServiceCollection BindViewToViewModel<TView, TViewModel>(this IServiceCollection services)
		where TView: class
		where TViewModel : class
	{
		services.AddTransient<TView>();
		services.AddTransient<TViewModel>();
		
		var binder = services.BuildServiceProvider().GetService<IViewModelToViewBinder>();
		
		if (binder is null)
		{
			binder = new ViewModelToViewBinder();
			services.AddSingleton(binder);
		}
		
		binder.Bind<TViewModel, TView>();

		return services;
	}
}