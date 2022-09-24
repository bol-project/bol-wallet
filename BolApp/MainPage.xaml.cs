using BolApp.Services;
using BolApp.ViewModels;

namespace BolApp;

public partial class MainPage : ContentPage
{
	public MainPage(MainPageViewModel mainPageViewModel)
	{
		InitializeComponent();
		BindingContext = mainPageViewModel;
	}
}

