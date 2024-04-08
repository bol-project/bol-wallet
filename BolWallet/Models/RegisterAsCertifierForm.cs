using System.ComponentModel.DataAnnotations;

namespace BolWallet.Models
{
    public class RegisterAsCertifierForm
    {
        [Required]
        public IEnumerable<string> Countries {  get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fee must be greater than 0")]
        public decimal Fee { get; set; }
    }
}
