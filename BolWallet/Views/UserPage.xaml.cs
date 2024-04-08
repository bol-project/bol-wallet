using System.Timers;

namespace BolWallet.Views;
public partial class UserPage : ContentPage
{
    private UserViewModel _userViewModel { get; set; }
    private System.Timers.Timer timer1 = new System.Timers.Timer(2000);

    public UserPage(UserViewModel userViewModel)
    {
        InitializeComponent();
        _userViewModel = userViewModel;
        BindingContext = userViewModel;
        userViewModel.Address = "1";
        timer1.Elapsed += OnCheckResult;

    }

    private void ValidButton_Click(object sender, EventArgs e)
    {
        Uri uri = new Uri($"http://10.0.2.2:5014/{_userViewModel.Address}", UriKind.RelativeOrAbsolute);

        wView.Source = new UrlWebViewSource { Url = uri.ToString() };
        timer1.Start();
        
    }
    
    public void OnCheckResult(Object sender, ElapsedEventArgs eventArgs)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var result = await wView.EvaluateJavaScriptAsync($"resback()");
            if(!string.IsNullOrEmpty(result) && result=="OK")
            {
                timer1.Stop();
                await DisplayAlert("Validation", "Validated successfully", "OK");
                eEntry.IsVisible = false;
                wView.IsVisible = false;
                vBtn.IsVisible  = false;
            }
        });
    }
}
