using BolWallet.Helpers;

namespace BolWallet.Extensions;

public static class RegistrationExtensions
{
	public static IServiceCollection RegisterViewAndViewModelSubsystem(this IServiceCollection services)
	{
		services.AddSingleton<IViewModelToViewBinder>(new ViewModelToViewBinder());
		services.AddSingleton<IViewModelToViewResolver, ViewModelToViewResolver>();

		services
			.BindViewModelToView<MainViewModel, MainPage>()
			.BindViewModelToView<CreateCodenameViewModel, MainPage>()
			.BindViewModelToView<CreateEdiViewModel, MainPage>()
			.BindViewModelToView<GenerateWalletWithPasswordViewModel, MainPage>()
			.BindViewModelToView<MainWithAccountViewModel, MainPage>()
            .BindViewModelToView<RegistrationViewModel, MainPage>()
            .BindViewModelToView<CertifyViewModel, MainPage>()
			.BindViewModelToView<AccountViewModel, MainPage>()
            .BindViewModelToView<BolCommunityViewModel, MainPage>()
			.BindViewModelToView<CertifierViewModel, MainPage>()
			.BindViewModelToView<FinancialTransactionsViewModel, MainPage>()
			.BindViewModelToView<RetrieveBolViewModel, MainPage>()

			.BindViewModelToView<SendBolViewModel, MainPage>()
			.BindViewModelToView<UserViewModel, MainPage>()            
			.BindViewModelToView<TransactionsViewModel, TransactionsPage>()			
			
			.BindViewModelToView<MoveClaimViewModel, MainPage>();

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