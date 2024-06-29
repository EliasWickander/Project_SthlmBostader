using HtmlAgilityPack;
using Project_SthlmBostader.Models;
using System.Globalization;
using System.Text.RegularExpressions;
using Project_SthlmBostader.ViewModels;

namespace Project_SthlmBostader.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ArticlesPage : ContentPage
    {
        public ArticlesPage()
        {
            InitializeComponent();
        }

        private void OnWebViewNavigated(object sender, WebNavigatedEventArgs e)
        {
            if (BindingContext is ArticlesViewModel viewModel)
            {
                viewModel.OnWebViewNavigated(sender, e);
            }
        }
    }
}