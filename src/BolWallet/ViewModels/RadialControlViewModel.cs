using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BolWallet.ViewModels
{
    public class RadialControlViewModel
    {
        public RadialControlViewModel()
        {
            ItemSource = new List<BolWallet.Controls.MenuItem>
        {
            new BolWallet.Controls.MenuItem("Apple", "fa-apple"),
            new BolWallet.Controls.MenuItem("Android", "fa-android"),
            new BolWallet.Controls.MenuItem("Windows", "fa-windows"),
            new BolWallet.Controls.MenuItem("Chip", "fa-microchip"),
            new BolWallet.Controls.MenuItem("Pizza", "fa-code"),
            new BolWallet.Controls.MenuItem("Terminal", "fa-terminal"),
            new BolWallet.Controls.MenuItem("Fire", "fa-fire-extinguisher"),
            new BolWallet.Controls.MenuItem("Bug", "fa-bug"),
            new BolWallet.Controls.MenuItem("Coffee", "fa-coffee"),
            new BolWallet.Controls.MenuItem("Developer", "fa-user-secret"),
        };
        }

        public IReadOnlyList<BolWallet.Controls.MenuItem> ItemSource { get; }
    }
}
