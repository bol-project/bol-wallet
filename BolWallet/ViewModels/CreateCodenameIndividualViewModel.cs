using Bol.Core.Abstractions;
using Bol.Core.Model;
using CommunityToolkit.Maui.Alerts;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace BolWallet.ViewModels;

public partial class CreateCodenameIndividualViewModel : CreateCodenameViewModel
{
    public CreateCodenameIndividualViewModel(INavigationService navigationService,
        ICodeNameService codeNameService,
        RegisterContent content,
        ISecureRepository secureRepository) : base(navigationService, codeNameService, content, secureRepository) { }
}

