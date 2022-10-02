using Bol.Core.Model;
using DevExpress.Maui.DataForm;
using System.ComponentModel.DataAnnotations;

namespace BolWallet.Models;

public class CodenameForm
{
	[DataFormDisplayOptions(LabelText = "\ue7fd", LabelWidth = "auto")]
	[DataFormItemPosition(RowOrder = 1, ItemOrderInRow = 1)]
	[DataFormTextEditor(InplaceLabelText = "Surname")]
	public string Surname { get; set; }

	[DataFormDisplayOptions(IsLabelVisible = false)]
	[DataFormItemPosition(RowOrder = 1, ItemOrderInRow = 2)]
	[DataFormTextEditor(InplaceLabelText = "Middle name")]
	public string MiddleName { get; set; }

	[DataFormDisplayOptions(IsLabelVisible = false)]
	[DataFormTextEditor(InplaceLabelText = "Third name")]
	[DataFormItemPosition(RowOrder = 1, ItemOrderInRow = 3)]
	public string ThirdName { get; set; }

	[DataFormDisplayOptions(LabelText = "\ue7e9", LabelWidth = "auto")]
	[DataFormTextEditor(InplaceLabelText = "Birthday")]
	public DateTime? Birthday { get; set; }

	[DataFormDisplayOptions(LabelText = "\uf8d9", LabelWidth = "auto")]
	[DataFormComboBoxEditor(InplaceLabelText = "Gender")]
	public Gender Gender { get; set; }

	[DataFormDisplayOptions(LabelText = "\ue80b", LabelWidth = "auto")]
	[DataFormComboBoxEditor(InplaceLabelText = "Country")]
	public string Country { get; set; }

	[DataFormDisplayOptions(LabelText = "\ue9f4", LabelWidth = "auto", HelpText = "To be added")]
	[DataFormTextEditor(InplaceLabelText = "Combination")]
	[MaxLength(1, ErrorMessage = "Combination should be only one digit/letter")]
	[RegularExpression("[A-Z0-9]", ErrorMessage = "Combination should be a capital letter or a number")]
	public string Combination { get; set; } = "1";
}