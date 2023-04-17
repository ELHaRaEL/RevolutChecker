using PuppeteerSharp.Input;
using PuppeteerSharp;

namespace RevolutChecker
{

    public class Revolut : IDisposable
    {
        public List<Prices> PricesData = new() { new Prices(false, DateTime.Now, RevolutCurriencies.ALL, string.Empty) }; // 


        private readonly SortedList<RevolutCurriencies, List<FinancialTransaction>> listOfAllFinancialTransactions = new();
        private readonly List<OrderedCashTransfer> listOfOrderedCashTransfer = new();

        private int countOfCurriencies = 0;
        private readonly string pathToChromeDir;
        private const string defaultAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/111.0";

        private PuppeteerSharp.Browser browser;
        private PuppeteerSharp.Page page;
        readonly Random random = new();

        //
        private bool waitingForTransfer = false;
        private int howOftenToCheckAccountsWhileWaitingForATransfer = 1; //min
        private int howOftenToCheckAccountsWhithoutWaitingForTransfer = 30; //min
        private string client_ID = string.Empty;
        private DateTime timeLastClickedMail = DateTime.MinValue;
        private DateTime timeLastCheckAccount = DateTime.MinValue;
        //
        public bool WaitingForTransfer { get { return waitingForTransfer; } set { waitingForTransfer = value; } }
        public int HowOftenToCheckAccountsWhithoutWaitingForTransfer { get { return howOftenToCheckAccountsWhithoutWaitingForTransfer; } set { howOftenToCheckAccountsWhithoutWaitingForTransfer = value; } }
        public int HowOftenToCheckAccountsWhileWaitingForATransfer { get { return howOftenToCheckAccountsWhileWaitingForATransfer; } set { howOftenToCheckAccountsWhileWaitingForATransfer = value; } }
        public string Client_ID { get { return client_ID; } set { client_ID = value; } }
        public DateTime TimeLastClickedMail { get { return timeLastClickedMail; } set { timeLastClickedMail = value; } }
        public DateTime TimeLastCheckAccount { get { return timeLastCheckAccount; } set { timeLastCheckAccount = value; } }

        public readonly TypeOptions[] TypeOptions = new TypeOptions[]
        {
            new TypeOptions()  { Delay = 35 },
            new TypeOptions()  { Delay = 300 },
        };

        private void ReLaunchBrowser(string pathToChromeDir, bool headless, string agent = defaultAgent)
        {
            Task<IBrowser> newBrowser = Puppeteer.LaunchAsync(new LaunchOptions
            {
                ExecutablePath = $"{pathToChromeDir}\\chrome.exe",
                Headless = headless,
                IgnoreHTTPSErrors = true,
                Args = new[]
                {
                    "--lang=en-gb"
                }
            });

            newBrowser.Wait();
            browser = (Browser)newBrowser.Result;
            browser.Disconnected += (sender, e) => { ReLaunchBrowser(pathToChromeDir, false); };
            Task<IPage> newPage = browser.NewPageAsync();
            newPage.Wait();
            page = (PuppeteerSharp.Page)newPage.Result;
            page.SetUserAgentAsync(agent).Wait();
        }


        public async Task<bool> ReLaunchPage(string agent = defaultAgent)
        {
            Task<IPage> newPage = browser.NewPageAsync();
            newPage.Wait();
            page = (PuppeteerSharp.Page)newPage.Result;
            await page.SetUserAgentAsync(agent);
            if (await page.GoToAsync("https://app.revolut.com/login", 120000) != null)
            {
                return true;
            }
            return false;
        }

