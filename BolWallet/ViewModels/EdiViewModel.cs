using Bol.Core.Abstractions;
using Bol.Core.Model;
using Bol.Cryptography;
using FluentValidation;
using Plugin.AudioRecorder;
using System.Runtime.Intrinsics.X86;
using MediaPicker = Microsoft.Maui.Media.MediaPicker;

namespace BolWallet.ViewModels;
public partial class EdiViewModel : BaseViewModel
{
	private readonly IPermissionService _permissionService;
	private readonly IBase16Encoder _base16Encoder;
	private readonly ISecureRepository _secureRepository;
	private readonly IEncryptedDigitalIdentityService _encryptedDigitalIdentityService;

	AudioRecorderService recorder;

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
		recorder = new AudioRecorderService
		{
			AudioSilenceTimeout = TimeSpan.FromMilliseconds(5000),
			TotalAudioTimeout = TimeSpan.FromMilliseconds(5000),
		};
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
	[RelayCommand]
	private async Task RecordAudio()
	{
		if (!await _permissionService.CheckSpeechPermission()) return;
		try
		{
			if (!recorder.IsRecording)
			{
				var audioRecordTask = await recorder.StartRecording();
				var audiofile = await audioRecordTask;

				if (audiofile != null)
				{
					EdiForm.Voice = File.ReadAllBytes(audiofile);
					EdiFormPaths.VoicePath = audiofile;
					OnPropertyChanged(nameof(EdiFormPaths));
				}
				else throw new();
			}
			else
			{
				await recorder.StopRecording();
			}
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException(ex.Message);
		}
	}

	[RelayCommand]
	private async Task StopRecordAudio()
	{
		if (!recorder.IsRecording) return;
		try
		{
			await recorder.StopRecording();
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException(ex.Message);
		}
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
					OnPropertyChanged(nameof(EdiFormPaths));
					break;
				}
			case nameof(EdiFormPaths.DrivingLicencePath):
				{
					EdiForm.DrivingLicense = fileBytes;
					EdiFormPaths.DrivingLicencePath = fileResult.FullPath;
					OnPropertyChanged(nameof(EdiFormPaths));
					break;
				}
			case nameof(EdiFormPaths.PhotoPath):
				{
					EdiForm.Photo = fileBytes;
					EdiFormPaths.PhotoPath = fileResult.FullPath;
					OnPropertyChanged(nameof(EdiFormPaths));
					break;
				}
			case nameof(EdiFormPaths.NinCertificatePath):
				{
					EdiForm.NinCertificate = fileBytes;
					EdiFormPaths.NinCertificatePath = fileResult.FullPath;
					OnPropertyChanged(nameof(EdiFormPaths));
					break;
				}
			case nameof(EdiFormPaths.BirthCertificatePath):
				{
					EdiForm.BirthCertificate = fileBytes;
					EdiFormPaths.BirthCertificatePath = fileResult.FullPath;
					OnPropertyChanged(nameof(EdiFormPaths));
					break;
				}
			case nameof(EdiFormPaths.TelephoneBillPath):
				{
					EdiForm.TelephoneBill = fileBytes;
					EdiFormPaths.TelephoneBillPath = fileResult.FullPath;
					OnPropertyChanged(nameof(EdiFormPaths));
					break;
				}
			case nameof(EdiFormPaths.OtherPath):
				{
					EdiForm.Other = fileBytes;
					EdiFormPaths.OtherPath = fileResult.FullPath;
					OnPropertyChanged(nameof(EdiFormPaths));
					break;
				}
			case nameof(EdiFormPaths.VoicePath):
				{
					EdiForm.Voice = fileBytes;
					EdiFormPaths.VoicePath = fileResult.FullPath;
					OnPropertyChanged(nameof(EdiFormPaths));
					break;
				}
		}
	}
	[RelayCommand]
	private async Task Submit()
	{
		if (!(EdiForm.DrivingLicense?.Length > 0 && EdiForm.IdentityCard?.Length > 0 && EdiForm.Passport?.Length > 0))
		{
			return;
		}

		EncryptedDigitalMatrix matrix = EdiFormModelToEncryptedDigitalMatrix();

		var result = _encryptedDigitalIdentityService.Generate(matrix);

		await _secureRepository.SetAsync("edi", result);
	}

	private EncryptedDigitalMatrix EdiFormModelToEncryptedDigitalMatrix()
	{
		EncryptedDigitalMatrix encryptedDigitalMatrix = new EncryptedDigitalMatrix();
		encryptedDigitalMatrix.Hashes = new HashTable();

		if (EdiForm.BirthCertificate != null) encryptedDigitalMatrix.Hashes.BirthCertificate = _base16Encoder.Encode(EdiForm.BirthCertificate);
		if (EdiForm.DrivingLicense != null) encryptedDigitalMatrix.Hashes.DrivingLicense = _base16Encoder.Encode(EdiForm.DrivingLicense);
		if (EdiForm.IdentityCard != null) encryptedDigitalMatrix.Hashes.IdentityCard = _base16Encoder.Encode(EdiForm.IdentityCard);
		if (EdiForm.NinCertificate != null) encryptedDigitalMatrix.Hashes.NinCertificate = _base16Encoder.Encode(EdiForm.NinCertificate);
		if (EdiForm.Passport != null) encryptedDigitalMatrix.Hashes.Passport = _base16Encoder.Encode(EdiForm.Passport);
		if (EdiForm.Voice != null) encryptedDigitalMatrix.Hashes.Voice = _base16Encoder.Encode(EdiForm.Voice);
		if (EdiForm.TelephoneBill != null) encryptedDigitalMatrix.Hashes.TelephoneBill = _base16Encoder.Encode(EdiForm.TelephoneBill);
		if (EdiForm.Other != null) encryptedDigitalMatrix.Hashes.Other = _base16Encoder.Encode(EdiForm.Other);
		if (EdiForm.TextInfo != null) encryptedDigitalMatrix.Hashes.TextInfo = _base16Encoder.Encode(EdiForm.TextInfo);

		return encryptedDigitalMatrix;
	}
}
