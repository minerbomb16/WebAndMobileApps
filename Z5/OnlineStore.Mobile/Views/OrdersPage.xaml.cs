using OnlineStore.Mobile.Services;
using OnlineStore.Domain.Models;
using System.Collections.ObjectModel;

namespace OnlineStore.Mobile
{
    public partial class OrdersPage : ContentPage
    {
        private readonly ApiService _apiService;
        public ObservableCollection<Order> Orders { get; set; }

        public OrdersPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            Orders = new ObservableCollection<Order>();
            BindingContext = this;
            LoadOrders();
        }

        public async void LoadOrders()
        {
            try
            {
                var orders = await _apiService.GetDataAsync<Order>("api/mobile/orders");
                if (orders == null || orders.Count == 0)
                {
                    Console.WriteLine("No orders found");
                    return;
                }

                Orders.Clear();
                foreach (var order in orders)
                {
                    Orders.Add(order);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading orders: {ex.Message}");
            }
        }
    }
}
