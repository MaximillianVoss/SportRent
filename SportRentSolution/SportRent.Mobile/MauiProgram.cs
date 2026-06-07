using Microsoft.Extensions.Logging;
using SportRent.Mobile.Services;
using SportRent.Mobile.ViewModels;

namespace SportRent.Mobile
{
    /// <summary>
    /// Точка настройки MAUI-приложения, сервисов и ViewModel.
    /// </summary>
    public static class MauiProgram
    {
        public static IServiceProvider Services { get; private set; } = null!;

        /// <summary>
        /// Создает MAUI-приложение и регистрирует зависимости для экранов.
        /// </summary>
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Сервисы данных живут как Singleton, потому что используют одну локальную базу и одну сессию пользователя.
            builder.Services.AddSingleton<ILocalDatabaseService, LocalDatabaseService>();
            builder.Services.AddSingleton<IUserSessionService, UserSessionService>();
            builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
            builder.Services.AddSingleton<ISportRentCatalogService, SportRentCatalogService>();
            builder.Services.AddSingleton<IOrdersService, OrdersService>();
            // ViewModel создаются заново для страниц, чтобы состояние формы не протекало между экранами.
            builder.Services.AddTransient<LoginPageViewModel>();
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<EquipmentDetailsPageViewModel>();
            builder.Services.AddTransient<CreateOrderPageViewModel>();
            builder.Services.AddTransient<OrdersPageViewModel>();
            builder.Services.AddTransient<ProfilePageViewModel>();

            MauiApp app = builder.Build();
            Services = app.Services;
            return app;
        }
    }
}