        private int ReturnCountNoSuccesOrderedCashTransfer()
        {
            int count = 0;
            if (listOfOrderedCashTransfer.Count != 0)
            {
                foreach (OrderedCashTransfer orderedCashTransfer in listOfOrderedCashTransfer)
                {
                    if (orderedCashTransfer.TransferSucces)
                        count++;
                }
                return count;
            }
            else return 0;
        }
        static (string name, string surname, string nickname) SplitDescription(string description)
        {
            description = description.Remove(0, description.IndexOf("from") + 5);
            string name = description.Remove(description.IndexOf(" "), description.Length - description.IndexOf(" "));
            string surname = description.Remove(0, description.IndexOf(" ") + 1);
            string nickname = string.Empty;

            //***HERE*** need to check what revolutID description looks like  // and add in parsing CSV encoding      FinancialParser
            return (name, surname, nickname);
        }

        public void CheckIfTheTransferArrived()
        {
            foreach (OrderedCashTransfer orderedCashTransfer in listOfOrderedCashTransfer)
            {
                if (!orderedCashTransfer.TransferSucces)
                {
                    foreach (FinancialTransaction financialTransaction in listOfAllFinancialTransactions[orderedCashTransfer.RevolutCurriencies])
                    {
                        if(financialTransaction.Description != null)
                        {

                            (string name, string surname, string nickname) = SplitDescription(financialTransaction.Description);
                        
                        if (orderedCashTransfer.Name == name || orderedCashTransfer.Surname == surname || orderedCashTransfer.Nickname == nickname)
                        {
                            if (orderedCashTransfer.CashAmount == financialTransaction.Amount || (orderedCashTransfer.CashAmount >= (financialTransaction.Amount - financialTransaction.Amount * orderedCashTransfer.PossibleDevation) || orderedCashTransfer.CashAmount <= (financialTransaction.Amount + financialTransaction.Amount * orderedCashTransfer.PossibleDevation)))
                            {
                                orderedCashTransfer.SetOrderAsSuccess();
                                // ***HERE***
                                // something more? need to trace
                                // ***HERE***
                            }
                        }

                        }
                    }

                }
            }
            if (ReturnCountNoSuccesOrderedCashTransfer() == 0)
            {
                WaitingForTransfer = false;
            }

        }


        public List<OrderedCashTransfer> GetListOfOrdersCashTransfer()
        {
            return listOfOrderedCashTransfer;
        }

        public void OrderWaitingForNewTransfer(decimal cashAmount, string name, string surname, string nickname, RevolutCurriencies revolutCurriencies, bool firstTransfer = true, decimal possibleDeviation = 00)  // trzeba bedzie sprawdzic jak to dziala z revolutid i do poprawy
        {
            listOfOrderedCashTransfer.Add(new OrderedCashTransfer(cashAmount, name, surname, nickname, revolutCurriencies, firstTransfer, possibleDeviation));
            WaitingForTransfer = true;
        }

        public Revolut(string pathToChromeDir, bool headless = false, string agent = defaultAgent)
        {
            // IMPORTANT  !!
            //
            // After first use, you must manualy copy all files from    pathToChromeDir\WinXXXX\Chromium  to    pathToChromeDir
            //
            // IMPORTANT  !!
            this.pathToChromeDir = pathToChromeDir;
            agent = string.IsNullOrEmpty(agent) ? defaultAgent : agent;
            if (!File.Exists(pathToChromeDir + "\\chrome.exe"))
            {
                BrowserFetcher browserFetcher = new(new BrowserFetcherOptions
                {
                    Path = pathToChromeDir
                });

                browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision).Wait();
            }

            Task<IBrowser> newBrowser = Puppeteer.LaunchAsync(new LaunchOptions
            {
                ExecutablePath = $"{pathToChromeDir}\\chrome.exe",
                Headless = headless,  // false = visible , true = no visible
                IgnoreHTTPSErrors = true,
                //    Product = Product.Chrome,
                Args = new[]
                {
                    "--lang=en-gb"
                }
            });

            newBrowser.Wait();
            browser = (Browser)newBrowser.Result;
            browser.Disconnected += (sender, e) => { ReLaunchBrowser(pathToChromeDir, false); };

            Task<IPage> newPage = browser.NewPageAsync();
            newPage.Wait();
            page = (PuppeteerSharp.Page)newPage.Result;
            page.SetUserAgentAsync(agent).Wait();

