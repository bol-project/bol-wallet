using System.IO.Compression;
using System.Reflection;
using System.Text;
using Bol.Cryptography;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;

namespace BolWallet.Services;
public class FileDownloadService : IFileDownloadService
{
    private readonly IFileSaver _fileSaver;
    private readonly IBase16Encoder _base16Encoder;

    public FileDownloadService(
        IFileSaver fileSaver,
        IBase16Encoder base16Encoder)
    {
        _fileSaver = fileSaver;
        _base16Encoder = base16Encoder;
    }

    public List<FileItem> CollectIndividualFilesForDownload(UserData userdata)
    {
        var files = new List<FileItem>();

        foreach (PropertyInfo property in userdata.GenericHashTableFiles.GetType().GetProperties())
        {
            var ediFileItem = property.GetValue(userdata.GenericHashTableFiles) as FileItem;

            if (ediFileItem?.Content != null)
            {
                files.Add(new FileItem()
                {
                    FileName = $"{Path.GetFileNameWithoutExtension(ediFileItem.FileName)}_{userdata.GetShortHash()}{Path.GetExtension(ediFileItem.FileName)}",
                    Content = ediFileItem.Content
                });
            }
        }

        foreach (EncryptedCitizenshipData encryptedCitizenshipForm in userdata.EncryptedCitizenshipForms)
        {
            foreach (PropertyInfo property in encryptedCitizenshipForm.CitizenshipActualBytes.GetType().GetProperties())
            {
                var ediFileItem = property.GetValue(encryptedCitizenshipForm.CitizenshipActualBytes) as string;

                PropertyInfo ediFileName = encryptedCitizenshipForm.CitizenshipHashTableFileNames.GetType().GetProperty(property.Name);

                var fileName = ediFileName.GetValue(encryptedCitizenshipForm.CitizenshipHashTableFileNames) as string;

                if (ediFileItem != Bol.Core.Constants.HASH_ZEROS)
                {
                    files.Add(new FileItem()
                    {
                        FileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{userdata.GetShortHash()}{Path.GetExtension(fileName)}",
                        Content = _base16Encoder.Decode(ediFileItem)
                    });
                }
            }
        }

        files.Add(new FileItem()
        {
            FileName = $"{nameof(userdata.CertificationMatrix)}_{userdata.GetShortHash()}.yaml",
            Content = Encoding.UTF8.GetBytes(userdata.CertificationMatrix)
        });

        files.Add(new FileItem()
        {
            FileName = $"{nameof(userdata.IdentificationMatrix)}_{userdata.GetShortHash()}.yaml",
            Content = Encoding.UTF8.GetBytes(userdata.IdentificationMatrix)
        });

        files.AddRange(userdata.CitizenshipMatrices
            .Select(((citizenship, i) => new FileItem
            {
                FileName = $"Citizenship_{i}_{userdata.GetShortHash()}.yaml",
                Content = Encoding.UTF8.GetBytes(citizenship)
            })));

        return files;
    }

    public List<FileItem> CollectCompanyFilesForDownload(UserData userdata)
    {
        var files = new List<FileItem>();

        foreach (PropertyInfo property in userdata.CompanyHashFiles.GetType().GetProperties())
        {
            var ediFileItem = property.GetValue(userdata.CompanyHashFiles) as FileItem;

            if (ediFileItem?.Content != null)
            {
                files.Add(new FileItem()
                {
                    FileName = ediFileItem.FileName,
                    Content = ediFileItem.Content
                });
            }
        }

        files.Add(new FileItem()
        {
            FileName = $"{nameof(userdata.CertificationMatrixCompany)}_{userdata.GetShortHash()}.yaml",
            Content = Encoding.UTF8.GetBytes(userdata.CertificationMatrixCompany)
        });

        files.Add(new FileItem()
        {
            FileName = $"{nameof(userdata.IdentificationMatrixCompany)}_{userdata.GetShortHash()}.yaml",
            Content = Encoding.UTF8.GetBytes(userdata.IdentificationMatrixCompany)
        });

        files.Add(new FileItem()
        {
            FileName = $"{nameof(userdata.IncorporationMatrix)}_{userdata.GetShortHash()}.yaml",
            Content = Encoding.UTF8.GetBytes(userdata.IncorporationMatrix)
        });

        return files;
    }

    public async Task SaveZipFileAsync(byte[] ediZipFiles, CancellationToken cancellationToken)
    {
        using (var stream = new MemoryStream(ediZipFiles))
        {
            var result = await _fileSaver.SaveAsync("certification-documents.zip", stream, cancellationToken);

            if (result.IsSuccessful)
            {
                await Toast.Make($"File 'certification-documents.zip' saved successfully!").Show();
            }
        }
    }

    public async Task<byte[]> CreateZipFileAsync(IEnumerable<FileItem> files, CancellationToken cancellationToken = default)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    if (file.Content != null)
                    {
                        var zipEntry = archive.CreateEntry(file.FileName, CompressionLevel.Optimal);
                        using (var entryStream = zipEntry.Open())
                        {
                            await entryStream.WriteAsync(file.Content, 0, file.Content.Length, cancellationToken);
                        }
                    }
                }
            }

            return memoryStream.ToArray();
        }
    }

    public async Task DownloadDataAsync<T>(T data, string fileName, CancellationToken cancellationToken = default)
    {
        try
        {
            string json = JsonSerializer.Serialize(data, Constants.WalletJsonSerializerDefaultOptions);
            byte[] jsonData = Encoding.UTF8.GetBytes(json);

            await SaveBytesToFileAsync(jsonData, fileName, cancellationToken);
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
    }

    public async Task DownloadDataAsync(byte[] data, string fileName, CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveBytesToFileAsync(data, fileName, cancellationToken);
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
    }

    private async Task SaveBytesToFileAsync(byte[] data, string fileName, CancellationToken cancellationToken = default)
    {
        using (var stream = new MemoryStream(data))
        {
            var result = await _fileSaver.SaveAsync(fileName, stream, cancellationToken);

            if (result.IsSuccessful)
            {
                await Toast.Make($"File '{fileName}' saved successfully!").Show();
            }
        }
    }
}
