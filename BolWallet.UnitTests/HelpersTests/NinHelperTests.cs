using FluentAssertions;
using BolWallet.Helpers;
using BolWallet.UnitTests.Fixtures;

namespace BolWallet.UnitTests.HelpersTests
{
    public class NinHelperTests : IClassFixture<RegisterContentFixture>
    {
        private readonly INinHelper _sut;

        public NinHelperTests(RegisterContentFixture registerContentFixture)
        {
            _sut = new NinHelper(registerContentFixture.RegisterContent);
        }

        [Theory]
        [InlineData("GRC", "11111111111", true, "")]
        [InlineData("FRA", "1111111111111", true, "")]
        [InlineData("ALB", "K11111111L", true, "")]
        [InlineData("MAR", "111111111", true, "")]
        [InlineData("GRC", "1111111111", false, 
            "The National Identification Number (NIN) provided does not match the expected length of 11 digits for the country code GRC." +
            " Please ensure that only capital letters (A-Z) and numbers are used for NIN.")]
        public void ValidateNin_ReturnsExpectedResult(
            string countryCode,
            string nin,
            bool expectedIsValid,
            string expectedErrorMessage)
        {
            // Act
            var result = _sut.ValidateNin(nin, countryCode);

            // Assert
            result.isValid.Should().Be(expectedIsValid);
            result.errorMessage.Should().Be(expectedErrorMessage);
        }

        [Theory]
        [InlineData("GRC", "AMKA")]
        [InlineData("FRA", "NIR")]
        [InlineData("ALB", "NIPT")]
        [InlineData("MAR", "CNIE")]
        public void GetNinInternationalName_ReturnsExpectedResult(string countryCode, string expectedName)
        {
            // Act
            var result = _sut.GetNinInternationalName(countryCode);

            // Assert
            result.Should().Be(expectedName);
        }
        
        [Theory]
        [InlineData(null, "11111111111")]
        [InlineData("", "11111111111")]
        [InlineData("GRC", null)]
        [InlineData("GRC", "")]
        public void ValidateNin_ReturnsExpectedResult_WhenInputIsNullOrEmpty(string countryCode, string nin)
        {
            // Act
            var result = _sut.ValidateNin(nin, countryCode);

            // Assert
            result.isValid.Should().BeFalse();
            result.errorMessage.Should().NotBeEmpty();
        }

        [Fact]
        public void ValidateNin_ReturnsExpectedResult_WhenCountryCodeDoesNotExist()
        {
            // Arrange
            var countryCode = "XYZ";
            var nin = "11111111111";

            // Act
            var result = _sut.ValidateNin(nin, countryCode);

            // Assert
            result.isValid.Should().BeFalse();
            result.errorMessage.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("XYZ")]
        public void GetNinInternationalName_ReturnsEmpty_WhenCountryCodeIsNullOrEmptyOrDoesNotExist(string countryCode)
        {
            // Act
            var result = _sut.GetNinInternationalName(countryCode);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
