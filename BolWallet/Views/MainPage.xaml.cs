using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Extensions.Options;

namespace BolWallet;

public partial class MainPage : ContentPage
{
    private readonly IBolRpcService _bolRpcClient;
    private readonly IOptions<BolConfig> _bolConfigOptions;
    RadialControlViewModel RadialVM = new();

    public MainPage(
        MainViewModel mainViewModel,
        IBolRpcService bolRpcClient,
        IOptions<BolConfig> bolConfigOptions)
    {
		InitializeComponent();
		BindingContext = mainViewModel;
        _bolRpcClient = bolRpcClient;
        _bolConfigOptions = bolConfigOptions;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Dispatcher.DispatchAsync(TrySetBolContractHash);
    }

    private async Task TrySetBolContractHash()
    {
        if (!string.IsNullOrWhiteSpace(_bolConfigOptions.Value.Contract))
        {
            return;
        }

        var result = await _bolRpcClient.GetBolContractHash();
        if (result.IsFailed)
        {
            await Toast.Make(result.Message).Show();
            return;
        }

        _bolConfigOptions.Value.Contract = result.Data;
        return;
    }
}

