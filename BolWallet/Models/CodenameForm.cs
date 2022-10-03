using Bol.Core.Model;
using DevExpress.Maui.DataForm;
using System.ComponentModel.DataAnnotations;

namespace BolWallet.Models;

public class CodenameForm
{
	[DataFormDisplayOptions(LabelText = "\ue7fd")]
	[DataFormItemPosition(RowOrder = 1, ItemOrderInRow = 1)]
	[DataFormTextEditor(InplaceLabelText = "Firstname")]
	[RegularExpression("[A-Z]+", ErrorMessage = "Firstname should be only in capital letters")]
	public string Firstname { get; set; }

	[DataFormDisplayOptions(LabelText = "", LabelWidth = "auto")]
	[DataFormItemPosition(RowOrder = 1, ItemOrderInRow = 2)]
	[DataFormTextEditor(InplaceLabelText = "Surname")]
	[RegularExpression("[A-Z]+", ErrorMessage = "Surname should be only in capital letters")]
	public string Surname { get; set; }

	[DataFormDisplayOptions(LabelText = "")]
	[DataFormItemPosition(RowOrder = 2, ItemOrderInRow = 1)]
	[DataFormTextEditor(InplaceLabelText = "Middle name")]
	[RegularExpression("[A-Z]*", ErrorMessage = "Middle name should be only in capital letters")]
	public string MiddleName { get; set; }

	[DataFormDisplayOptions(LabelText = "", LabelWidth = "auto")]
	[DataFormItemPosition(RowOrder = 2, ItemOrderInRow = 2)]
	[DataFormTextEditor(InplaceLabelText = "Third name")]
	[RegularExpression("[A-Z]*", ErrorMessage = "Third name should be only in capital letters")]
	public string ThirdName { get; set; }

	[DataFormDisplayOptions(LabelText = "\ue7e9")]
	[DataFormDateEditor(InplaceLabelText = "Birthday")]
	public DateTime? Birthday { get; set; }

	[DataFormDisplayOptions(LabelText = "\uf8d9")]
	[DataFormComboBoxEditor(InplaceLabelText = "Gender")]
	public Gender Gender { get; set; }

	[DataFormDisplayOptions(LabelText = "\ue80b")]
	[DataFormComboBoxEditor(
		InplaceLabelText = "Country",
		ValueMember = nameof(Models.Country.Alpha3),
		DisplayMember = nameof(Models.Country.Name))]
	public string Country { get; set; }

	[DataFormDisplayOptions(LabelText = "\ue9f4", HelpText = "To be added")]
	[DataFormTextEditor(InplaceLabelText = "Combination")]
	[MaxLength(1, ErrorMessage = "Combination should be only one digit/letter")]
	[RegularExpression("[A-Z0-9]", ErrorMessage = "Combination should be a capital letter or a number")]
	public string Combination { get; set; } = "1";
}