            // ###   LOGS
            //page.Error += (s, e) => { Console.WriteLine("Error:" + e.ToString()); };
            //page.PageError += (s, e) => { Console.WriteLine("Error:" + e.ToString()); };
            //page.Console += (s, e) => { Console.WriteLine(e.Message.Text); };
            // ###
        }







        private async Task<RevolutPageStatus.PageStatus> CheckPageStatusSelector(string css, RevolutPageStatus.PageStatus pageStatus)
        {
            try
            {
                if (css.StartsWith("//") || css.StartsWith("/")) // css = xpath
                {
                    var elementHandles = await page.XPathAsync(css);
                    if (elementHandles.Length > 0)
                    {
                        return pageStatus;
                    }
                    else return RevolutPageStatus.PageStatus.Error;
                }

                else // css = selector  
                {
                    if (await page.QuerySelectorAsync(css) != null)
                        return pageStatus;
                    else return RevolutPageStatus.PageStatus.Error;
                }
            }
            catch (EvaluationFailedException) { return RevolutPageStatus.PageStatus.Error; }
            catch (MessageException) { return RevolutPageStatus.PageStatus.Error; }
        }
        public bool CheckPageBlank()
        {
            if (page.Url == "about:blank")
            {
                return true;
            }
            else return false;
        }

        internal async Task<RevolutPageStatus.PageStatus> CheckPageStatus()
        {
            for (int i = 0; i < RevolutPageStatus.XPathsPageStatus.Length; i++)
            {
                if (await CheckPageStatusSelector(RevolutPageStatus.XPathsPageStatus[i], (RevolutPageStatus.PageStatus)i) != RevolutPageStatus.PageStatus.Error)
                {
                    if ((RevolutPageStatus.PageStatus)i == RevolutPageStatus.PageStatus.LoginToRevolutMain)
                    {
                        if (await CheckPageStatusSelector(RevolutPageStatus.XPathsPageStatus[(int)RevolutPageStatus.PageStatus.VerifyByAppRevolut], RevolutPageStatus.PageStatus.VerifyByAppRevolut) != RevolutPageStatus.PageStatus.Error)
                        {
                            if (await CheckPageStatusSelector(RevolutPageStatus.XPathsPageStatus[(int)RevolutPageStatus.PageStatus.VerifyYourIdentityVia], RevolutPageStatus.PageStatus.VerifyYourIdentityVia) != RevolutPageStatus.PageStatus.Error)
                            {

                                return RevolutPageStatus.PageStatus.VerifyYourIdentityVia;
                            }
                            else return RevolutPageStatus.PageStatus.VerifyByAppRevolut;

                        }
                        if (await CheckPageStatusSelector(RevolutPageStatus.XPathsPageStatus[(int)RevolutPageStatus.PageStatus.CheckMail], RevolutPageStatus.PageStatus.CheckMail) != RevolutPageStatus.PageStatus.Error)
                        {
                            return RevolutPageStatus.PageStatus.CheckMail;
                        }
                        if (await CheckPageStatusSelector(RevolutPageStatus.XPathsPageStatus[(int)RevolutPageStatus.PageStatus.LoginToRevolutSpan], RevolutPageStatus.PageStatus.LoginToRevolutSpan) != RevolutPageStatus.PageStatus.Error)
                        {

                            return RevolutPageStatus.PageStatus.LoginToRevolutSpan;
                        }
                        if (await CheckPageStatusSelector(RevolutPageStatus.XPathsPageStatus[(int)RevolutPageStatus.PageStatus.LoginSelectAccount], RevolutPageStatus.PageStatus.LoginSelectAccount) != RevolutPageStatus.PageStatus.Error)
                        {
                            return RevolutPageStatus.PageStatus.LoginSelectAccount;
                        }

                    }
                    if ((RevolutPageStatus.PageStatus)i == RevolutPageStatus.PageStatus.VerifyByAppRevolut)
                    {
                        if (await CheckPageStatusSelector(RevolutPageStatus.XPathsPageStatus[(int)RevolutPageStatus.PageStatus.VerifyYourIdentityVia], RevolutPageStatus.PageStatus.VerifyYourIdentityVia) != RevolutPageStatus.PageStatus.Error)
                        {

                            return RevolutPageStatus.PageStatus.VerifyYourIdentityVia;
                        }
                        else return RevolutPageStatus.PageStatus.VerifyByAppRevolut;
                    }
                    return (RevolutPageStatus.PageStatus)i;
                }
            }
            return RevolutPageStatus.PageStatus.Error;
        }
        private void PrintFinancialTransactionsByCurriencies(RevolutCurriencies revolutCurriencies)
        {
            List<FinancialTransaction> listOfFinancialTransaction = listOfAllFinancialTransactions[revolutCurriencies];
            PrintHorizontalLine("Financial transaction list: " + revolutCurriencies.ToString());
            foreach (FinancialTransaction financialTransaction in listOfFinancialTransaction)
            {
                Console.WriteLine("Description: " + financialTransaction.Description);
                Console.WriteLine("{0,-10}{1,-11}{2,-5}{3,-10}{4,-8}{5,-21}{6,-21}{7,-11}{8,-10}", "Type", "Amount", "Cur", "Balance", "Fee", "Started Date", "Completed Date", "State", "Product");
                Console.WriteLine("{0,-10}{1,-11}{2,-5}{3,-10}{4,-8}{5,-21}{6,-21}{7,-11}{8,-10}", financialTransaction.Type, financialTransaction.Amount, financialTransaction.Currency, financialTransaction.Balance, financialTransaction.Fee, financialTransaction.StartedDate, financialTransaction.CompletedDate, financialTransaction.State, financialTransaction.Product);
                Console.WriteLine();
            }
        }

