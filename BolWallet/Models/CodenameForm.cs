using Bol.Core.Model;
using DevExpress.Maui.DataForm;

namespace BolWallet.Models;

public class CodenameForm
{
	[DataFormComboBoxEditor]
	public string Country { get; set; }

	public string Surname { get; set; }

	public string MiddleName { get; set; }

	public string ThirdName { get; set; }

	public Gender Gender { get; set; }

	public DateTime DateOfBirth { get; set; }

	public string Combination { get; set; }
}