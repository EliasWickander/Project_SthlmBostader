using Project_SthlmBostader.Services;
using Project_SthlmBostader.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Project_SthlmBostader
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
