using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Cryptography;
using CommunityToolkit.Maui.Alerts;
using System.Reflection;

namespace BolWallet.ViewModels;

public partial class CreateCompanyEdiViewModel : BaseViewModel
{
    private readonly IBase16Encoder _base16Encoder;
    private readonly ISecureRepository _secureRepository;
    private readonly ISha256Hasher _sha256Hasher;
    private readonly IEncryptedDigitalIdentityService _encryptedDigitalIdentityService;
    private CertificationMatrixCompany _certificationMatrix;
    private readonly IMediaService _mediaService;
    public CompanyHashFiles ediFiles;
    
    public CreateCompanyEdiViewModel(
        INavigationService navigationService,
        IBase16Encoder base16Encoder,
        ISecureRepository secureRepository,
        ISha256Hasher sha256Hasher,
        IEncryptedDigitalIdentityService encryptedDigitalIdentityService,
        IMediaService mediaService)
        : base(navigationService)
    {
        _base16Encoder = base16Encoder;
        _secureRepository = secureRepository;
        _sha256Hasher = sha256Hasher;
        _encryptedDigitalIdentityService = encryptedDigitalIdentityService;
        _mediaService = mediaService;
        _certificationMatrix = new CertificationMatrixCompany
        {
            Incorporation = new Incorporation(),
            Hashes = new CompanyHashTable()
        };
        _companyHashTableForm = new CompanyHashTableForm();

        ediFiles = new CompanyHashFiles();
    }

    [ObservableProperty] private CompanyHashTableForm _companyHashTableForm;

    [ObservableProperty] private bool _isLoading = false;

    [RelayCommand]
    private async Task PickFileAsync(string propertyName)
    {
        var pickFileResult = await _mediaService.PickFileAsync();

        PropertyInfo propertyNameInfo = GetPropertyInfo(propertyName);

        await PathPerImport(propertyNameInfo, pickFileResult);
    }

    [RelayCommand]
    private async Task TakePhotoAsync(string propertyName)
    {
        var takePhotoResult = await _mediaService.TakePhotoAsync(FileSystem.CacheDirectory);

        PropertyInfo propertyNameInfo = GetPropertyInfo(propertyName);

        await PathPerImport(propertyNameInfo, takePhotoResult);
    }

    [RelayCommand]
    private async Task Submit()
    {
        try
        {
            IsLoading = true;

            userData = await this._secureRepository.GetAsync<UserData>("userdata");

            _certificationMatrix.CodeName = userData.Codename;
            _certificationMatrix.Incorporation.Title = userData.Company.Title;
            _certificationMatrix.Incorporation.VatNumber = userData.Company.VatNumber;
            _certificationMatrix.Incorporation.IncorporationDate = userData.Company.IncorporationDate;

            var idmc = await Task.Run(() => _encryptedDigitalIdentityService.GenerateMatrix(_certificationMatrix));

            var edi = await Task.Run(() => _encryptedDigitalIdentityService.GenerateCompanyEDI(idmc));

            _certificationMatrix.IncorporationHash = idmc.IncorporationHash;

            userData.Edi = edi;
            userData.CertificationMatrixCompany = _encryptedDigitalIdentityService.SerializeMatrix(_certificationMatrix);
            userData.IdentificationMatrixCompany = _encryptedDigitalIdentityService.SerializeMatrix(idmc);
            userData.IncorporationMatrix = _encryptedDigitalIdentityService.SerializeIncorporation(_certificationMatrix);

            await _secureRepository.SetAsync("userdata", userData);

            await NavigationService.NavigateTo<GenerateWalletWithPasswordViewModel>(true);
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

    private async Task PathPerImport(PropertyInfo propertyNameInfo, FileResult fileResult)
    {
        if (fileResult == null) return;

        var fileBytes = File.ReadAllBytes(fileResult.FullPath);

        var fileName = propertyNameInfo.Name + Path.GetExtension(fileResult.FullPath);

        var ediFileItem = new FileItem { Content = fileBytes, FileName = fileName };

        SetFileHash(propertyNameInfo, fileName, fileBytes);

        ediFiles
            .GetType()
            .GetProperty(propertyNameInfo.Name)
            .SetValue(ediFiles, ediFileItem);

        OnPropertyChanged(nameof(CompanyHashTableForm));

        userData.CompanyHashFiles = ediFiles;

        await _secureRepository.SetAsync("userdata", userData);
    }

    private void SetFileHash(PropertyInfo propertyNameInfo, string fileName, byte[] fileBytes)
    {
        propertyNameInfo.SetValue(CompanyHashTableForm, fileName);

        _certificationMatrix.Hashes
            .GetType()
            .GetProperty(propertyNameInfo.Name)
            .SetValue(_certificationMatrix.Hashes, _base16Encoder.Encode(_sha256Hasher.Hash(fileBytes)));
    }

    public async Task Initialize()
    {
        userData = await this._secureRepository.GetAsync<UserData>("userdata");

        if (userData?.CompanyHashFiles is null) return;

        ediFiles = userData.CompanyHashFiles;

        foreach (var propertyInfo in ediFiles.GetType().GetProperties())
        {
            PropertyInfo propertyNameInfo = GetPropertyInfo(propertyInfo.Name);

            var ediFileItem = (FileItem)propertyInfo.GetValue(ediFiles, null);

            if (ediFileItem?.Content == null)
            {
                continue;
            }

            propertyNameInfo.SetValue(CompanyHashTableForm, ediFileItem.FileName);

            _certificationMatrix.Hashes
                .GetType()
                .GetProperty(propertyNameInfo.Name)
                .SetValue(_certificationMatrix.Hashes, _base16Encoder.Encode(_sha256Hasher.Hash(ediFileItem.Content)));

            ediFiles
                .GetType()
                .GetProperty(propertyNameInfo.Name)
                .SetValue(ediFiles, ediFileItem);
        }

        OnPropertyChanged(nameof(GenericHashTableForm));
    }

    private PropertyInfo GetPropertyInfo(string propertyName)
    {
        return this.CompanyHashTableForm.GetType().GetProperty(propertyName);
    }
}
