using Microsoft.Extensions.DependencyInjection;
using SportRent.Mobile.Models;
using SportRent.Mobile.ViewModels;

namespace SportRent.Mobile
{
    public partial class MainPage : ContentPage
    {
        private bool _hasInitialized;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = MauiProgram.Services.GetRequiredService<MainPageViewModel>();
        }

        private MainPageViewModel ViewModel => (MainPageViewModel)BindingContext;

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

        private async void OnEquipmentTapped(object? sender, TappedEventArgs e)
        {
            if ((sender as BindableObject)?.BindingContext is CatalogEquipmentItem item)
            {
                await ViewModel.OpenEquipmentAsync(item);
            }
        }

        private void OnCategoryChipClicked(object? sender, EventArgs e)
        {
            if ((sender as BindableObject)?.BindingContext is CategoryChipViewModel chip)
            {
                ViewModel.SelectCategory(chip);
            }
        }
    }
}
