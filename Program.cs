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

            Console.Write("What game are you looking for? ");
            string gameToSearch = Console.ReadLine();
            gameToSearch = gameToSearch.Trim().ToLower().Replace(" ", "+");

            try
            {
                await page.GoToAsync($"https://gg.deals/search/?title={gameToSearch}", timeout: 0);
            }
            catch (NavigationException ex)
            {
                Console.WriteLine(ex.Message);
            }

            await GetGameList(page);

            await GetStorePrices(page);

            //await browser.CloseAsync();

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

            for(int i = 0; i < gameSelection.Count; i++)
            {
                Console.WriteLine($"{i+1}. {gameSelection[i].Name}");
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
            /*var showMoreButton = await page.QuerySelectorAsync(".btn-game-see-more");
            if (showMoreButton != null)
            {
                await showMoreButton.ClickAsync();
                await showMoreButton.ClickAsync();
                Thread.Sleep(2000);
            }*/
            
            var showMoreButton = await page.QuerySelectorAllAsync(".btn-game-see-more");

            foreach (var button in showMoreButton)
            {
                await button.ClickAsync();
                await button.ClickAsync();
                Thread.Sleep(2000);
            }


            var games = await page.QuerySelectorAllAsync(".game-list-item");

            Console.WriteLine(games.Length);

            int index = 1;

            foreach (var game in games)
            {
                var type = await game.EvaluateFunctionAsync<string>("game => game.getAttribute('data-shop-name')");
                //excludes stuff like dlc, packs, etc.
                if (!string.IsNullOrWhiteSpace(type))
                {
                    var priceElement = await game.QuerySelectorAsync(".price-inner");

                    priceElement ??= await game.QuerySelectorAsync(".price-text");

                    string price = await (await priceElement.GetPropertyAsync("textContent")).JsonValueAsync<string>();

                    Console.WriteLine($"Store: {type} {index}\nPrice: {price}");
                    //index++;
                    continue;
                }
                //index++;
            }
        }
    }
}
