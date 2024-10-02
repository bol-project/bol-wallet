using Bol.Core.Abstractions;
using Bol.Core.Accessors;

namespace BolWallet.Services
{
    public class BolWalletContextAccessor(
        WalletContextAccessor walletContextAccessor,
        INetworkPreferences networkPreferences) : IContextAccessor
    {
        public IBolContext GetContext()
        {
            var bolContext = walletContextAccessor.GetContext();
            if (bolContext is UnauthorizedBolContext)
            {
                return bolContext;
            }

            return new BolContext(
                networkPreferences.TargetNetworkConfig.Contract,
                bolContext.CodeName,
                bolContext.Edi,
                bolContext.CodeNameKey,
                bolContext.PrivateKey,
                bolContext.MainAddress,
                bolContext.BlockChainAddress,
                bolContext.SocialAddress,
                bolContext.VotingAddress,
                bolContext.CommercialAddresses);
        }
    }
}
