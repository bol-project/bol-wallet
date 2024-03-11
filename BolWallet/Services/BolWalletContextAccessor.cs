using Bol.Core.Abstractions;
using Bol.Core.Accessors;
using Bol.Core.Model;
using Microsoft.Extensions.Options;

namespace BolWallet.Services
{
    public class BolWalletContextAccessor(
        WalletContextAccessor walletContextAccessor,
        IOptions<BolConfig> bolConfig) : IContextAccessor
    {
        public IBolContext GetContext()
        {
            var bolContext = walletContextAccessor.GetContext();

            return new BolContext(
                bolConfig.Value.Contract,
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
