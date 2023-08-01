using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BolWallet.ViewModels;
public partial class BolCommunityViewModel : BaseViewModel
{
    public string BolCommunityHeaderText => "Bol Community";
    public string CertifyText => "Certify";
    public string DeleteFakeAccountText => "Delete Face Account";
    public string DeleteExpiredAccountText => "Delete Expired Account";
    public string VoteText => "Vote";
    public string SignMessageText => "Sign Message";
    public string BolIdentityText => "Bol Identity";

    public BolCommunityViewModel(INavigationService navigationService) : base(navigationService)
    {

    }
}
