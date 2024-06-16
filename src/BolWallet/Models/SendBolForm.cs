using Bol.Core.Model;
using System.Numerics;

namespace BolWallet.Models;

public partial class SendBolForm : ObservableObject
{
	[ObservableProperty]
	public string _comAddress;

	[ObservableProperty]
	public string _receiverCodename;

	[ObservableProperty]
	public string _receiverAddress;

	private string _amount;
	public string Amount
	{
		get => _amount;
		set
		{
			_amount = value;
			if (decimal.TryParse(_amount, out var decimalValue))
			{
				_actualAmount = decimal.Round(decimalValue, 8);
			}
			else
				_actualAmount = 0;
			OnPropertyChanged();
			OnPropertyChanged(nameof(ActualAmount));
		}
	}

	private decimal _actualAmount;
	public decimal ActualAmount
	{
		get { return _actualAmount; }
	}
}
