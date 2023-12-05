using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;
using System.ComponentModel;
using System.Linq;

namespace BolWallet.Views;

public partial class CreateCodenamePage : ContentPage
{
	public DateTime maxDate { get; set; }
	public DateTime minDate { get; set; }

	public CreateCodenamePage(CreateCodenameViewModel createCodenameViewModel)
	{
		InitializeComponent();
		BindingContext = createCodenameViewModel;
		GenderSelection.ItemsSource = (typeof(Gender).GetEnumValues());
		
        maxDate = new DateTime( DateTime.Now.Year-1,12,31);
		minDate = new DateTime(1900, 1, 1);
        dBirthdate.MaximumDate = maxDate;
		dBirthdate.MinimumDate = minDate;
        if (createCodenameViewModel.CodenameForm.Birthdate.Value == null)
            dBirthdate.Date = maxDate;
    }

	private void OnTapCopy(object sender, EventArgs e)
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