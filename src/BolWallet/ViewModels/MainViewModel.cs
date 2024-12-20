﻿using Bol.Core.Abstractions;
using BolWallet.Models.Messages;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;

namespace BolWallet.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly ISecureRepository _secureRepository;
    private readonly IWalletService _walletService;
    private readonly INetworkPreferences _networkPreferences;
    private readonly IBolRpcService _bolRpcService;
    private readonly IMessenger _messenger;

    public MainViewModel(
        INavigationService navigationService,
        ISecureRepository secureRepository,
        IWalletService walletService,
        INetworkPreferences networkPreferences,
        IBolRpcService bolRpcService,
        IMessenger messenger)
        : base(navigationService)
    {
        _secureRepository = secureRepository;
        _walletService = walletService;
        _networkPreferences = networkPreferences;
        _bolRpcService = bolRpcService;
        _messenger = messenger;

        SetTitleMessage();
    }

    private async Task TrySetBolContractHash()
    {
        var result = await _bolRpcService.GetBolContractHash();
        if (result.IsFailed)
        {
            await Toast.Make(result.Message).Show();
            return;
        }

        _networkPreferences.SetBolContractHash(result.Data);
    }

    [ObservableProperty]
    private bool _isLoading = false;
    
    [ObservableProperty]
    private string _loadingText = "Loading...";

    [ObservableProperty]
    private string _title;
    
    [ObservableProperty]
    private string _welcomeMessage = Constants.WelcomeMessage;
    
    [ObservableProperty]
    private string _switchToNetworkText;
    
    [RelayCommand]
    private async Task SwitchNetwork()
    {
        var prompt = await Application.Current.Windows[0].Page.DisplayPromptAsync(
            "Network Change",
            $"You are currently connected to {_networkPreferences.Name}." +
            $"{Environment.NewLine}{Environment.NewLine}" +
            $"To switch to {_networkPreferences.AlternativeName}, type its name below:",
            keyboard: Keyboard.Plain,
            initialValue: "",
            accept: $"Yes, change to {_networkPreferences.AlternativeName}!",
            cancel: "Cancel, I don't know what I'm doing!");
        
        if (string.IsNullOrWhiteSpace(prompt) || !prompt.Equals(_networkPreferences.AlternativeName))
        {
            await Application.Current.Windows[0].Page.DisplayAlert("Network Change",
                $"{_networkPreferences.AlternativeName} network name wasn't verified, no switch will occur.",
                "OK");
            
            return;
        }
        
        IsLoading = true;
        LoadingText = "Changing network...";
        
        _networkPreferences.SwitchNetwork();
        await TrySetBolContractHash();
        SetTitleMessage();
        _ = _messenger.Send(Constants.TargetNetworkChangedMessage);

        LoadingText = string.Empty;
        IsLoading = false;
        
        await Application.Current.Windows[0].Page.DisplayAlert("Network Change",
            $"Switch to {_networkPreferences.Name} network completed successfully.",
            "OK");
    }
    
    [RelayCommand]
	private async Task NavigateToCodenameCompanyPage()
	{
        await NavigationService.NavigateTo<CreateCodenameCompanyViewModel>(true);
    }

    [RelayCommand]
    private async Task NavigateToWalletCreationInfoPage()
    {
        await App.Current.Windows[0].Page.Navigation.PushAsync(new Views.WalletCreationInfo());
    }

    [RelayCommand]
    private async Task ImportYourWallet()
    {
        try
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.json" } },
                { DevicePlatform.Android, new[] { "application/json" } },
                { DevicePlatform.MacCatalyst, new[] { "json" } },
                { DevicePlatform.WinUI, new[] { ".json", "application/json" } }
            });

            var pickResult = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = customFileType, PickerTitle = "Import Your Json Wallet"
            });

            if (pickResult == null)
                return;

            var jsonString = await File.ReadAllTextAsync(pickResult.FullPath);
            
            // var passwordPopup = new PasswordPopup();
            // await Application.Current.MainPage.ShowPopupAsync(passwordPopup);
            // var password = await passwordPopup.TaskCompletionSource.Task;

            string password;
            while(true)
            {
                password = await Application.Current.Windows[0].Page.DisplayPromptAsync(
                    "Unlock Wallet",
                    "Type your password to unlock your wallet.",
                    keyboard: Keyboard.Password,
                    initialValue: "",
                    accept: "OK, unlock my wallet!",
                    cancel: "Cancel");

                if (password is null)
                {
                    return;
                }
                
                password = password.Trim();
                if (password is { Length: > 0 } )
                {
                    break;
                }
            }
            
            IsLoading = true;
            LoadingText = "Unlocking your wallet... Please wait.";
                
            var validPassword = await Task.Run(() => _walletService.CheckWalletPassword(jsonString, password));
            if (!validPassword)
            {
                await Application.Current.Windows[0].Page.DisplayAlert(
                    "Incorrect Password",
                    "Please provide a valid password.",
                    "OK");
                return;
            }
            
            var bolWallet = JsonSerializer.Deserialize<Bol.Core.Model.BolWallet>(jsonString,
                Constants.WalletJsonSerializerDefaultOptions);

            var userData = new UserData { Codename = bolWallet.Name, BolWallet = bolWallet, WalletPassword = password };

            await _secureRepository.SetAsync("userdata", userData);

            await NavigationService.NavigateTo<MainWithAccountViewModel>(changeRoot: true);
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
        finally
        { 
            IsLoading = false;
            LoadingText = String.Empty;
        }
    }
    
    [RelayCommand]
    private void SaveLogfile()
    {
        _ = _messenger.Send(Constants.SaveLogfileMessage);
    }

    private void SetTitleMessage()
    {
        Title = _networkPreferences.IsMainNet ? string.Empty : $"({_networkPreferences.Name})";
    }
}
