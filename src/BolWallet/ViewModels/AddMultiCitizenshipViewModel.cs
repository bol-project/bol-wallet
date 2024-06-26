﻿using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Bol.Core.Abstractions;
using Bol.Core.Rpc.Model;
using CommunityToolkit.Maui.Alerts;

namespace BolWallet.ViewModels;

public class MultiCitizenshipModel
{
    [Required]
    public string CountryCode { get; set; }

    [Required]
    [RegularExpression("^[A-Z]*$", ErrorMessage = "Only capital letters are allowed.")]
    public string FirstName { get; set; }
        
    [Required]
    public string Nin { get; set; }

    [Required] 
    public DateTime? BirthDate { get; set; }
}

public partial class AddMultiCitizenshipViewModel : BaseViewModel
{
    private readonly ICodeNameService _codeNameService;
    private readonly IBolService _bolService;
    private readonly RegisterContent _registerContent;
    
    public AddMultiCitizenshipViewModel(INavigationService navigationService, ICodeNameService codeNameService, IBolService bolService, RegisterContent registerContent) : base(navigationService)
    {
        _codeNameService = codeNameService;
        _bolService = bolService;
        _registerContent = registerContent;
    }

    public MultiCitizenshipModel MultiCitizenshipModel { get; } = new MultiCitizenshipModel();
    
    [ObservableProperty] private string _shortHash;
    [ObservableProperty] private bool _isMultiCitizenshipRegistered;
    [ObservableProperty] private bool _isLoading;

    public async Task Generate()
    {
        ShortHash = _codeNameService.GenerateShortHash(MultiCitizenshipModel.FirstName, MultiCitizenshipModel.BirthDate.Value, MultiCitizenshipModel.Nin);
        try
        {
            IsMultiCitizenshipRegistered = await _bolService.IsMultiCitizenship(MultiCitizenshipModel.CountryCode, ShortHash);
        }
        catch (RpcException)
        {
            IsMultiCitizenshipRegistered = false;
        }
    }

    public async Task Register()
    {
        try
        {
            IsLoading = true;
            await _bolService.AddMultiCitizenship(MultiCitizenshipModel.CountryCode, ShortHash);

            for (int i = 0; i < 20; i++)
            {
                try
                {
                    IsMultiCitizenshipRegistered =
                        await _bolService.IsMultiCitizenship(MultiCitizenshipModel.CountryCode, ShortHash);
                }
                catch (RpcException)
                {
                    IsMultiCitizenshipRegistered = false;
                }

                if (IsMultiCitizenshipRegistered) break;
                await Task.Delay(1000);
            }
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    public void ValidateNin()
    {
        var countryCode = MultiCitizenshipModel.CountryCode;
        var nin = MultiCitizenshipModel.Nin;

        if (string.IsNullOrWhiteSpace(countryCode) || string.IsNullOrWhiteSpace(nin)) return;
        
        const string Pattern = @"^[A-Z0-9]*$";
        var regex = new Regex(Pattern);

        var ninRequiredDigits = _registerContent.NinPerCountryCode[countryCode].Digits;
        
        bool isNinValid = regex.IsMatch(nin);
        bool isNinLengthCorrect = ninRequiredDigits == nin.Length;

        if (isNinValid && isNinLengthCorrect)
        {
            NinValidationErrorMessage = "";
            return;
        }

        NinValidationErrorMessage = 
            $"The National Identification Number (NIN) provided does not match the expected length of {ninRequiredDigits} digits for the country code {countryCode}." +
            " Please ensure that only capital letters (A-Z) and numbers are used in the NIN.";
    }

    public string NinValidationErrorMessage { get; set; }
    public string NinInternationalName => string.IsNullOrWhiteSpace(MultiCitizenshipModel.CountryCode) 
        ? ""
        : _registerContent.NinPerCountryCode[MultiCitizenshipModel.CountryCode]?.InternationalName;
}
