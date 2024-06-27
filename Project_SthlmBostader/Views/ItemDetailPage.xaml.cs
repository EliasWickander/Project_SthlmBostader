using Project_SthlmBostader.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace Project_SthlmBostader.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}