using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Input;
using HtmlAgilityPack;
using Project_SthlmBostader.Models;

namespace Project_SthlmBostader.ViewModels
{
    public class ArticlesViewModel : BindableObject
    {
        private string m_webViewSource = string.Empty;

        public string WebViewSource
        {
            get
            {
                return m_webViewSource;
            }
            set
            {
                if (m_webViewSource != value)
                {
                    m_webViewSource = value;
                    OnPropertyChanged(nameof(WebViewSource));
                }
            }
        }

        public ArticlesViewModel()
        {
            WebViewSource = "https://bostad.stockholm.se/bostad?s=57.66891&n=60.99975&w=15.57861&e=19.35791&hide-filter=true&sort=annonserad-fran-desc&mobilvy=lista";
        }
        
        public async void OnWebViewNavigated(object sender, WebNavigatedEventArgs e)
        {
            if(e.Result == WebNavigationResult.Success)
            {
                List<ArticleData> articles = await DownloadArticles(sender as WebView);
            }  
        }
        
        private async Task<List<ArticleData>> DownloadArticles(WebView webView)
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