using System.IO.Compression;
using System.Reflection;
using System.Text;
using Bol.Cryptography;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using Newtonsoft.Json;

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

    public List<FileItem> CollectFilesForDownload(UserData userdata)
    {
        List<FileItem> files = new List<FileItem>();

        foreach (PropertyInfo property in userdata.GenericHashTableFiles.GetType().GetProperties())
        {
            var ediFileItem = property.GetValue(userdata.GenericHashTableFiles) as FileItem;

            if (ediFileItem?.Content != null)
            {
                files.Add(new FileItem()
                {
                    FileName = ediFileItem.FileName,
                    Content = ediFileItem.Content
                });
            }
        }

        foreach (EncryptedCitizenshipForm encryptedCitizenshipForm in userdata.EncryptedCitizenshipForms)
        {
            foreach (PropertyInfo property in encryptedCitizenshipForm.CitizenshipHashes.GetType().GetProperties())
            {
                var ediFileItem = property.GetValue(encryptedCitizenshipForm.CitizenshipHashes) as string;

                PropertyInfo ediFileName = encryptedCitizenshipForm.CitizenshipHashTableFileNames.GetType().GetProperty(property.Name);

                var fileName = ediFileName.GetValue(encryptedCitizenshipForm.CitizenshipHashTableFileNames) as string;

                if (ediFileItem != Bol.Core.Constants.HASH_ZEROS)
                {
                    files.Add(new FileItem()
                    {
                        FileName = fileName,
                        Content = _base16Encoder.Decode(ediFileItem)
                    });
                }
            }
        }

        files.Add(new FileItem()
        {
            FileName = $"{nameof(userdata.ExtendedEncryptedDigitalMatrix)}.yaml",
            Content = Encoding.UTF8.GetBytes(userdata.ExtendedEncryptedDigitalMatrix)
        });

        files.Add(new FileItem()
        {
            FileName = $"{nameof(userdata.EncryptedDigitalMatrix)}.yaml",
            Content = Encoding.UTF8.GetBytes(userdata.EncryptedDigitalMatrix)
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
            string json = JsonConvert.SerializeObject(data);
            byte[] jsonData = Encoding.UTF8.GetBytes(json);

            using (var stream = new MemoryStream(jsonData))
            {
                var result = await _fileSaver.SaveAsync(fileName, stream, cancellationToken);

                if (result.IsSuccessful)
                {
                    await Toast.Make($"File '{fileName}' saved successfully!").Show();
                }
            }
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
    }
}
