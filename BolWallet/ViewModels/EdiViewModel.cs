using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Cryptography;
using FluentValidation;
using MediaPicker = Microsoft.Maui.Media.MediaPicker;

namespace BolWallet.ViewModels;
public partial class EdiViewModel : BaseViewModel
{
	private readonly IPermissionService _permissionService;
	private readonly IBase16Encoder _base16Encoder;
	private readonly ISecureRepository _secureRepository;
	private readonly IEncryptedDigitalIdentityService _encryptedDigitalIdentityService;

	public EdiViewModel(INavigationService navigationService,
		IPermissionService permissionService,
		IBase16Encoder base16Encoder,
		ISecureRepository secureRepository,
		IEncryptedDigitalIdentityService encryptedDigitalIdentityService)
		: base(navigationService)
	{
		_permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
		_base16Encoder = base16Encoder ?? throw new ArgumentNullException(nameof(base16Encoder));
		_secureRepository = secureRepository ?? throw new ArgumentNullException(nameof(secureRepository));
		_encryptedDigitalIdentityService = encryptedDigitalIdentityService ?? throw new ArgumentNullException(nameof(encryptedDigitalIdentityService));
		EdiForm = new EdiForm();
		EdiFormPaths = new EdiFormPaths();
	}

	[ObservableProperty]
	private EdiForm _ediForm;

	[ObservableProperty]
	private EdiFormPaths _ediFormPaths;

	[RelayCommand]
	private async Task PickPhotoAsync(string propertyName)
	{
		if (!(await _permissionService.CheckStoragePermission())) return;
		var customFileType =
			new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
		{
					 { DevicePlatform.iOS, new[] { "com.adobe.pdf" } },
					 { DevicePlatform.Android, new[] { "application/pdf", "image/*","audio/*" } },
					 { DevicePlatform.Tizen, new[] { "*/*" } },
					 { DevicePlatform.macOS, new[] { "pdf"} },
			});
		var pickResult = await FilePicker.PickAsync(new PickOptions
		{
			FileTypes = customFileType,
			PickerTitle = "Pick a file"
		});

		PathPerImport(propertyName, pickResult);

	}
	[RelayCommand]
	private async Task TakePhotoAsync(string propertyName)
	{
		if (!(await _permissionService.CheckCameraPermission())) return;

		var takePictureResult = await MediaPicker.CapturePhotoAsync();

		PathPerImport(propertyName, takePictureResult);
	}

	private void PathPerImport(string propertyName, FileResult fileResult)
	{
		byte[] fileBytes = null;

		if (fileResult != null) fileBytes = File.ReadAllBytes(fileResult.FullPath);
		else return;

		switch (propertyName)
		{
			case nameof(EdiFormPaths.IdentityCardPath):
				{
					EdiForm.IdentityCard = fileBytes;
					EdiFormPaths.IdentityCardPath = fileResult.FullPath;
					OnPropertyChanged(nameof(EdiFormPaths));
					break;
				}
			case nameof(EdiFormPaths.PassportPath):
				{
					EdiForm.Passport = fileBytes;
					EdiFormPaths.PassportPath = fileResult.FullPath;
					break;
				}
			case nameof(EdiFormPaths.DrivingLicencePath):
				{
					EdiForm.DrivingLicense = fileBytes;
					EdiFormPaths.DrivingLicencePath = fileResult.FullPath;
					break;
				}
			case nameof(EdiFormPaths.PhotoPath):
				{
					EdiForm.Photo = fileBytes;
					EdiFormPaths.PhotoPath = fileResult.FullPath;
					break;
				}
			case nameof(EdiFormPaths.NinCertificatePath):
				{
					EdiForm.NinCertificate = fileBytes;
					EdiFormPaths.NinCertificatePath = fileResult.FullPath;
					break;
				}
			case nameof(EdiFormPaths.BirthCertificatePath):
				{
					EdiForm.BirthCertificate = fileBytes;
					EdiFormPaths.BirthCertificatePath = fileResult.FullPath;
					break;
				}
			case nameof(EdiFormPaths.TelephoneBillPath):
				{
					EdiForm.TelephoneBill = fileBytes;
					EdiFormPaths.TelephoneBillPath = fileResult.FullPath;
					break;
				}
			case nameof(EdiFormPaths.OtherPath):
				{
					EdiForm.Other = fileBytes;
					EdiFormPaths.OtherPath = fileResult.FullPath;
					break;
				}
			case nameof(EdiFormPaths.VoicePath):
				{
					EdiForm.Voice = fileBytes;
					EdiFormPaths.VoicePath = fileResult.FullPath;
					break;
				}
		}
	}
	[RelayCommand]
	private async Task Submit()
	{

	}
}
