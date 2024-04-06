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
    private readonly IEncryptedDigitalIdentityService _encryptedDigitalIdentityService;
    private readonly IMediaService _mediaService;
    private ExtendedEncryptedDigitalMatrix extendedEncryptedDigitalMatrix;
    public GenericHashTableFiles ediFiles;
    private readonly IAudioRecorder _recorder;

    public CreateEdiViewModel(
        INavigationService navigationService,
        IPermissionService permissionService,
        IBase16Encoder base16Encoder,
        ISha256Hasher sha256Hasher,
        ISecureRepository secureRepository,
        IEncryptedDigitalIdentityService encryptedDigitalIdentityService,
        IAudioManager audioManager,
        IMediaService mediaService)
        : base(navigationService)
    {
        _permissionService = permissionService;
        _base16Encoder = base16Encoder;
        _sha256Hasher = sha256Hasher;
        _secureRepository = secureRepository;
        _encryptedDigitalIdentityService = encryptedDigitalIdentityService;
        _mediaService = mediaService;
        extendedEncryptedDigitalMatrix = new ExtendedEncryptedDigitalMatrix { Hashes = new GenericHashTable() };
        GenericHashTableForm = new GenericHashTableForm();
        ediFiles = new GenericHashTableFiles() { };
        _recorder = audioManager.CreateRecorder();
    }
    
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

            extendedEncryptedDigitalMatrix.CodeName = userData.Codename;

            var encryptedCitizenshipForRegistration = userData.EncryptedCitizenshipForms.FirstOrDefault(ecf => ecf.CountryCode.Trim() == userData.Person.CountryCode.Trim());

            extendedEncryptedDigitalMatrix.Hashes.IdentityCard = encryptedCitizenshipForRegistration.CitizenshipHashes.IdentityCard;
            extendedEncryptedDigitalMatrix.Hashes.IdentityCardBack = encryptedCitizenshipForRegistration.CitizenshipHashes.IdentityCardBack;
            extendedEncryptedDigitalMatrix.Hashes.Passport = encryptedCitizenshipForRegistration.CitizenshipHashes.Passport;
            extendedEncryptedDigitalMatrix.Hashes.ProofOfNin = encryptedCitizenshipForRegistration.CitizenshipHashes.ProofOfNin;
            extendedEncryptedDigitalMatrix.Hashes.BirthCertificate = encryptedCitizenshipForRegistration.CitizenshipHashes.BirthCertificate;

            List<EncryptedCitizenship> encryptedCitizenships = new List<EncryptedCitizenship>();

            foreach (var form in userData.EncryptedCitizenshipForms)
            {
                encryptedCitizenships.Add(new EncryptedCitizenship
                {
                    CountryCode = form.CountryCode,
                    Nin = form.Nin,
                    SurName = form.SurName,
                    FirstName = form.FirstName,
                    SecondName = form.SecondName,
                    ThirdName = form.ThirdName,
                    CitizenshipHashes = form.CitizenshipHashes,
                    BirthCountryCode = userData.BirthCountryCode,
                    BirthDate = userData.Person.Birthdate
                });
            }

            extendedEncryptedDigitalMatrix.CitizenshipMatrices = encryptedCitizenships.ToArray();

            var edm = await Task.Run(() => _encryptedDigitalIdentityService.GenerateMatrix(extendedEncryptedDigitalMatrix));

            var edi = await Task.Run(() => _encryptedDigitalIdentityService.GenerateEDI(edm));

            extendedEncryptedDigitalMatrix.Citizenships = edm.Citizenships;
            extendedEncryptedDigitalMatrix.Citizenships = Array.Empty<string>();

            userData.Edi = edi;
            userData.ExtendedEncryptedDigitalMatrix = _encryptedDigitalIdentityService.SerializeMatrix(extendedEncryptedDigitalMatrix);
            userData.EncryptedDigitalMatrix = _encryptedDigitalIdentityService.SerializeMatrix(edm);

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

        OnPropertyChanged(nameof(GenericHashTableForm));

        userData.GenericHashTableFiles = ediFiles;

        await _secureRepository.SetAsync("userdata", userData);
    }

    private void SetFileHash(PropertyInfo propertyNameInfo, string fileName, byte[] fileBytes)
    {
        propertyNameInfo.SetValue(GenericHashTableForm, fileName);

        extendedEncryptedDigitalMatrix.Hashes
            .GetType()
            .GetProperty(propertyNameInfo.Name)
            .SetValue(extendedEncryptedDigitalMatrix.Hashes, _base16Encoder.Encode(_sha256Hasher.Hash(fileBytes)));
    }

    public async Task Initialize()
    {
        userData = await this._secureRepository.GetAsync<UserData>("userdata");

        if (userData?.GenericHashTableFiles is null) return;

        ediFiles = userData.GenericHashTableFiles;

        foreach (var propertyInfo in ediFiles.GetType().GetProperties())
        {
            PropertyInfo propertyNameInfo = GetPropertyInfo(propertyInfo.Name);

            var ediFileItem = (FileItem)propertyInfo.GetValue(ediFiles, null);

            if (ediFileItem?.Content == null)
            {
                continue;
            }

            propertyNameInfo.SetValue(GenericHashTableForm, ediFileItem.FileName);

            extendedEncryptedDigitalMatrix.Hashes
                .GetType()
                .GetProperty(propertyNameInfo.Name)
                .SetValue(extendedEncryptedDigitalMatrix.Hashes, _base16Encoder.Encode(_sha256Hasher.Hash(ediFileItem.Content)));

            ediFiles
                .GetType()
                .GetProperty(propertyNameInfo.Name)
                .SetValue(ediFiles, ediFileItem);
        }

        OnPropertyChanged(nameof(GenericHashTableForm));
    }

    private PropertyInfo GetPropertyInfo(string propertyName)
    {
        return this.GenericHashTableForm.GetType().GetProperty(propertyName);
    }
}
