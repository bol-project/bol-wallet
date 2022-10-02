using Bol.Core.Model;
using DevExpress.Maui.DataForm;

namespace BolWallet.Models;

public class CodenameForm
{
	[DataFormItemPosition(RowOrder = 1, ItemOrderInRow = 1)]
	public string Surname { get; set; }

	[DataFormItemPosition(RowOrder = 2, ItemOrderInRow = 1)]
	public string MiddleName { get; set; }

	[DataFormItemPosition(RowOrder = 2, ItemOrderInRow = 2)]
	public string ThirdName { get; set; }

	public DateTime DateOfBirth { get; set; }
	
	public Gender Gender { get; set; }

	[DataFormComboBoxEditor]
	public string Country { get; set; }

	public string Combination { get; set; }
}