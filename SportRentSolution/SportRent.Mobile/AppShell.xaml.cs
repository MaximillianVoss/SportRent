using SportRent.Mobile.Pages;

namespace SportRent.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(EquipmentDetailsPage), typeof(EquipmentDetailsPage));
            Routing.RegisterRoute(nameof(CreateOrderPage), typeof(CreateOrderPage));
        }
    }
}
