namespace OnlineStore.Mobile
{
    public partial class AppShell : Shell
    {
        public Command RefreshCommand { get; }
        public Command NavigateToCategoriesCommand { get; }
        public Command NavigateToProductsCommand { get; }
        public Command NavigateToOrdersCommand { get; }

        private ProductsPage _productsPage;
        private OrdersPage _ordersPage;
        private CategoriesPage _categoriesPage;

        public AppShell(ProductsPage productsPage, OrdersPage ordersPage, CategoriesPage categoriesPage)
        {
            InitializeComponent();

            _productsPage = productsPage;
            _ordersPage = ordersPage;
            _categoriesPage = categoriesPage;

            RefreshCommand = new Command(ExecuteRefresh);

            NavigateToCategoriesCommand = new Command(() =>
            {
                _categoriesPage?.LoadCategories();
                NavigateToPage(_categoriesPage);
            });

            NavigateToProductsCommand = new Command(() =>
            {
                _productsPage?.LoadProducts();
                NavigateToPage(_productsPage);
            });

            NavigateToOrdersCommand = new Command(() =>
            {
                _ordersPage?.LoadOrders();
                NavigateToPage(_ordersPage);
            });

            BindingContext = this;
        }

        private void ExecuteRefresh()
        {
            _productsPage?.LoadProducts();
            _ordersPage?.LoadOrders();
            _categoriesPage?.LoadCategories();
        }

        private async void NavigateToPage(ContentPage page)
        {
            if (page != null)
            {
                await Navigation.PushAsync(page);
            }
        }
    }
}
