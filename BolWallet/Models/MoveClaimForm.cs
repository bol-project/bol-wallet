using System.Numerics;

namespace BolWallet.Models;
public partial class MoveClaimForm : ObservableObject
{
	[ObservableProperty]
	public string _comAddress;

	private string _amount;
	public string Amount
	{
		get => _amount;
		set
		{
			_amount = value;
			if (decimal.TryParse(_amount, out var decimalValue))
			{
				_actualAmount = new BigInteger(decimalValue * (decimal)Math.Pow(10, 8));
			}
			OnPropertyChanged();
			OnPropertyChanged(nameof(ActualAmount));
		}
	}

	private BigInteger _actualAmount;
	public BigInteger ActualAmount
	{
		get { return _actualAmount; }
	}
}