        private static void PrintHorizontalLine(string methodName = "", string lineString = "Print time: {0}", DateTime dateTime = default)
        {
            if (dateTime == default)
            {
                dateTime = DateTime.Now;
            }
            Console.WriteLine();
            for (int i = 0; i < 66; i++)
            {
                Console.Write("#");
            }
            Console.WriteLine();
            Console.WriteLine(lineString, dateTime);
            Console.WriteLine(methodName);
            Console.WriteLine();
        }

        public async Task<bool> CheckFileDownloadAndLoad(string dirPath, RevolutCurriencies revolutCurriencies)
        {
            try
            {
                string[] files = Directory.GetFiles(dirPath, "*.csv").OrderByDescending(f => new FileInfo(f).LastWriteTime).ToArray();
                if (files.Length == 0)
                {
                    return false;
                }
                string latestFile = files[0];
                string input = await File.ReadAllTextAsync(latestFile);
                List<FinancialTransaction> transactions = FinancialParser.ParseTransactions(input);
                if (listOfAllFinancialTransactions.ContainsKey(revolutCurriencies))
                {
                    listOfAllFinancialTransactions.SetValueAtIndex(listOfAllFinancialTransactions.IndexOfKey(revolutCurriencies), transactions);
                }
                else
                {
                    listOfAllFinancialTransactions.Add(revolutCurriencies, transactions);
                }
                PrintFinancialTransactionsByCurriencies(revolutCurriencies);
            }
            catch (FileLoadException) { return false; }
            catch (System.IO.IOException) { return false; }
            return true;
        }

