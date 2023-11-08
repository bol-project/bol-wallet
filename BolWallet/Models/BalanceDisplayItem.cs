namespace BolWallet.Models
{
	public class BalanceDisplayItem : ObservableObject
	{
		public string Address { get; set; }
		public string Balance { get; set; }

		public BalanceDisplayItem(string address, string balance)
		{
			Address = address;
			Balance = balance;
		}
	}
}
