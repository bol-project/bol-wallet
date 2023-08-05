using BolWallet.Helpers;
using BolWallet.Views;

namespace BolWallet.Extensions;

public static class RegistrationExtensions
{
	public static IServiceCollection RegisterViewAndViewModelSubsystem(this IServiceCollection services)
	{
		services.AddSingleton<IViewModelToViewBinder>(new ViewModelToViewBinder());
		services.AddSingleton<IViewModelToViewResolver, ViewModelToViewResolver>();
		
		services
			.BindViewModelToView<MainViewModel, MainPage>()
			.BindViewModelToView<CreateCodenameViewModel, CreateCodenamePage>()
			.BindViewModelToView<CreateEdiViewModel, CreateEdiPage>()
			.BindViewModelToView<GenerateWalletWithPasswordViewModel, GenerateWalletWithPasswordPage>()
            .BindViewModelToView<BolCommunityViewModel, BolCommunityPage>()
            .BindViewModelToView<MainWithAccountViewModel, MainWithAccountPage>()
            .BindViewModelToView<RetrieveBolViewModel, RetrieveBolPage>()
            .BindViewModelToView<SendBolViewModel, SendBolPage>()
            .BindViewModelToView<UserViewModel, UserPage>()
            .BindViewModelToView<MoveClaimViewModel, MoveClaimPage>();

		return services;
	}
	
	private static IServiceCollection BindViewModelToView<TViewModel, TView>(this IServiceCollection services)
		where TViewModel : class
		where TView: class
	{
		services.AddTransient<TViewModel>();
		services.AddTransient<TView>();
		
		using var serviceProvider = services.BuildServiceProvider();
		
		var binder = serviceProvider.GetService<IViewModelToViewBinder>();
		
		if (binder is null)
		{
			binder = new ViewModelToViewBinder();
			services.AddSingleton(binder);
		}
		
		binder.Bind<TViewModel, TView>();

		return services;
	}
}