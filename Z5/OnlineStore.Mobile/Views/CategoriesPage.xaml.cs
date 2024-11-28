using OnlineStore.Mobile.Services;
using OnlineStore.Domain.Models;
using System.Collections.ObjectModel;

namespace OnlineStore.Mobile
{
    public partial class CategoriesPage : ContentPage
    {
        private readonly ApiService _apiService;
        public ObservableCollection<Category> Categories { get; set; }

        public CategoriesPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            Categories = new ObservableCollection<Category>();
            BindingContext = this;
            LoadCategories();
        }

        public async void LoadCategories()
        {
            try
            {
                var categories = await _apiService.GetDataAsync<Category>("api/mobile/categories");
                if (categories == null || categories.Count == 0)
                {
                    Console.WriteLine("No categories found");
                    return;
                }

                Categories.Clear();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading categories: {ex.Message}");
            }
        }
    }
}
