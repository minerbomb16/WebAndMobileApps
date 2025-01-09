using Microsoft.Maui.Controls;
using System;
using TodoListSolution.Mobile.ViewModels;
using TodoListSolution.Mobile.Models;

namespace TodoListSolution.Mobile.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }

        private void OnRefreshClicked(object sender, EventArgs e)
        {
            if (BindingContext is MainViewModel vm)
            {
                vm.ReloadData();
            }
        }

        private void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var item = (TodoItemDTO)checkBox.BindingContext;

            if (e.Value && BindingContext is MainViewModel vm)
            {
                vm.MarkDoneCommand?.Execute(item);
            }
        }

        private void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var item = (TodoItemDTO)button.BindingContext;

            if (BindingContext is MainViewModel vm)
            {
                vm.DeleteCommand?.Execute(item);
            }
        }
    }
}
