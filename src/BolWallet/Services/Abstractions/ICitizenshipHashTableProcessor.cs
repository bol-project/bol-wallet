using Bol.Core.Model;

namespace BolWallet.Services.Abstractions;

public interface ICitizenshipHashTableProcessor
{
    /// <summary>
    ///  Process a file and update all the hash tables related to the citizenship form.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="propertyName"></param>
    /// <param name="countryCode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ProcessFile(
        FileResult file,
        string propertyName,
        string countryCode,
        CancellationToken cancellationToken);

    CitizenshipHashTableFileNames GetCitizenshipHashTableFileNames();
    CitizenshipHashTable GetCitizenshipHashes();
    CitizenshipHashTable GetCitizenshipActualBytes();
}
