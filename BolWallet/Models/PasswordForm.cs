using System.ComponentModel.DataAnnotations;

namespace BolWallet.Models;
public class PasswordForm
{

    [Required]
    [StringLength(30, ErrorMessage = "Password must be at least 8 characters long.", MinimumLength = 8)]
    [RegularExpression(
        "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{8,}$",
        ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, one number, and one special character."
    )]
    public string Password { get; set; }

    [Required]
    [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
    public string Password2 { get; set; }
}
