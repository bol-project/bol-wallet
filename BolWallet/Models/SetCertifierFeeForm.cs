using System.ComponentModel.DataAnnotations;

namespace BolWallet.Models
{
    public class SetCertifierFeeForm
    {
        [Required]
        [Range(0.00000001, 0.05, ErrorMessage = "Fee must be between 1pet and 0.05b.")]
        public decimal Fee { get; set; }
    }
}
