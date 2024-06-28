using HtmlAgilityPack;
using Project_SthlmBostader.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Project_SthlmBostader.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ArticlesPage : ContentPage
    {
        public ArticlesPage()
        {
            InitializeComponent();

            webView.Source = "https://bostad.stockholm.se/bostad?s=57.66891&n=60.99975&w=15.57861&e=19.35791&hide-filter=true&sort=annonserad-fran-desc&mobilvy=lista";

            webView.Navigated += OnWebViewNavigated;
        }

        private async void OnWebViewNavigated(object sender, WebNavigatedEventArgs e)
        {
            if(e.Result == WebNavigationResult.Success)
            {
                List<ArticleData> articles = await DownloadArticles();
            }
        }

        private async Task<List<ArticleData>> DownloadArticles()
        {
            string htmlContent = await webView.EvaluateJavaScriptAsync("document.documentElement.outerHTML");

            //Decode encoded html caused by javascript evaluation
            htmlContent = Regex.Replace(htmlContent, @"\\[Uu]([0-9A-Fa-f]{4})", m => char.ToString((char)ushort.Parse(m.Groups[1].Value, NumberStyles.AllowHexSpecifier)));
            htmlContent = Regex.Unescape(htmlContent);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            var allArticlesHtmlNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'ad-list__article')]");

            if (allArticlesHtmlNodes == null)
                return null;

            List<ArticleData> articles = new List<ArticleData>(allArticlesHtmlNodes.Count);

            for (int i = 0; i < allArticlesHtmlNodes.Count; i++)
            {
                HtmlNode articleHtmlNode = allArticlesHtmlNodes[i];

                ArticleData article = ParseHtmlToArticle(articleHtmlNode);

                articles.Add(article);
            }

            return articles;
        }

        private ArticleData ParseHtmlToArticle(HtmlNode htmlNode)
        {
            ArticleData articleData = new ArticleData();

            foreach (var descendant in htmlNode.Descendants())
            {
                if (descendant.HasClass("ad-list__link"))
                    articleData.m_link = descendant.GetAttributeValue("href", string.Empty);

                if (descendant.HasClass("apartment-listing__item__area"))
                    articleData.m_area = descendant.InnerText;

                if (descendant.HasClass("ad-list__title"))
                    articleData.m_street = descendant.InnerText;

                if (descendant.HasClass("ad-list__data"))
                {
                    string[] splitData = descendant.InnerText.Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries);

                    if (splitData.Length >= 0)
                        articleData.m_rent = GetNumberFromString(splitData[0]);

                    if (splitData.Length >= 1)
                        articleData.m_floor = GetNumberFromString(splitData[1]);

                    if (splitData.Length >= 2)
                        articleData.m_rooms = GetNumberFromString(splitData[2]);

                    if (splitData.Length >= 3)
                        articleData.m_size = GetNumberFromString(splitData[3]);
                }
            }

            return articleData;
        }

        private int GetNumberFromString(string str)
        {
            int result = -1;

            string numbers = string.Concat(str.Where(char.IsNumber));

            if (int.TryParse(numbers, out int numberAsInt))
                result = numberAsInt;

            return result;
        }
    }
}