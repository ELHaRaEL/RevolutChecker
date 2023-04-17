using System.Text.Json;

namespace RevolutChecker
{
    public class Manager
    {
        public Manager()
        {
            string revolutJson = File.ReadAllText("Revolut.json");
            RevolutArgs? _1 = JsonSerializer.Deserialize<RevolutArgs>(revolutJson);
            if (_1 != null)
            {
                revolutService = new Revolut(_1.PathToChromeDir, _1.Headless, _1.UserAgent);
                inputEmail = _1.InputEmail;
                inputPin1 = _1.InputPin1;
                inputPin2 = _1.InputPin2;
                inputPin3 = _1.InputPin3;
                inputPin4 = _1.InputPin4;
            }
            else
            {
                revolutService = new Revolut(string.Empty, true, string.Empty);
            }

            string gmailJson = File.ReadAllText("Gmail.json");
            GmailArgs? _2 = JsonSerializer.Deserialize<GmailArgs>(gmailJson);
            if (_2 != null)
            {
                gmailService = new Gmail(_2.ApplicationName, _2.MailAddress, _2.PathToClientSecretJson);
            }
            else
            {
                gmailService = new Gmail(string.Empty, string.Empty, string.Empty);
            }
            _1 = null;
            _2 = null;
        }


        public class RevolutArgs
        {
            private string pathToChromeDir = string.Empty;
            private bool headless = true;  // false = visible GUI , true = no visible GUI
            private string userAgent = string.Empty;
            private string inputEmail = "your-email@gmail.com";
            private string inputPin1 = "0";
            private string inputPin2 = "0";
            private string inputPin3 = "0";
            private string inputPin4 = "0";

            public string PathToChromeDir { get { return pathToChromeDir; } set { pathToChromeDir = value; } }

            public bool Headless { get { return headless; } set { headless = value; } }
            public string UserAgent { get { return userAgent; } set { userAgent = value; } }

            public string InputEmail { get { return inputEmail; } set { inputEmail = value; } }
            public string InputPin1 { get { return inputPin1; } set { inputPin1 = value; } }
            public string InputPin2 { get { return inputPin2; } set { inputPin2 = value; } }
            public string InputPin3 { get { return inputPin3; } set { inputPin3 = value; } }
            public string InputPin4 { get { return inputPin4; } set { inputPin4 = value; } }
        }

        private class GmailArgs
        {
            private string applicationName = string.Empty;
            private string mailAddress = string.Empty;
            private string pathToClientSecretJson = string.Empty;
            public string ApplicationName { get { return applicationName; } set { applicationName = value; } }
            public string MailAddress { get { return mailAddress; } set { mailAddress = value; } }
            public string PathToClientSecretJson { get { return pathToClientSecretJson; } set { pathToClientSecretJson = value; } }
        }


        private readonly string inputEmail = string.Empty;
        private readonly string inputPin1 = string.Empty;
        private readonly string inputPin2 = string.Empty;
        private readonly string inputPin3 = string.Empty;
        private readonly string inputPin4 = string.Empty;


        private readonly Gmail gmailService;
        private readonly Revolut revolutService;


        private RevolutItems.Items menuPrintFinanceStartDateMonthButton = RevolutItems.Items.MenuPrintFinanceStartDateJanuaryButton;
        internal RevolutItems.Items MenuPrintFinanceStartDateMonthButton { get { return menuPrintFinanceStartDateMonthButton; } set { menuPrintFinanceStartDateMonthButton = value; } }

        public void OrderWaitingForNewTransfer(decimal cashAmount, string name, string surname, string nickname, RevolutCurriencies revolutCurriencies, bool firstTransfer = true, decimal possibleDeviation = 0)
        {
            revolutService.OrderWaitingForNewTransfer(cashAmount, name, surname, nickname, revolutCurriencies, firstTransfer, possibleDeviation);
        }

        public List<OrderedCashTransfer> GetListOfOrdersCashTransfer()
        {
            return revolutService.GetListOfOrdersCashTransfer();
        }

