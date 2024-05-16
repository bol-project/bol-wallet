using System.Text.RegularExpressions;
using Bol.Core.Model;
using BolWallet.Bolnformation;
using BolWallet.Components;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BolWallet.ViewModels;

public partial class SubmitCitizenshipViewModel : BaseViewModel
{
    private readonly IMediaService _mediaService;
    private readonly RegisterContent _registerContent;
    private readonly ICitizenshipHashTableProcessor _hashTableProcessor;
    private readonly INavigationService _navigationService;
    private readonly ISecureRepository _secureRepository;
    private readonly IDialogService _dialogService;
    private readonly ILogger<SubmitCitizenshipViewModel> _logger;

    private UserData UserData { get; set; }

    public SubmitCitizenshipViewModel(
        IMediaService mediaService,
        RegisterContent registerContent,
        ICitizenshipHashTableProcessor hashTableProcessor,
        INavigationService navigationService,
        ISecureRepository secureRepository,
        IDialogService dialogService,
        ILogger<SubmitCitizenshipViewModel> logger) : base(navigationService)
    {
        _mediaService = mediaService;
        _registerContent = registerContent;
        _hashTableProcessor = hashTableProcessor;
        _navigationService = navigationService;
        _secureRepository = secureRepository;
        _dialogService = dialogService;
        _logger = logger;
    }

    [ObservableProperty] private bool _isFormInitialized;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private List<string> _outstandingCitizenships;
    [ObservableProperty] private EncryptedCitizenshipForm _citizenshipForm;
    [ObservableProperty] CitizenshipHashTableFiles _files;

    [ObservableProperty] private string _ninInternationalName;
    [ObservableProperty] private string _ninValidationErrorMessage = "";

    public void OpenMoreInfo()
    {
        var parameters = new DialogParameters<MoreInfoDialog>
        {
            { x => x.Title, SubmitCitizenshipInformation.Title },
            { x => x.Paragraph1, SubmitCitizenshipInformation.Description },
            { x => x.Paragraph2, SubmitCitizenshipInformation.Content }
        };

        var options = new DialogOptions { CloseOnEscapeKey = true };
        
        _dialogService.Show<MoreInfoDialog>("", parameters, options);
    }
    
    public bool HasAddedMandatoryFiles => Files is 
        { Passport: not null } 
        or { IdentityCard: not null, IdentityCardBack: not null }
        or { ProofOfNin: not null }
        or { BirthCertificate: not null };
    
    public IEnumerable<string> SelectedCountryNames => UserData.Citizenships.Select(c => c.Name).ToArray();

    public async Task OnInitializeAsync(CancellationToken cancellationToken = default)
    {
        IsLoading = true;
        IsFormInitialized = false;
        
        try
        {
            UserData = await _secureRepository.GetAsync<UserData>("userdata");

            var outstandingCitizenships = GetOutstandingCitizenships();
            
            OutstandingCitizenships = outstandingCitizenships;
            
            if (OutstandingCitizenships.Count == 0)
            {
                await _navigationService.NavigateTo<CreateCodenameIndividualViewModel>();
            }
            else
            {
                CitizenshipForm = new EncryptedCitizenshipForm();
                Files = new CitizenshipHashTableFiles();
                CitizenshipForm.CountryName = OutstandingCitizenships.First();
                CitizenshipForm.CountryCode = UserData
                    .Citizenships
                    .First(c => c.Name == CitizenshipForm.CountryName).Alpha3;
                IsFormInitialized = true;
            }
            IsLoading = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing SubmitCitizenshipViewModel");
            await Toast.Make(ex.Message).Show(cancellationToken);
        }
    }
    
    private List<string> GetOutstandingCitizenships()
    {
        var selectedCountries = UserData.Citizenships;
        
        var submittedCountries = UserData
            .EncryptedCitizenshipForms
            .Where(form => form.IsSubmitted)
            .Select(f => f.CountryName)
            .ToArray();
        
        // Those forms no longer belong to the selected countries. They will be removed.
        var formsToClean = submittedCountries.Except(selectedCountries.Select(c => c.Name)).ToList();
        UserData.EncryptedCitizenshipForms.RemoveAll(form => formsToClean.Contains(form.CountryName));
        
        var outstandingCountries = selectedCountries
            .Select(c => c.Name)
            .Except(submittedCountries)
            .ToList();

        return outstandingCountries;
    }

    [RelayCommand]
    private async Task PickFile(string fileType)
    {
        var fileResult = await GetFileAsync(_mediaService.PickFileAsync);
        SetFile(fileType, fileResult);
    }

    [RelayCommand]
    private async Task TakePhoto(string fileType)
    {
        var fileResult = await GetFileAsync(() => _mediaService.TakePhotoAsync(FileSystem.CacheDirectory));
        SetFile(fileType, fileResult);
    }
    
    [RelayCommand]
    private async Task PickPhoto(string fileType, CancellationToken cancellationToken = default)
    {
        var fileResult = await GetFileAsync(_mediaService.PickPhotoAsync);
        SetFile(fileType, fileResult);
    }
    
    [RelayCommand]
    private void SetNinInternationalName()
    {
        NinInternationalName = _registerContent.NinPerCountryCode[CitizenshipForm.CountryCode].InternationalName;
    }
    