        public async Task<bool> WaitAndClickItem(string css, int timeout = 3500)
        {
            WaitForSelectorOptions waitForSelectorOptions = new() { Timeout = timeout };

            try
            {
                if (css.StartsWith("//") || css.StartsWith("/"))
                {
                    // css = xPath 
                    _ = await page.WaitForXPathAsync(css, waitForSelectorOptions);
                    await Task.Delay(random.Next(300, 550));
                    string script = $"(document.evaluate(\"{css}\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue).click()";
                    _ = await page.EvaluateExpressionAsync(script);  // click by javascript
                    return true;
                }
                else
                {
                    // css = selector
                    _ = await page.WaitForSelectorAsync(css, waitForSelectorOptions);
                    _ = (ElementHandle)await page.QuerySelectorAsync(css);
                    await Task.Delay(random.Next(300, 550));
                    await page.ClickAsync(css);
                    return true;
                }
            }
            catch (EvaluationFailedException) { return false; }
            catch (WaitTaskTimeoutException) { return false; }
            catch (PuppeteerSharp.SelectorException) { return false; }
        }
        public async Task<bool> InsertText(string css, string text, TypeOptions? typeOptions = null)
        {
            typeOptions ??= TypeOptions[0];
            try
            {
                if (css.StartsWith("//") || css.StartsWith("/"))
                {
                    // css = xPath
                    string jsGetElementByXPath = $"document.evaluate(\"{css}\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.value = '{text}';";
                    await page.EvaluateExpressionAsync(jsGetElementByXPath);
                    await Task.Delay(random.Next(300, 550));
                    return true;
                }

                else
                {
                    // css = selector
                    await page.TypeAsync(css, text, typeOptions);
                    await Task.Delay(random.Next(300, 550));
                    return true;
                }
            }
            catch (EvaluationFailedException) { return false; }
            catch (SelectorException) { return false; }
        }

        public async Task<bool> WaitFor(string css, int timeout = 3500)
        {
            WaitForSelectorOptions waitForSelectorOptions = new() { Timeout = timeout };
            try
            {
                if (css.StartsWith("//") || css.StartsWith("/"))
                {
                    // css = xPath
                    await page.WaitForXPathAsync(css, waitForSelectorOptions);
                    await Task.Delay(random.Next(200, 300));
                    return true;
                }
                else
                {
                    // css = selector
                    await page.WaitForSelectorAsync(css, waitForSelectorOptions);
                    await Task.Delay(random.Next(200, 300));
                    return true;

                }
            }
            catch (WaitTaskTimeoutException) { return false; }
        }
        public async Task<bool> GoToAsync(string url, int timeout = 60000)
        {
            if (await page.GoToAsync(url, timeout) != null)
                return true;
            else return false;
        }

        public async Task<string> SetDownloadPathFile(RevolutCurriencies currencieToPath)
        {
            string? currencieFolder = Enum.GetName(typeof(RevolutCurriencies), currencieToPath);
            if (currencieFolder != null)
            {
                string finalPath = pathToChromeDir + "\\Downloads\\" + currencieFolder;
                await page.Client.SendAsync("Page.setDownloadBehavior", new { behavior = "allow", downloadPath = finalPath });
                return finalPath;
            }
            return string.Empty;
        }


        public async Task<bool> SetActualCurrencyAndTime()
        {
            SortedList<RevolutCurriencies, string> arrayOfCurrencies = await ReturnArrayOfCurrencies();
            if (PricesData[0].Price != arrayOfCurrencies[RevolutCurriencies.ALL])
            {
                PricesData[0].Price = arrayOfCurrencies[RevolutCurriencies.ALL];
                PricesData[0].LastChange = DateTime.Now;
            }

            if (PricesData.Count < arrayOfCurrencies.Count)
            {
                int constCount = PricesData.Count;
                for (int i = 0; i < arrayOfCurrencies.Count; i++)
                {
                    if (!PricesData.Any<Prices>(k => k.Curriencies == arrayOfCurrencies.GetKeyAtIndex(i)))
                    {
                        PricesData.Add(new Prices(true, DateTime.Now, arrayOfCurrencies.GetKeyAtIndex(i), arrayOfCurrencies.GetValueAtIndex(i)));
                    }
                }
            }

            for (int i = 1; i < arrayOfCurrencies.Count; i++)
            {
                if (PricesData[i].Price != arrayOfCurrencies.GetValueAtIndex(arrayOfCurrencies.IndexOfKey(PricesData[i].Curriencies)))
                {
                    PricesData[i].Price = arrayOfCurrencies.GetValueAtIndex(i);
                    PricesData[i].LastChange = DateTime.Now;
                    PricesData[i].Print = true;
                    PricesData[i].Curriencies = arrayOfCurrencies.GetKeyAtIndex(i);
                }
            }
            return true;
        }

