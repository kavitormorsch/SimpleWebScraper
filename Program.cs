using HtmlAgilityPack;
using PuppeteerSharp;
using System;
using System.Threading.Tasks;



namespace SimpleWebScraper
{
    internal class Program
    {
        public class Product
        {
            public string? Url { get; set; }
            public string? Image { get; set; }
            public string? Name { get; set; }
            public string? Price { get; set; }

        }

        static async Task Main(string[] args)
        {
            //await new BrowserFetcher().DownloadAsync();

            var browser = await Puppeteer.LaunchAsync(new LaunchOptions() {
                DefaultViewport = new ViewPortOptions() { Width = 1280 },
                HeadlessMode = 0,
            });

            var page = await browser.NewPageAsync();
            await page.SetUserAgentAsync("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36");
            await page.GoToAsync("https://gg.deals");

            await ChangeLanguage(page);
       
            try
            {
                await page.GoToAsync("https://gg.deals/search/?title=Raidou");
            }
            catch (NavigationException ex)
            {
                Console.WriteLine(ex.Message);
            }

            //await browser.CloseAsync();

            //var resultList = await page.


            /*
            var web = new HtmlWeb();

            var document = web.LoadFromBrowser();

            
            string ggDealsBaseLink = "https://gg.deals";

            string searchWord = "Raidou";

            var document = web.Load(ggDealsBaseLink + "/search/?title=" + searchWord);

            var resultList = document.DocumentNode.QuerySelectorAll(".game-item");

            var selectedGame = new Product();

            foreach (var result in resultList)
            {
                var gameTitle = HtmlEntity.DeEntitize(result.QuerySelector(".game-info-wrapper").QuerySelector(".game-info-column").QuerySelector(".game-info-title-wrapper").QuerySelector(".game-info-title").QuerySelector("a").InnerText);
                Console.WriteLine(gameTitle);
                var url = HtmlEntity.DeEntitize(result.QuerySelector("a").Attributes["href"].Value);

                selectedGame.Name = gameTitle;
                selectedGame.Url = "/game/raidou-remastered-the-mystery-of-the-soulless-army/";
            }

            document = web.Load(ggDealsBaseLink + selectedGame.Url);

            Console.WriteLine(ggDealsBaseLink + selectedGame.Url);

            var storelist = document.DocumentNode.QuerySelectorAll(".similar-deals-container");

            foreach (var store in storelist)
            {
                Console.WriteLine(store.QuerySelector(".game-item").Attributes[3].Value);
                Console.WriteLine(store.QuerySelector(".price-inner").InnerText);
            }

            
            var products = new List<Product>();

            var firstPageToScrape = "https://www.scrapingcourse.com/ecommerce/page/1";

            var pagesDiscovered = new List<string>() {firstPageToScrape};

            var pagesToScrape = new Queue<string>();

            pagesToScrape.Enqueue(firstPageToScrape);

            int i = 1;

            int limit = 12;

            while (pagesToScrape.Count != 0 && i < limit)
            {
                var currentPage = pagesToScrape.Dequeue();

                var currentDocument = web.Load(currentPage);

                var paginationHTMLElements = currentDocument.DocumentNode.QuerySelectorAll("a.page-numbers");

                foreach (var paginationHTMLElement in paginationHTMLElements)
                {
                    var newPaginationLink = paginationHTMLElement.Attributes["href"].Value;

                    if (!pagesDiscovered.Contains(newPaginationLink))
                    {
                        if (!pagesToScrape.Contains(newPaginationLink))
                        {
                            pagesToScrape.Enqueue(newPaginationLink);
                        }
                        pagesDiscovered.Add(newPaginationLink);
                    }
                }

                var productHTMLElements = currentDocument.DocumentNode.QuerySelectorAll("li.product");

                foreach (var productElement in productHTMLElements)
                {
                    var url = HtmlEntity.DeEntitize(productElement.QuerySelector("a").Attributes["href"].Value);
                    var image = HtmlEntity.DeEntitize(productElement.QuerySelector("img").Attributes["src"].Value);
                    var name = HtmlEntity.DeEntitize(productElement.QuerySelector("h2").InnerText);
                    var price = HtmlEntity.DeEntitize(productElement.QuerySelector(".price").InnerText);
                    var product = new Product() { Url = url, Image = image, Name = name, Price = price };

                    products.Add(product);
                }
                i++;
            }

            foreach (Product item in products)
            {
                Console.WriteLine($"{item.Name}" +
                    $"\n{item.Price}" +
                    $"\n{item.Image}" +
                    $"\n{item.Url}" +
                    $"\n======================");

            }

            Console.WriteLine("Hello, World!");
            */


        }

        static async Task ChangeLanguage(IPage page)
        {
            var regionList = await page.QuerySelectorAsync("#settings-menu-region");
            var regionButton = await regionList.QuerySelectorAsync(".settings-menu-select-action");

            await regionButton.ClickAsync();

            var languageList = await regionList.QuerySelectorAllAsync(".settings-menu-select-option");

            foreach (var language in languageList)
            {
                var languageButton = await language.QuerySelectorAsync(".region-change-link");

                string languageText = await (await languageButton.GetPropertyAsync("textContent")).JsonValueAsync<string>();

                Console.WriteLine(languageText);

                if (languageText.Trim() == "United States")
                {
                    await languageButton.ClickAsync();
                }

            }
        }
    }
}
