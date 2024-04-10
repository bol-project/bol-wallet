using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Cryptography;
using CommunityToolkit.Maui.Alerts;
using System.Reflection;
using Plugin.Maui.Audio;

namespace BolWallet.ViewModels;

public partial class CreateEdiViewModel : BaseViewModel
{
    private readonly IPermissionService _permissionService;
    private readonly IBase16Encoder _base16Encoder;
    private readonly ISha256Hasher _sha256Hasher;
    private readonly ISecureRepository _secureRepository;
    private readonly IEncryptedDigitalIdentityService _ediService;
    private readonly IMediaService _mediaService;
    private readonly CertificationMatrix _certificationMatrix;
    private readonly IAudioRecorder _recorder;
    
    public CreateEdiViewModel(
        INavigationService navigationService,
        IPermissionService permissionService,
        IBase16Encoder base16Encoder,
        ISha256Hasher sha256Hasher,
        ISecureRepository secureRepository,
        IEncryptedDigitalIdentityService ediService,
        IAudioManager audioManager,
        IMediaService mediaService)
        : base(navigationService)
    {
        _permissionService = permissionService;
        _base16Encoder = base16Encoder;
        _sha256Hasher = sha256Hasher;
        _secureRepository = secureRepository;
        _ediService = ediService;
        _mediaService = mediaService;
        _certificationMatrix = new CertificationMatrix { Hashes = new GenericHashTable() };
        GenericHashTableForm = new GenericHashTableForm();
        _ediFiles = new GenericHashTableFiles();
        _recorder = audioManager.CreateRecorder();
    }
    
    [ObservableProperty] private GenericHashTableFiles _ediFiles;
    
    [ObservableProperty] private GenericHashTableForm _genericHashTableForm;

    [ObservableProperty] private bool _isLoading = false;

    [ObservableProperty] private bool _isRecording = false;