        private async Task<int> ReturnCountOfAvaibleCurrencies()
        {
            int countOfCurriencies = 0;
            while (true)
            {
                if (await WaitFor("/html/body/div[5]/div/span/div[1]/div[5]/div[" + (countOfCurriencies + 1) + "]", 50))
                {
                    countOfCurriencies++;
                }
                else return countOfCurriencies;
            }
        }

        private async Task<SortedList<RevolutCurriencies, string>> ReturnArrayOfCurrencies()
        {
            SortedList<RevolutCurriencies, string> arrayOfCurrencies = new();
            try
            {
                if (countOfCurriencies == 0)
                {
                    countOfCurriencies = await ReturnCountOfAvaibleCurrencies();
                }
                string all = await page.EvaluateExpressionAsync<string>("document.evaluate('/html/body/div[5]/div/span/div[1]/div[3]/div/div/button/span[2]', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.textContent"); // price ALL        ANOTHER THAN OTHERS PRICES!!
                arrayOfCurrencies.Add(RevolutCurriencies.ALL, all);

                for (int i = 1; i < countOfCurriencies + 1; i++)
                {
                    string price = await page.EvaluateExpressionAsync<string>("document.evaluate('/html/body/div[5]/div/span/div[1]/div[5]/div[" + i + "]/button/span[2]', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.textContent"); // price 
                    string curriencie = await page.EvaluateExpressionAsync<string>("document.evaluate('/html/body/div[5]/div/span/div[1]/div[5]/div[" + i + "]/button/span/span[2]', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.textContent"); // curriencie
                    if (Enum.TryParse<RevolutCurriencies>(curriencie, out RevolutCurriencies result))
                    {
                        arrayOfCurrencies.Add(result, price);
                    }
                }
            }
            catch (EvaluationFailedException) { return arrayOfCurrencies; }
            return arrayOfCurrencies;
        }

        //public string ReturnAndSetNonceFromUrl()
        //{
        //    string parseUrl = page.Url.ToString();
        //    string nonce = parseUrl.Remove(0, parseUrl.IndexOf("nonce=") + 6).Trim();
        //    if (nonce.IndexOf("&") != -1)
        //    {
        //        nonce = nonce.Remove(nonce.IndexOf("&")).Trim();
        //    }
        //    Nonce = nonce;
        //    return nonce;
        //}

        public string ReturnAndSetClientIdFromUrl()
        {
            string client_id = string.Empty;
            if (page.Url != null)
            {

                string parseUrl = page.Url.ToString();
                if (parseUrl.IndexOf("client_id=") != -1)
                {
                    client_id = parseUrl.Remove(0, parseUrl.IndexOf("client_id=") + 10).Trim();
                    if (client_id.IndexOf("&") != -1)
                        client_id = client_id.Remove(client_id.IndexOf("&")).Trim();
                    Client_ID = client_id;
                }
            }
            return client_id;
        }



        void IDisposable.Dispose()
        {
            browser?.Dispose();
            page?.Dispose();
            GC.SuppressFinalize(this);
        }

        ~Revolut()
        {
            browser?.Dispose();
            page?.Dispose();
        }



    }

}


// ACCESS TO ELEMENT
//var elementHandles = await page.XPathAsync("//input[@name='q']");
//var elementHandle = elementHandles[0];
//await elementHandle.TypeAsync("Puppeteer Sharp");

// ACCESS TO ELEMENT BY JAVASCRIPT
//string xpath = "//input[@name='username']";
//JSHandle elementHandle = await page.EvaluateExpressionAsync<JSHandle>($"document.evaluate('{xpath}', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue");
//ElementHandle inputElement = elementHandle.GetObject<ElementHandle>();