using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BolWallet.Services
{
    public enum NavButtons
    {
        Refresh,
        ImportWallet,
        Register,
        Print,
        Add,
        Clear,
        New,
        Menu,
        ToolBar
    }
    public class Breadcrumbs
    {
        
        public bool isRegistered { get; set; }
        public bool isAccountOpen { get; set; }
        public UserData userData { get; set; }
        public List<BreadcrumbItem> _items { get; set; } = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#", disabled: true),
        };

        public event EventHandler<bool> OnUpdate;
        public event EventHandler<NavButtons> OnButtonClick;

        public void NavButtonsClick(NavButtons btn)
        {
            this.OnButtonClick?.Invoke(this, btn);
        }

        public void Update()
        {
            this.OnUpdate?.Invoke(this,true);
        }
    }
}
