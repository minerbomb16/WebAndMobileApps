using OnlineStore.Mobile.Services;
using OnlineStore.Domain.Models;
using System.Collections.ObjectModel;

namespace OnlineStore.Mobile
{
    public partial class ProductsPage : ContentPage
    {
        private readonly ApiService _apiService;
        public ObservableCollection<Product> Products { get; set; }

        public ProductsPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            Products = new ObservableCollection<Product>();
            BindingContext = this;
            LoadProducts();
        }

        public async void LoadProducts()
        {
            try
            {
                Console.WriteLine("Starting to load products...");
                var products = await _apiService.GetDataAsync<Product>("api/mobile/products");

                if (products != null && products.Count > 0)
                {
                    Console.WriteLine($"Successfully fetched {products.Count} products from the API.");
                }
                else
                {
                    Console.WriteLine("No products were returned by the API.");
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    Products.Clear();
                    foreach (var product in products)
                    {
                        Products.Add(product);
                    }
                    Console.WriteLine($"Successfully updated UI with {Products.Count} products.");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading products: {ex.Message}");
            }
        }

    }
}