using System.Collections.ObjectModel;
using System.Windows.Input;
using SportRent.Mobile.Models;
using SportRent.Mobile.Pages;
using SportRent.Mobile.Services;

namespace SportRent.Mobile.ViewModels;

public sealed class MainPageViewModel : ViewModelBase
{
    private readonly ISportRentCatalogService _catalogService;
    private readonly List<CatalogEquipmentItem> _allEquipment = [];

    private bool _isBusy;
    private string _searchText = string.Empty;
    private string _resultsSummary = "Загрузка каталога...";
    private string _overviewText = "Подбираем доступный инвентарь по пунктам проката.";
    private string? _errorMessage;
    private int _totalEquipmentMetric;
    private int _totalRentalPointsMetric;
    private int _totalCategoriesMetric;

    public MainPageViewModel(ISportRentCatalogService catalogService)
    {
        _catalogService = catalogService;
        Categories = [];
        VisibleEquipment = [];
        SelectCategoryCommand = new Command<CategoryChipViewModel>(SelectCategory);
        RefreshCommand = new Command(async () => await InitializeAsync(forceRefresh: true));
    }

    public ObservableCollection<CategoryChipViewModel> Categories { get; }

    public ObservableCollection<CatalogEquipmentItem> VisibleEquipment { get; }

    public ICommand SelectCategoryCommand { get; }

    public ICommand RefreshCommand { get; }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (!SetProperty(ref _searchText, value))
            {
                return;
            }

            ApplyFilters();
        }
    }

    public string ResultsSummary
    {
        get => _resultsSummary;
        private set => SetProperty(ref _resultsSummary, value);
    }

    public string OverviewText
    {
        get => _overviewText;
        private set => SetProperty(ref _overviewText, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            if (SetProperty(ref _errorMessage, value))
            {
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public int TotalEquipmentMetric
    {
        get => _totalEquipmentMetric;
        private set => SetProperty(ref _totalEquipmentMetric, value);
    }

    public int TotalRentalPointsMetric
    {
        get => _totalRentalPointsMetric;
        private set => SetProperty(ref _totalRentalPointsMetric, value);
    }

    public int TotalCategoriesMetric
    {
        get => _totalCategoriesMetric;
        private set => SetProperty(ref _totalCategoriesMetric, value);
    }

    public async Task InitializeAsync(bool forceRefresh = false, CancellationToken cancellationToken = default)
    {
        if (IsBusy && !forceRefresh)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            CatalogSnapshot snapshot = await _catalogService.GetCatalogAsync(cancellationToken);

            _allEquipment.Clear();
            _allEquipment.AddRange(snapshot.Equipment);

            Categories.Clear();
            Categories.Add(new CategoryChipViewModel
            {
                CategoryId = null,
                Title = "Все",
                IsSelected = true
            });

            foreach (CatalogCategory category in snapshot.Categories)
            {
                Categories.Add(new CategoryChipViewModel
                {
                    CategoryId = category.Id,
                    Title = category.Title,
                    IsSelected = false
                });
            }

            TotalEquipmentMetric = snapshot.Stats.TotalEquipment;
            TotalRentalPointsMetric = snapshot.Stats.TotalRentalPoints;
            TotalCategoriesMetric = snapshot.Stats.TotalCategories;
            OverviewText = $"{snapshot.Stats.TotalEquipment} позиций доступны в {snapshot.Stats.TotalRentalPoints} пунктах проката.";

            ApplyFilters();
        }
        catch (Exception ex)
        {
            VisibleEquipment.Clear();
            ResultsSummary = "Каталог недоступен";
            OverviewText = "Не удалось загрузить локальную базу данных.";
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task OpenEquipmentAsync(CatalogEquipmentItem item)
    {
        if (item is null)
        {
            return;
        }

        await Shell.Current.GoToAsync(nameof(EquipmentDetailsPage), true, new Dictionary<string, object>
        {
            ["equipmentId"] = item.Id
        });
    }

    public void SelectCategory(CategoryChipViewModel? chip)
    {
        if (chip is null)
        {
            return;
        }

        foreach (CategoryChipViewModel category in Categories)
        {
            category.IsSelected = ReferenceEquals(category, chip);
        }

        ApplyFilters();
    }

    private void ApplyFilters()
    {
        IEnumerable<CatalogEquipmentItem> query = _allEquipment;
        int? selectedCategoryId = Categories.FirstOrDefault(c => c.IsSelected)?.CategoryId;

        if (selectedCategoryId.HasValue)
        {
            query = query.Where(item => item.CategoryId == selectedCategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            string search = SearchText.Trim();
            query = query.Where(item =>
                item.Title.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                item.CategoryTitle.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                item.BrandModelText.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                item.TypeTitle.Contains(search, StringComparison.CurrentCultureIgnoreCase));
        }

        List<CatalogEquipmentItem> filtered = query.ToList();

        VisibleEquipment.Clear();
        foreach (CatalogEquipmentItem item in filtered)
        {
            VisibleEquipment.Add(item);
        }

        ResultsSummary = filtered.Count switch
        {
            0 => "Ничего не найдено",
            1 => "1 позиция в каталоге",
            _ => $"{filtered.Count} позиций в каталоге"
        };
    }
}
