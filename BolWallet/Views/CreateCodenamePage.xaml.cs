using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;
using System.ComponentModel;

namespace BolWallet.Views;

public partial class CreateCodenamePage : ContentPage
{
	public CreateCodenamePage(CreateCodenameViewModel createCodenameViewModel)
	{
		InitializeComponent();
		BindingContext = createCodenameViewModel;
		GenderSelection.ItemsSource = typeof(Gender).GetEnumValues();
	}

	private void OnTapCopy(object sender, HandledEventArgs e)
	{
		Clipboard.Default.SetTextAsync(Codename.Text);

		Toast.Make("Copied to Clipboard").Show();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((CreateCodenameViewModel)BindingContext).Initialize();
    }
}