    [RelayCommand]
    private async Task PickFileAsync(string propertyName)
    {
        var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.iOS, new[] { "com.adobe.pdf", "public.image" } },
            { DevicePlatform.Android, new[] { "application/pdf", "image/*"} },
            { DevicePlatform.MacCatalyst, new[] { "pdf", "public.image" } },
            { DevicePlatform.WinUI, new[] { ".pdf", ".gif", ".png", ".jpg", ".jpeg" } },
        });

        var pickResult = await FilePicker.PickAsync(new PickOptions
        {
            FileTypes = customFileType, PickerTitle = "Pick a file"
        });

        PropertyInfo propertyNameInfo = GetPropertyInfo(propertyName);

        await PathPerImport(propertyNameInfo, pickResult);
    }

    [RelayCommand]
    private async Task PickAudioAsync(string propertyName)
    {
        var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.iOS, new[] {"public.audio" } },
            { DevicePlatform.Android, new[] {  "audio/*" } },
            { DevicePlatform.MacCatalyst, new[] {"public.audio" } },
            { DevicePlatform.WinUI, new[] { ".mp3" } },
        });

        var pickResult = await FilePicker.PickAsync(new PickOptions
        {
            FileTypes = customFileType,
            PickerTitle = "Pick a file"
        });

        PropertyInfo propertyNameInfo = GetPropertyInfo(propertyName);

        await PathPerImport(propertyNameInfo, pickResult);
    }
    
    [RelayCommand]
    private async Task TakePhotoAsync(string propertyName)
    {
        var photoFilePath = await _mediaService.TakePhotoAsync(FileSystem.CacheDirectory);
            
        var fileResult = new FileResult(photoFilePath);
        
        PropertyInfo propertyNameInfo = GetPropertyInfo(propertyName);

        await PathPerImport(propertyNameInfo, fileResult);
    }

    [RelayCommand]
    private async Task PickPhotoAsync(string propertyName)
    {
        var photoResult = await _mediaService.PickPhotoAsync();

        PropertyInfo propertyNameInfo = GetPropertyInfo(propertyName);

        await PathPerImport(propertyNameInfo, photoResult);
    }

    [RelayCommand]
    private async Task StartRecording()
    {
        var hasGivenPermission = await _permissionService.TryGetPermissionAsync<Permissions.Microphone>();

        if (!hasGivenPermission) return;

        if (!_recorder.IsRecording)
        {
            IsRecording = true;
            await _recorder.StartAsync();
        }
    }

    [RelayCommand]
    private async Task StopRecording()
    {
        if (!_recorder.IsRecording) return;
        
        var recording = await _recorder.StopAsync();
        IsRecording = false;
        
        var audioStream = recording.GetAudioStream();

        string directoryPath = FileSystem.AppDataDirectory; // or FileSystem.CacheDirectory for temporary files
        string filePath = Path.Combine(directoryPath, "personalVoice.audio");

        await using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            await audioStream.CopyToAsync(fileStream);
        }

        PropertyInfo propertyNameInfo = GetPropertyInfo(nameof(GenericHashTableForm.PersonalVoice));
        await PathPerImport(propertyNameInfo, new FileResult(filePath));
    }

    [RelayCommand]
    private async Task Submit()
    {
        try
        {
            IsLoading = true;

            userData = await this._secureRepository.GetAsync<UserData>("userdata");

            _certificationMatrix.CodeName = userData.Codename;

            var encryptedCitizenshipForRegistration = userData.EncryptedCitizenshipForms.FirstOrDefault(ecf => ecf.CountryCode.Trim() == userData.Person.CountryCode.Trim());

            _certificationMatrix.Hashes.IdentityCard = encryptedCitizenshipForRegistration.CitizenshipHashes.IdentityCard;
            _certificationMatrix.Hashes.IdentityCardBack = encryptedCitizenshipForRegistration.CitizenshipHashes.IdentityCardBack;
            _certificationMatrix.Hashes.Passport = encryptedCitizenshipForRegistration.CitizenshipHashes.Passport;
            _certificationMatrix.Hashes.ProofOfNin = encryptedCitizenshipForRegistration.CitizenshipHashes.ProofOfNin;
            _certificationMatrix.Hashes.BirthCertificate = encryptedCitizenshipForRegistration.CitizenshipHashes.BirthCertificate;

            var citizenships = new List<Citizenship>();

            foreach (var form in userData.EncryptedCitizenshipForms)
            {
                citizenships.Add(new Citizenship
                {
                    CountryCode = form.CountryCode,
                    Nin = form.Nin,
                    SurName = form.SurName,
                    FirstName = form.FirstName,
                    SecondName = string.IsNullOrEmpty(form.SecondName) ? null : form.SecondName,
                    ThirdName = string.IsNullOrEmpty(form.ThirdName) ? null : form.ThirdName,
                    CitizenshipHashes = form.CitizenshipHashes,
                    BirthCountryCode = userData.BirthCountryCode,
                    BirthDate = userData.Person.Birthdate
                });
            }

            _certificationMatrix.Citizenships = citizenships.ToArray();

            var idm = await Task.Run(() => _ediService.GenerateMatrix(_certificationMatrix));

            var edi = await Task.Run(() => _ediService.GenerateEDI(idm));

            userData.Edi = edi;
            userData.CertificationMatrix = _ediService.SerializeMatrix(_certificationMatrix);
            userData.IdentificationMatrix = _ediService.SerializeMatrix(idm);
            userData.CitizenshipMatrices = _ediService.SerializeCitizenships(_certificationMatrix);

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

        _ediFiles
            .GetType()
            .GetProperty(propertyNameInfo.Name)
            .SetValue(_ediFiles, ediFileItem);

        OnPropertyChanged(nameof(GenericHashTableForm));

        userData.GenericHashTableFiles = _ediFiles;

        await _secureRepository.SetAsync("userdata", userData);
    }

    private void SetFileHash(PropertyInfo propertyNameInfo, string fileName, byte[] fileBytes)
    {
        propertyNameInfo.SetValue(GenericHashTableForm, fileName);

        _certificationMatrix.Hashes
            .GetType()
            .GetProperty(propertyNameInfo.Name)
            .SetValue(_certificationMatrix.Hashes, _base16Encoder.Encode(_sha256Hasher.Hash(fileBytes)));
    }

    public async Task Initialize()
    {
        userData = await this._secureRepository.GetAsync<UserData>("userdata");

        if (userData?.GenericHashTableFiles is null) return;

        _ediFiles = userData.GenericHashTableFiles;

        foreach (var propertyInfo in _ediFiles.GetType().GetProperties())
        {
            PropertyInfo propertyNameInfo = GetPropertyInfo(propertyInfo.Name);

            var ediFileItem = (FileItem)propertyInfo.GetValue(_ediFiles, null);

            if (ediFileItem?.Content == null)
            {
                continue;
            }

            propertyNameInfo.SetValue(GenericHashTableForm, ediFileItem.FileName);

            _certificationMatrix.Hashes
                .GetType()
                .GetProperty(propertyNameInfo.Name)
                .SetValue(_certificationMatrix.Hashes, _base16Encoder.Encode(_sha256Hasher.Hash(ediFileItem.Content)));

            _ediFiles
                .GetType()
                .GetProperty(propertyNameInfo.Name)
                .SetValue(_ediFiles, ediFileItem);
        }

        OnPropertyChanged(nameof(GenericHashTableForm));
    }

    private PropertyInfo GetPropertyInfo(string propertyName)
    {
        return this.GenericHashTableForm.GetType().GetProperty(propertyName);
    }
}
