using Microsoft.Maui.Controls;
using System;
using TodoListSolution.Mobile.ViewModels;
using TodoListSolution.Mobile.Models;

namespace TodoListSolution.Mobile.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        // Inject MainViewModel through constructor
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        private void OnRefreshClicked(object sender, EventArgs e)
        {
            if (BindingContext is MainViewModel vm)
            {
                vm.LoadData();
            }
        }
    }
}
