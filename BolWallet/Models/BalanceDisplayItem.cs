namespace BolWallet.Models
{
	public class BalanceDisplayItem : ObservableObject
	{
		public string BalanceInfo { get; set; }

		public BalanceDisplayItem(string balanceInfo)
		{
			BalanceInfo = balanceInfo;
		}
	}
}
