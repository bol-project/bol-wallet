using Bol.Core.Model;
using Bol.Cryptography;
using Microsoft.Extensions.Logging;

namespace BolWallet.Services;

public class CitizenshipHashTableProcessor : ICitizenshipHashTableProcessor
{
    private readonly IBase16Encoder _base16Encoder;
    private readonly ISha256Hasher _sha256Hasher;
    private readonly ILogger<CitizenshipHashTableProcessor> _logger;
    private CitizenshipHashTableFileNames CitizenshipHashTableFileNames { get; }
    private CitizenshipHashTable CitizenshipHashes { get; }
    private CitizenshipHashTable CitizenshipActualBytes { get; }

    public CitizenshipHashTableProcessor(
        IBase16Encoder base16Encoder,
        ISha256Hasher sha256Hasher,
        ILogger<CitizenshipHashTableProcessor> logger)
    {
        _base16Encoder = base16Encoder;
        _sha256Hasher = sha256Hasher;
        _logger = logger;
        
        CitizenshipHashTableFileNames = new CitizenshipHashTableFileNames();
        CitizenshipHashes = new CitizenshipHashTable();
        CitizenshipActualBytes = new CitizenshipHashTable();
    }

    public async Task ProcessFile(
        FileResult file,
        string propertyName,
        string countryCode,
        CancellationToken cancellationToken)
    {
        if (file != null)
        {
            try
            {
                var fileBytes = await File.ReadAllBytesAsync(file.FullPath, cancellationToken);
                var encodedFileBytes = _base16Encoder.Encode(fileBytes);
                var hashedEncodedFileBytes = _base16Encoder.Encode(_sha256Hasher.Hash(fileBytes));

                var generatedFileName = GenerateHashTableFileName(propertyName, file.FullPath, countryCode);

                CitizenshipHashTableFileNames
                    .GetType()
                    .GetProperty(propertyName)?
                    .SetValue(CitizenshipHashTableFileNames, generatedFileName);
                CitizenshipHashes
                    .GetType()
                    .GetProperty(propertyName)?
                    .SetValue(CitizenshipHashes, hashedEncodedFileBytes);
                CitizenshipActualBytes
                    .GetType()
                    .GetProperty(propertyName)
                    ?.SetValue(CitizenshipActualBytes, encodedFileBytes);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error handling file");
                throw;
            }
        }
    }

    private static string GenerateHashTableFileName(string propertyName, string fullPath, string countryCode)
    {
        return $"{propertyName}_{countryCode}{Path.GetExtension(fullPath)}";
    }

    public CitizenshipHashTableFileNames GetCitizenshipHashTableFileNames()
    {
        return CitizenshipHashTableFileNames;
    }

    public CitizenshipHashTable GetCitizenshipHashes()
    {
        return CitizenshipHashes;
    }

    public CitizenshipHashTable GetCitizenshipActualBytes()
    {
        return CitizenshipActualBytes;
    }
}
