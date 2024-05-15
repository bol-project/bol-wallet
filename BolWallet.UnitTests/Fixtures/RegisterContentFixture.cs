using Bol.Core.Model;
using BolWallet.Models;
using Country = BolWallet.Models.Country;

namespace BolWallet.UnitTests.Fixtures;

public class RegisterContentFixture
{
    public readonly List<Country> Countries;
    public readonly List<NinSpecification> NinSpecifications;

    public RegisterContent RegisterContent { get; }

    public RegisterContentFixture()
    {
        Countries = new List<Country>
        {
            new()
            {
                Name = "Greece",
                Alpha3 = "GRC",
                Region = "Europe"
            },
            new()
            {
                Name = "France",
                Alpha3 = "FRA",
                Region = "Europe"
            },
            new()
            {
                Name = "Albania",
                Alpha3 = "ALB",
                Region = "Europe"
            },
            new()
            {
                Name = "Morocco",
                Alpha3 = "MAR",
                Region = "Africa"
            }
        };
        
        NinSpecifications = new List<NinSpecification>()
        {
            new()
            {
                Country = "Greece",
                CountryCode = "GRC",
                InternationalName = "AMKA",
                LocalName = "AMKA",
                Format = "11111111111",
                Digits = 11,
                Regex = "^[0-9]{11}$",
                Status = NinStatus.Active
            },
            new()
            {
                Country = "France",
                CountryCode = "FRA",
                InternationalName = "NIR",
                LocalName = "NIR",
                Format = "1111111111111",
                Digits = 13,
                Regex = "^[0-9]{13}$",
                Status = NinStatus.Active
            },
            new()
            {
                Country = "Albania",
                CountryCode = "ALB",
                InternationalName = "NIPT",
                LocalName = "NIPT",
                Format = "K11111111L",
                Digits = 10,
                Regex = "^[A-Z][0-9]{8}[A-Z]$",
                Status = NinStatus.Active
            },
            new()
            {
                Country = "Morocco",
                CountryCode = "MAR",
                InternationalName = "CNIE",
                LocalName = "CNIE",
                Format = "111111111",
                Digits = 9,
                Regex = "^[0-9]{9}$",
                Status = NinStatus.Active
            }
        };
        
        RegisterContent = new RegisterContent
        {
            Countries = Countries,
            NinPerCountryCode = NinSpecifications.ToDictionary(x => x.CountryCode)
        };
    }
}
