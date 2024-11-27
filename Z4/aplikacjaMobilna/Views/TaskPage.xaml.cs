using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using aplikacjaMobilna.ViewModels;

namespace aplikacjaMobilna.Views
{
    public partial class TasksPage : ContentPage
    {
        public TasksPage(TasksViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
