using BolWallet.Bolnformation;
using BolWallet.Components;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Extensions.Logging;
using MudBlazor;
using SimpleResults;
using Bol.Core.Abstractions;
using CommunityToolkit.Maui.Core;

namespace BolWallet.ViewModels
{
    public partial class DownloadCertificationDocumentsViewModel : BaseViewModel
    {
        private readonly ISecureRepository _secureRepository;
        private readonly IFileDownloadService _fileDownloadService;
        private readonly IDialogService _dialogService;
        private readonly IBolService _bolService;

        public DownloadCertificationDocumentsViewModel(
            INavigationService navigationService,
            ISecureRepository secureRepository,
            IFileDownloadService fileDownloadService,
            ILogger<DownloadCertificationDocumentsViewModel> logger,
            IDialogService dialogService,
            IBolService bolService
            ) : base(navigationService)
        {
            _secureRepository = secureRepository;
            _fileDownloadService = fileDownloadService;
            _dialogService = dialogService;
            _bolService = bolService;
        }

        [ObservableProperty] private List<FileItem> _files;

        [ObservableProperty] protected bool _isLoading = false;

        public async Task OnInitializeAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                userData = await _secureRepository.GetAsync<UserData>("userdata");

                if (userData.IsIndividualRegistration)
                    Files = _fileDownloadService.CollectIndividualFilesForDownload(userData);
                else
                    Files = _fileDownloadService.CollectCompanyFilesForDownload(userData);
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show(cancellationToken);
            }
        }

        [RelayCommand]
        public async Task DownloadAllDocuments(List<FileItem> files)
        {
            var ediZipFiles = await _fileDownloadService.CreateZipFileAsync(files);

            await _fileDownloadService.SaveZipFileAsync(ediZipFiles);
        }

        [RelayCommand]
        public async Task DownloadDocument(FileItem file)
        {
            await _fileDownloadService.DownloadDataAsync(file.Content, file.FileName);
        }

        [RelayCommand]
        public async Task CheckCodenameExistanceAndProceedd(CancellationToken token = default)
        {
            try
            {
                var codenameExistsResult = await CodenameExists(userData.Codename, token);

                switch (codenameExistsResult.IsSuccess)
                {
                    case true when !codenameExistsResult.Data.Exists:
                        {
                            await NavigationService.NavigateTo<MainWithAccountViewModel>();

                            return;
                        }
                    case true when codenameExistsResult.Data.Exists:
                        {
                            var alternatives = string.Join(Environment.NewLine, codenameExistsResult.Data.Alternatives);

                            await Toast
                                .Make($"The codename already exists. Found:{Environment.NewLine}{alternatives}",
                                    ToastDuration.Long)
                                .Show(token);

                            return;
                        }
                    case false:
                        {
                            await Toast.Make($"Codename existence check failed with error: {codenameExistsResult.Message}",
                                ToastDuration.Long).Show(token);
                            return;
                        }
                }
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show(token);
            }

        }

        public void OpenGeneratedDocumentsMoreInfo()
        {
            var parameters = new DialogParameters<MoreInfoDialog>
        {
            { x => x.Title, GeneratedDocumentsInformation.Title },
            { x => x.Paragraph1, GeneratedDocumentsInformation.Description },
            { x => x.Paragraph2, GeneratedDocumentsInformation.Content }
        };

            var options = new DialogOptions { CloseOnEscapeKey = true };

            _dialogService.Show<MoreInfoDialog>("", parameters, options);
        }

        protected async Task<Result<CodenameExistsCheck>> CodenameExists(string codename, CancellationToken token = default)
        {
            try
            {
                IsLoading = true;
                var alternatives = (await _bolService.FindAlternativeCodeNames(codename, token)).ToArray();

                return alternatives.Length == 0
                    ? Result.Success(CodenameExistsCheck.CodenameDoesNotExist())
                    : Result.Success(CodenameExistsCheck.CodenameExists(alternatives));
            }
            catch (Exception e)
            {
                return Result.CriticalError(e.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
