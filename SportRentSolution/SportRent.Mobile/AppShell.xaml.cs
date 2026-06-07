using SportRent.Mobile.Pages;

namespace SportRent.Mobile
{
    /// <summary>
    /// Shell основного приложения с вкладками каталога, заказов и профиля.
    /// </summary>
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            // Вложенные страницы регистрируются отдельно, потому что они открываются из вкладки каталога.
            Routing.RegisterRoute(nameof(EquipmentDetailsPage), typeof(EquipmentDetailsPage));
            Routing.RegisterRoute(nameof(CreateOrderPage), typeof(CreateOrderPage));
        }
    }
}