        private async Task<bool> WhatToDo()
        {
            if (revolutService.CheckPageBlank() == true)
            {
                if (await revolutService.GoToAsync("https://app.revolut.com/login", 120000) == true)
                    return true;
                else return false;
            }
            RevolutPageStatus.PageStatus pageStatus = await revolutService.CheckPageStatus();

            if (pageStatus == RevolutPageStatus.PageStatus.LoggedOut)
            {
                if (await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)RevolutItems.Items.LogoutedLoginButton]) != false)
                {
                    return true;
                }
                else return false;

                //"//h2[contains(text(), 'You have been logged out')]",                                                 //  "//h2[contains(text(), 'Wylogowaliśmy Cię')]",
                //"//button//span[contains(.,'Log in')]/parent::button",                                                //  "//button//span[contains(.,'Zaloguj się')]/parent::button",
            }
            else if (pageStatus == RevolutPageStatus.PageStatus.SomethingWrong)
            {
                if (await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)RevolutItems.Items.SomethingGoesWrongUnderstandButton]) != false)
                {
                    if (await revolutService.GoToAsync("https://app.revolut.com/login", 120000) == true)  //***HERE*** check how its work
                        return true;
                }
                else return false;

                //"//span//div//span[contains(.,'Something went wrong, please try again later')]",                      //  "//span//div//span[contains(.,'Coś poszło nie tak. Spróbuj później.')]",
                //"//button[contains(.,'Got it')]",                                                                     //  "//button[contains(.,'Rozumiem')]",
            }
            else if (pageStatus == RevolutPageStatus.PageStatus.LoginToRevolutMain)
            {
                if (await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)RevolutItems.Items.MainTabButtonMail]) != false)
                {
                    if (await revolutService.WaitFor(RevolutItems.ItemsCSS[(int)RevolutItems.Items.MainInputPlaceHolderMail]) != false)
                    {
                        if (await revolutService.InsertText(RevolutItems.ItemsCSS[(int)RevolutItems.Items.MainInputPlaceHolderMail], inputEmail) != false)
                        {
                            if (await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)RevolutItems.Items.MainButtonContinue]) != false)
                            {
                                return true;
                            }
                            //"//main//h1[contains(.,'Log in to Revolut')]",                                                        //  "//main//h1[contains(.,'Zaloguj się do Revolut')]",
                            //"//div//button[contains(.,'Email')]",                                                                 //  "//div//button[contains(.,'E-mail')]",
                            //"input[placeholder='Enter your email']",                                                              //  "input[placeholder='Podaj swój e-mail']",
                            //"//span[contains(.,'Continue')]/parent::button",                                                      //  "//span[contains(.,'Kontynuuj')]/parent::button",
                        }

                    }

                }
            }
            else if (pageStatus == RevolutPageStatus.PageStatus.LoginToRevolutSpan || pageStatus == RevolutPageStatus.PageStatus.LoginSelectAccount)
            {
                if (await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)RevolutItems.Items.LoginAsButton]) != false)
                {

                    return true;
                }
                else return false;

                //"//span//h1[contains(.,'Log in to Revolut')]",                                                        //  "//span//h1[contains(.,'Zaloguj się do Revolut')]",
                //"button[aria-label*='Continue as']",                                                                  //  "button[aria-label*='Kontynuuj jako']",
            }
            else if (pageStatus == RevolutPageStatus.PageStatus.VerifyYourIdentityVia)
            {
                if (await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)RevolutItems.Items.ChooseAnotherMethodMailButton]) != false)
                {
                    revolutService.TimeLastClickedMail = DateTime.Now;
                    return true;
                }

                //"//span[contains(text(),'Verify your identity via')]",                                                //  "//span[contains(text(),'Zweryfikuj tożsamość poprzez')]",
                //"//span[contains(.,'Email')]/parent::button",                                                         //  "//span[contains(.,'E-mail')]/parent::button",
            }
            else if (pageStatus == RevolutPageStatus.PageStatus.VerifyByAppRevolut)
            {
                if (await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)RevolutItems.Items.ChooseAnotherMethodButton]) != false)
                {
                    return true;
                }

                //"//span[contains(text(),'Confirm with Revolut app')]",                                                //  "//span[contains(text(),'Potwierdź za pomocą aplikacji Revolut')]",
                //"//button[contains(text(),'Choose another method')]",                                                 //  "//button[contains(text(),'Wybierz inną metodę')]",

            }
            else if (pageStatus == RevolutPageStatus.PageStatus.InsertPIN6)
            {
                if (await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)RevolutItems.Items.ChooseAnotherMethodButton]) != false)
                    if (await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)RevolutItems.Items.ChooseAnotherMethodMailButton]) != false)
                        return true;

                //"input[aria-label*='Code input 6']",                                                                  //  "input[aria-label*='Wprowadzenie kodu 6']",
                //"//button[contains(text(),'Choose another method')]",                                                 //  "//button[contains(text(),'Wybierz inną metodę')]",
                //"//span[contains(.,'Email')]/parent::button",                                                         //  "//span[contains(.,'E-mail')]/parent::button",
            }
            else if (pageStatus == RevolutPageStatus.PageStatus.InsertPIN4)
            {
                if (await revolutService.WaitFor(RevolutItems.ItemsCSS[(int)RevolutItems.Items.InputPin4]) != false)
                {
                    await revolutService.InsertText(RevolutItems.ItemsCSS[(int)RevolutItems.Items.InputPin1], inputPin1, revolutService.TypeOptions[1]);
                    await revolutService.InsertText(RevolutItems.ItemsCSS[(int)RevolutItems.Items.InputPin2], inputPin2, revolutService.TypeOptions[1]);
                    await revolutService.InsertText(RevolutItems.ItemsCSS[(int)RevolutItems.Items.InputPin3], inputPin3, revolutService.TypeOptions[1]);
                    await revolutService.InsertText(RevolutItems.ItemsCSS[(int)RevolutItems.Items.InputPin4], inputPin4, revolutService.TypeOptions[1]);
                }

                // pageStatus == RevolutPageStatus.PageStatus.InsertPIN4   WHEN    -->  CheckPageStatusSelector
                // Higher up in the array is a selector with the value 'input 6'. So if it doesn't find 6 pins, the next search will be 'Input 1' and then it detects a 4 pin window

                //"input[aria-label*='Code input 4']",                                                                  //  "input[aria-label*='Wprowadzenie kodu 4']",
                //"input[aria-label*='Code input 1']",                                                                  //  "input[aria-label*='Wprowadzenie kodu 1']",
                //"input[aria-label*='Code input 2']",                                                                  //  "input[aria-label*='Wprowadzenie kodu 2']",
                //"input[aria-label*='Code input 3']",                                                                  //  "input[aria-label*='Wprowadzenie kodu 3']",
                //"input[aria-label*='Code input 4']",                                                                  //  "input[aria-label*='Wprowadzenie kodu 4']",
            }
            else if (pageStatus == RevolutPageStatus.PageStatus.MainPage)
            {
                await CheckTimeAndAccountBalances();
                await DownloadExcelAndPrintNewTransfers();
                //***HERE***
                revolutService.CheckIfTheTransferArrived();

                return true;

                //"[data-testid='homePageHeader']",                                                     //  "[data-testid='homePageHeader']",
            }
            else if (pageStatus == RevolutPageStatus.PageStatus.CheckMail)
            {
                while (DateTime.Compare(revolutService.TimeLastClickedMail, DateTime.Now.AddSeconds(-220)) > 0) // 220 seconds to check mail and if without succes in 100s then resend mail
                {
                    await gmailService.CheckInbox(2000, false);
                    revolutService.ReturnAndSetClientIdFromUrl();
                    if (DateTime.Compare(revolutService.TimeLastClickedMail, DateTime.Now.AddSeconds(-100)) > 0) // check mail if have not passed 100 seconds
                    {
                        GmailMail? authorizationMail = gmailService.ReturnAuthorizationMail(revolutService.Client_ID);
                        if (authorizationMail != null)
                        {
                            if (DateTime.Compare(authorizationMail.Date, DateTime.Now.AddMinutes(-3)) > 0)
                            {
                                if (await revolutService.GoToAsync(authorizationMail.Link))
                                {
                                    gmailService.RemoveMail(authorizationMail.Id);
                                    if (await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)RevolutItems.Items.MenuAcceptAllCookiesButton], 5000) != false)
                                    {
                                        await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)RevolutItems.Items.MenuCloseAdvertisementButton], 5000); // only one dont work
                                        await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)RevolutItems.Items.MenuCloseAdvertisementButton], 5000); // must x2,   to fix
                                        return true;

                                        //"//span[contains(text(), 'Allow all cookies')]/parent::button",                                       //  "//span[contains(text(), 'Zezwól na wszystkie pliki cookie')]/parent::button",
                                        //"button[aria-label*='Close']",                                                                        //  "button[aria-label*='Close']"
                                        //"button[aria-label*='Close']",                                                                        //  "button[aria-label*='Close']"
                                    }
                                }
                            }
                            else
                            {
                                gmailService.RemoveMail(authorizationMail.Id);
                            }
                        }
                        else
                        {
                            await Task.Delay(2000);
                        }
                    }
                    else    // if 100 seconds have passed, resend mail
                    {
                        await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)RevolutItems.Items.ChooseAnotherMethodMailResendButton], 5000);
                        // revolutService.TimeLastClickedMail = DateTime.Now;

                        //"//span[contains(.,'Resend email')]/parent::button",                                                  //  "//span[contains(.,'Wyślij ponownie wiadomość e-mail')]/parent::button",
                    }
                }
            }
            else if (pageStatus == RevolutPageStatus.PageStatus.Error)
            {
                return false;
            }

            else
            {
                return false;
            }
            return false;
        }

        private async Task<bool> CheckTimeAndAccountBalances()
        {

            if (DateTime.Compare(revolutService.TimeLastCheckAccount, DateTime.Now.AddMinutes(-revolutService.HowOftenToCheckAccountsWhileWaitingForATransfer)) < 0)
            {
                if (await revolutService.GoToAsync("https://app.revolut.com/home", 120000) == true)  // check transfer automatic update web, if not then that line must to be
                {
                    if (await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)RevolutItems.Items.MenuPriceHideableButton]))
                    {
                        if (await revolutService.SetActualCurrencyAndTime())
                        {
                            revolutService.TimeLastCheckAccount = DateTime.Now;
                            await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)RevolutItems.Items.MenuPriceCurrencyALLButton]);
                            return true;
                        }
                    }
                }
            }
            return false;

            //"//button//span[contains(@class, 'Hideable__HideableBase')]/parent::button",                          //  "//button//span[contains(@class, 'Hideable__HideableBase')]/parent::button",
            //"//button[contains(., 'All accounts')]",                                                              //  "//button[contains(., 'Wszystkie konta')]",
        }

        private async Task<bool> DownloadExcelAndPrintNewTransfers()
        {
            for (int i = 1; i < revolutService.PricesData.Count; i++)  // from 1, because without ALL      revolutService.PricesData[0].Curriencies = ALL
            {
                if (revolutService.PricesData[i].Print)
                {
                    if (await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)RevolutItems.Items.MenuPriceHideableButton], 5000))
                    {
                        if (Enum.TryParse<RevolutItems.Items>("MenuPriceCurrency" + Enum.GetName(typeof(RevolutCurriencies), revolutService.PricesData[i].Curriencies) + "Button", out RevolutItems.Items resultCurriencies))
                        {

                            if (await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)resultCurriencies]))
                            {
                                /// DOWNLOAD CVF IF CHANGE
                                string path = await revolutService.SetDownloadPathFile(revolutService.PricesData[i].Curriencies);

                                if (await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)RevolutItems.Items.MenuPrintFinanceButton]))
                                {
                                    if (await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)RevolutItems.Items.MenuPrintFinanceExcelButton]))
                                    {

                                        if (await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)RevolutItems.Items.MenuPrintFinanceStartDateButton]))
                                            if (await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)MenuPrintFinanceStartDateMonthButton], 4000))
                                            {
                                                if (await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)RevolutItems.Items.MenuPrintFinanceGenerateCVSButton], 4000))
                                                {
                                                    await revolutService.WaitFor(RevolutItems.ItemsCSS[(int)RevolutItems.Items.MenuPrintFinanceDownloadStatementIsReady], 2000);
                                                    await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)RevolutItems.Items.MenuPrintFinanceDownloadButton], 7000);
                                                    await revolutService.WaitAndClickItem(RevolutItems.ItemsCSS[(int)RevolutItems.Items.MenuPrintFinanceBackToMainMenuButton]);
                                                    if (path != string.Empty)
                                                    {
                                                        await revolutService.CheckFileDownloadAndLoad(path, revolutService.PricesData[i].Curriencies);
                                                    }
                                                }
                                            }
                                    }
                                }
                            }
                        }
                    }
                    revolutService.PricesData[i].LastChange = DateTime.Now;
                    revolutService.PricesData[i].Print = false;
                }
            }
            return true;

            //"//button//span[contains(@class, 'Hideable__HideableBase')]/parent::button",                          //  "//button//span[contains(@class, 'Hideable__HideableBase')]/parent::button",
            //"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'XXX')]",    XXX - Currencies
            //"//button[contains(., 'Statement')]",                                                                 //  "//button[contains(., 'Wyciąg')]",
            //"//button[contains(., 'Excel')]",                                                                     //  "//button[contains(., 'Excel')]",
            //"//div[contains(text(), 'Starting on')]/following-sibling::div//input[@type='button']",               //  "//div[contains(text(), 'Data początkowa')]/following-sibling::div//input[@type='button']",
            //"//div[contains(text(), 'Jan')]",                                                                     //  "//div[contains(text(), 'sty')]",
            //"//button[contains(., 'Generate')]",                                                                  //  "//button[contains(., 'Wygeneruj')]",
            //"//span[contains(text(), 'Statement is ready')]",                                                     //  "//span[contains(text(), 'Wyciąg jest gotowy')]"  ??
            //"//button[contains(., 'Download')]",                                                                  //  "//button[contains(., 'Pobierz')]",
            //"//button[contains(@aria-label, 'Back from statement')]",                                             //  "//button[contains(@aria-label, 'Back from statement')]",
        }
        private RevolutOrderManager.Order CheckOrderManager()
        {
            if ((revolutService.WaitingForTransfer == false && DateTime.Compare(revolutService.TimeLastCheckAccount, DateTime.Now.AddMinutes(-revolutService.HowOftenToCheckAccountsWhithoutWaitingForTransfer)) < 0) || revolutService.WaitingForTransfer == true)
            {
                return RevolutOrderManager.Order.LoginAndCheckAccountBallance;
            }
            else
            {
                return RevolutOrderManager.Order.Wait;
            }
        }


        public async Task StartManager()
        {
            int CountOfFalse = 0;
            await Task.Run(async () =>
            {
                while (true)
                {
                    RevolutOrderManager.Order order = CheckOrderManager();

                    if (order == RevolutOrderManager.Order.LoginAndCheckAccountBallance)
                    {
                        if (CountOfFalse > 30)
                        {
                            if (await revolutService.ReLaunchPage() == true)
                            {
                                CountOfFalse = 0;
                            }
                        }

                        if (await WhatToDo() == false)
                        {
                            CountOfFalse++;
                            await Task.Delay(1000);
                        }
                        else
                        {
                            CountOfFalse = 0;
                        }
                    }
                    else if (order == RevolutOrderManager.Order.Wait)
                    {
                        await Task.Delay(60000);
                    }
                }
            });
        }
    }
}


