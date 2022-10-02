using Bol.Core.Model;
using DevExpress.Maui.DataForm;

namespace BolWallet.Models;

public class CodenameForm
{
	[DataFormDisplayOptions(LabelIcon = "user", LabelWidth = "auto")]
	[DataFormItemPosition(RowOrder = 1, ItemOrderInRow = 1)]
	[DataFormTextEditor(InplaceLabelText = "Surname")]
	public string Surname { get; set; }

	[DataFormDisplayOptions(IsLabelVisible = false)]
	[DataFormItemPosition(RowOrder = 1, ItemOrderInRow = 2)]
	[DataFormTextEditor(InplaceLabelText = "Middle name")]
	public string MiddleName { get; set; }

	[DataFormDisplayOptions(IsLabelVisible = false)]
	[DataFormTextEditor(InplaceLabelText = "Third name")]
	[DataFormItemPosition(RowOrder = 2, ItemOrderInRow = 1)]
	public string ThirdName { get; set; }

	[DataFormDisplayOptions(LabelWidth = "auto")]
	public DateTime? Birthday { get; set; }

	[DataFormDisplayOptions(LabelWidth = "auto")]
	public Gender Gender { get; set; }

	[DataFormDisplayOptions(LabelWidth = "auto")]
	[DataFormComboBoxEditor]
	public string Country { get; set; }

	[DataFormDisplayOptions(LabelWidth = "auto")]
	public string Combination { get; set; }
}