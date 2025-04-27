using PuppeteerSharp;
using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerExtraSharp.Plugins.AnonymizeUa;
using PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions;
using System.ComponentModel.DataAnnotations;
using SimpleWebScraper.Classes;

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

            string mainLink = "https://gg.deals";

            var browser = await Puppeteer.LaunchAsync(new LaunchOptions()
            {
                DefaultViewport = new ViewPortOptions() { Width = 1280 },
                Headless = false
            });

            var page = await browser.NewPageAsync();
            await page.SetUserAgentAsync("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36");
            await page.GoToAsync(mainLink,timeout: 0);

            await ChangeLanguage(page);       

            try
            {
                await page.GoToAsync("https://gg.deals/search/?title=Raidou", timeout: 0);
            }
            catch (NavigationException ex)
            {
                Console.WriteLine(ex.Message);
            }

            await GetGameList(page);

            await GetStorePrices(page);

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

                if (languageText.Trim() == "United States")
                {
                    Console.WriteLine("Language set to US.");
                    await languageButton.ClickAsync();
                    //wait for language change before swapping pages, otherwise net::ERR_ABORT will occur
                    Thread.Sleep(1500);
                }

            }
        }

        static async Task GetGameList(IPage page)
        {
            var games = await page.QuerySelectorAllAsync(".game-item");

            List<Game> gameSelection = new();

            foreach (var game in games)
            {
                var gameTitleElement = await game.QuerySelectorAsync(".title-inner");

                string gameTitle = await (await gameTitleElement.GetPropertyAsync("textContent")).JsonValueAsync<string>();

                var link = await game.QuerySelectorAsync(".full-link");

                string linkText = await (await link.GetPropertyAsync("href")).JsonValueAsync<string>();

                gameSelection.Add(new Game(gameTitle, linkText));
            }

            foreach (var game in gameSelection)
            {
                Console.WriteLine(game.Name);
            }

            int index = 0;

            while (true)
            {
                Console.Write("Pick an Option: ");
                string userInput = Console.ReadLine();

                if (!Int32.TryParse(userInput, out index))
                {
                    Console.WriteLine("Wrong input");
                }
                if (index > gameSelection.Count)
                {
                    Console.WriteLine("Invalid Index");
                }
                break;
            }

            await page.GoToAsync(gameSelection[index-1].Link);

        }

        static async Task GetStorePrices(IPage page)
        {
            var showMoreButton = await page.QuerySelectorAsync(".btn-game-see-more");
            if (showMoreButton != null)
            {
                await showMoreButton.ClickAsync();
                await showMoreButton.ClickAsync();
                Thread.Sleep(2000);
            }
            
            

            var games = await page.QuerySelectorAllAsync(".game-list-item");

            Console.WriteLine(games.Length);

            foreach (var game in games)
            {
                var type = await game.EvaluateFunctionAsync<string>("game => game.getAttribute('data-shop-name')");
                //excludes stuff like dlc, packs, etc.
                if (!string.IsNullOrWhiteSpace(type))
                {
                    var priceElement = await game.QuerySelectorAsync(".price-inner");

                    string price = await (await priceElement.GetPropertyAsync("textContent")).JsonValueAsync<string>();

                    Console.WriteLine($"Store: {type}\nPrice: {price}");
                }
            }
        }
    }
}