    [RelayCommand]
    private void RemoveFile(string fileTypeStr)
    {
        CitizenshipFileType fileType = ConvertStringToEnum(fileTypeStr);

        switch (fileType)
        {
            case CitizenshipFileType.IdentityCard:
                Files.IdentityCard = null;
                break;
            case CitizenshipFileType.IdentityCardBack:
                Files.IdentityCardBack = null;
                break;
            case CitizenshipFileType.Passport:
                Files.Passport = null;
                break;
            case CitizenshipFileType.ProofOfNin:
                Files.ProofOfNin = null;
                break;
            case CitizenshipFileType.BirthCertificate:
                Files.BirthCertificate = null;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(fileType), fileType, "The file type is not supported.");
        }

        Toast.Make("The file was successfully removed.").Show();
    }

    [RelayCommand]
    private Task ValidateNin()
    {
        var nin = CitizenshipForm.Nin;
        var countryCode = CitizenshipForm.CountryCode;
        
        const string Pattern = @"^[A-Z0-9]*$";
        var regex = new Regex(Pattern);

        var ninRequiredDigits = _registerContent.NinPerCountryCode[countryCode].Digits;
        
        bool isNinValid = regex.IsMatch(nin);
        bool isNinLengthCorrect = ninRequiredDigits == nin.Length;

        if (isNinValid && isNinLengthCorrect)
        {
            NinValidationErrorMessage = "";
            return Task.CompletedTask;
        }

        NinValidationErrorMessage = 
                $"The National Identification Number (NIN) provided does not match the expected length of {ninRequiredDigits} digits for the country code {countryCode}." +
                " Please ensure that only capital letters (A-Z) and numbers are used in the NIN.";
        
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task SubmitForm(EncryptedCitizenshipForm form,  CancellationToken cancellationToken = default)
    {
        try
        {
            IsLoading = true;
            
            await Task.Run((Func<Task>)(async () =>
            {
                var countryCode = form.CountryCode;

                await _hashTableProcessor
                    .ProcessFile(Files.IdentityCard, nameof(CitizenshipHashTable.IdentityCard), countryCode,
                        cancellationToken);
                await _hashTableProcessor
                    .ProcessFile(Files.IdentityCardBack, nameof(CitizenshipHashTable.IdentityCardBack), countryCode,
                        cancellationToken);
                await _hashTableProcessor
                    .ProcessFile(Files.Passport, nameof(CitizenshipHashTable.Passport), countryCode, cancellationToken);
                await _hashTableProcessor
                    .ProcessFile(Files.ProofOfNin, nameof(CitizenshipHashTable.ProofOfNin), countryCode,
                        cancellationToken);
                await _hashTableProcessor
                    .ProcessFile(Files.BirthCertificate, nameof(CitizenshipHashTable.BirthCertificate), countryCode,
                        cancellationToken);

                var encryptedCitizenshipData = new EncryptedCitizenshipData
                {
                    CountryCode = countryCode,
                    CountryName = form.CountryName,
                    Nin = form.Nin,
                    SurName = form.SurName,
                    FirstName = form.FirstName,
                    SecondName = form.SecondName,
                    ThirdName = form.ThirdName,
                    CitizenshipHashes = _hashTableProcessor.GetCitizenshipHashes(),
                    CitizenshipActualBytes = _hashTableProcessor.GetCitizenshipActualBytes(),
                    CitizenshipHashTableFileNames = _hashTableProcessor.GetCitizenshipHashTableFileNames(),
                    IsSubmitted = true
                };

                var existingFormIndex = UserData
                    .EncryptedCitizenshipForms
                    .FindIndex(f => f.CountryCode == form.CountryCode);

                // If the form already exists, replace it.
                // Also, insert the updated form at the same index otherwise the EDI will change.
                if (existingFormIndex != -1)
                {
                    UserData.EncryptedCitizenshipForms.RemoveAt(existingFormIndex);
                    UserData.EncryptedCitizenshipForms.Insert(existingFormIndex, encryptedCitizenshipData);
                }
                else
                {
                    UserData.EncryptedCitizenshipForms.Add(encryptedCitizenshipData);
                }

                await _secureRepository.SetAsync("userdata", UserData);
                OutstandingCitizenships.Remove(form.CountryName);
            }), cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error submitting citizenship form.");
            throw;
        }
        finally
        {
            IsLoading = false;
            await _navigationService.NavigateTo<SubmitCitizenshipViewModel>();
        }
    }

    private void SetFile(string fileTypeStr, FileResult file)
    {
        CitizenshipFileType fileType = ConvertStringToEnum(fileTypeStr);
        
        switch (fileType)
        {
            case CitizenshipFileType.IdentityCard:
                Files.IdentityCard = file;
                break;
            case CitizenshipFileType.IdentityCardBack:
                Files.IdentityCardBack = file;
                break;
            case CitizenshipFileType.Passport:
                Files.Passport = file;
                break;
            case CitizenshipFileType.ProofOfNin:
                Files.ProofOfNin = file;
                break;
            case CitizenshipFileType.BirthCertificate:
                Files.BirthCertificate = file;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(fileType), fileType, "The file type is not supported.");
        }
    }

    private async Task<FileResult> GetFileAsync(Func<Task<FileResult>> fileAction)
    {
        try
        {
            IsLoading = true;

            var fileResult = await fileAction();
            return fileResult;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error handling file");
            await Toast.Make(e.Message, ToastDuration.Long).Show();
            throw;
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private static CitizenshipFileType ConvertStringToEnum(string fileTypeStr)
    {
        if (!Enum.TryParse(fileTypeStr, out CitizenshipFileType fileType))
        {
            throw new ArgumentException($"Invalid file type: {fileTypeStr}");
        }

        return fileType;
    }
}
