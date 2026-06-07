using Microsoft.Extensions.DependencyInjection;
using SportRent.Mobile.Models;
using SportRent.Mobile.ViewModels;

namespace SportRent.Mobile
{
    /// <summary>
    /// Code-behind главного экрана каталога: связывает XAML-события с MainPageViewModel.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        private bool _hasInitialized;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = MauiProgram.Services.GetRequiredService<MainPageViewModel>();
        }

        private MainPageViewModel ViewModel => (MainPageViewModel)BindingContext;

        /// <summary>
        /// При первом показе загружает каталог из локальной базы.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_hasInitialized)
            {
                return;
            }

            _hasInitialized = true;
            await ViewModel.InitializeAsync();
        }

        /// <summary>
        /// Обрабатывает нажатие на карточку инвентаря в каталоге.
        /// </summary>
        private async void OnEquipmentTapped(object? sender, TappedEventArgs e)
        {
            if ((sender as BindableObject)?.BindingContext is CatalogEquipmentItem item)
            {
                await ViewModel.OpenEquipmentAsync(item);
            }
        }

        /// <summary>
        /// Обрабатывает выбор категории в горизонтальном списке фильтров.
        /// </summary>
        private void OnCategoryChipClicked(object? sender, EventArgs e)
        {
            if ((sender as BindableObject)?.BindingContext is CategoryChipViewModel chip)
            {
                ViewModel.SelectCategory(chip);
            }
        }
    }
}
