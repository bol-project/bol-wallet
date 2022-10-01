using Bol.Core.Model;

namespace BolWallet.Models;

public class CreateCodenameForm
{
	public string CountryCode { get; set; }
	public string Surname { get; set; }
	public string MiddleName { get; set; }
	public string ThirdName { get; set; }
	public Gender Gender { get; set; }
	public DateTime DateOfBirth { get; set; }
	public string Combination { get; set; }
}