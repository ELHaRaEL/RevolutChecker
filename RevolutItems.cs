using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevolutChecker
{
    internal class RevolutItems
    {



        readonly public static string[] ItemsCSS = {
                  // XPath
"//div//button[contains(.,'Email')]",                                                                 //  "//div//button[contains(.,'E-mail')]",
"//span[contains(.,'Continue')]/parent::button",                                                      //  "//span[contains(.,'Kontynuuj')]/parent::button",
"//button[contains(text(),'Choose another method')]",                                                 //  "//button[contains(text(),'Wybierz inną metodę')]",
"//span[contains(.,'Email')]/parent::button",                                                         //  "//span[contains(.,'E-mail')]/parent::button",
"//span[contains(.,'Resend email')]/parent::button",                                                  //  "//span[contains(.,'Wyślij ponownie wiadomość e-mail')]/parent::button",
"//button[contains(.,'Got it')]",                                                                     //  "//button[contains(.,'Rozumiem')]",
"//button//span[contains(.,'Log in')]/parent::button",                                                //  "//button//span[contains(.,'Zaloguj się')]/parent::button",
"//button//span[contains(@class, 'Hideable__HideableBase')]/parent::button",                          //  "//button//span[contains(@class, 'Hideable__HideableBase')]/parent::button",
"//button[contains(., 'Statement')]",                                                                 //  "//button[contains(., 'Wyciąg')]",
"//button[contains(., 'Excel')]",                                                                     //  "//button[contains(., 'Excel')]",
"//div[contains(text(), 'Starting on')]/following-sibling::div//input[@type='button']",               //  "//div[contains(text(), 'Data początkowa')]/following-sibling::div//input[@type='button']",
"//div[contains(text(), 'Jan')]",                                                                     //  "//div[contains(text(), 'sty')]",
"//div[contains(text(), 'Fab')]",                                                                     //  "//div[contains(text(), 'lut')]",
"//div[contains(text(), 'Mar')]",                                                                     //  "//div[contains(text(), 'mar')]",
"//div[contains(text(), 'Apr')]",                                                                     //  "//div[contains(text(), 'kwi')]",
"//div[contains(text(), 'May')]",                                                                     //  "//div[contains(text(), 'maj')]",
"//div[contains(text(), 'Jun')]",                                                                     //  "//div[contains(text(), 'cze')]",
"//div[contains(text(), 'Jul')]",                                                                     //  "//div[contains(text(), 'lip')]",
"//div[contains(text(), 'Aug')]",                                                                     //  "//div[contains(text(), 'sie')]",
"//div[contains(text(), 'Sept')]",                                                                    //  "//div[contains(text(), 'wrz')]",
"//div[contains(text(), 'Oct')]",                                                                     //  "//div[contains(text(), 'paz')]",
"//div[contains(text(), 'Nov')]",                                                                     //  "//div[contains(text(), 'lis')]",
"//div[contains(text(), 'Dec')]",                                                                     //  "//div[contains(text(), 'gru')]",
"//button[contains(., 'Generate')]",                                                                  //  "//button[contains(., 'Wygeneruj')]",
"//button[contains(., 'Download')]",                                                                  //  "//button[contains(., 'Pobierz')]",
"//span[contains(text(), 'Statement is ready')]",                                                     //  "//span[contains(text(), 'Wyciąg jest gotowy')]"  ??
"//div[contains(text(), 'Ending on')]/following-sibling::div//input[@type='button']",                 //  "//div[contains(text(), 'Data końcowa')]/following-sibling::div//input[@type='button']",
"//button[contains(@aria-label, 'Back from statement')]",                                             //  "//button[contains(@aria-label, 'Back from statement')]",
"//span[contains(text(), 'Allow all cookies')]/parent::button",                                       //  "//span[contains(text(), 'Zezwól na wszystkie pliki cookie')]/parent::button",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'AED')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'AUD')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'BGN')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'CAD')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'CHF')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'CZK')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'DKK')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'EUR')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'GBP')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'HKD')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'HUF')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'ILS')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'ISK')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'JPY')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'MAD')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'MXN')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'NOK')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'NZD')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'PLN')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'QAR')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'RON')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'RSD')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'SAR')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'SEK')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'SGD')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'THB')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'TRY')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'USD')]",
"/html/body/div[5]/div/span/div[1]/div[5]//button[contains(.,'ZAR')]",
"//button[contains(., 'All accounts')]",                                                              //  "//button[contains(., 'Wszystkie konta')]",
                

  // SELECTORS
"input[aria-label*='Code input 1']",                                                                  //  "input[aria-label*='Wprowadzenie kodu 1']",
"input[aria-label*='Code input 2']",                                                                  //  "input[aria-label*='Wprowadzenie kodu 2']",
"input[aria-label*='Code input 3']",                                                                  //  "input[aria-label*='Wprowadzenie kodu 3']",
"input[aria-label*='Code input 4']",                                                                  //  "input[aria-label*='Wprowadzenie kodu 4']",
"input[aria-label*='Code input 5']",                                                                  //  "input[aria-label*='Wprowadzenie kodu 5']",
"input[aria-label*='Code input 6']",                                                                  //  "input[aria-label*='Wprowadzenie kodu 6']",
"button[aria-label*='Continue as']",                                                                  //  "button[aria-label*='Kontynuuj jako']",
"input[placeholder='Enter your email']",                                                              //  "input[placeholder='Podaj swój e-mail']",
"button[aria-label*='Close']",                                                                        //  "button[aria-label*='Close']"
};


        public enum Items
        {
            // XPath

            MainTabButtonMail,
            MainButtonContinue,
            ChooseAnotherMethodButton,
            ChooseAnotherMethodMailButton,
            ChooseAnotherMethodMailResendButton,
            SomethingGoesWrongUnderstandButton,
            LogoutedLoginButton,
            MenuPriceHideableButton,
            MenuPrintFinanceButton,
            MenuPrintFinanceExcelButton,
            MenuPrintFinanceStartDateButton,
            MenuPrintFinanceStartDateJanuaryButton,
            MenuPrintFinanceStartDateFabuaryButton,
            MenuPrintFinanceStartDateMarchButton,
            MenuPrintFinanceStartDateAprilButton,
            MenuPrintFinanceStartDateMayButton,
            MenuPrintFinanceStartDateJuneButton,
            MenuPrintFinanceStartDateJulyButton,
            MenuPrintFinanceStartDateAugustButton,
            MenuPrintFinanceStartDateSeptemberButton,
            MenuPrintFinanceStartDateOctoberButton,
            MenuPrintFinanceStartDateNovemberButton,
            MenuPrintFinanceStartDateDecemberButton,
            MenuPrintFinanceGenerateCVSButton,
            MenuPrintFinanceDownloadButton,
            MenuPrintFinanceDownloadStatementIsReady,
            MenuPrintFinanceEndDateButton,
            MenuPrintFinanceBackToMainMenuButton,
            MenuAcceptAllCookiesButton,
            MenuPriceCurrencyAEDButton,
            MenuPriceCurrencyAUDButton,
            MenuPriceCurrencyBGNButton,
            MenuPriceCurrencyCADButton,
            MenuPriceCurrencyCHFButton,
            MenuPriceCurrencyCZKButton,
            MenuPriceCurrencyDKKButton,
            MenuPriceCurrencyEURButton,
            MenuPriceCurrencyGBPButton,
            MenuPriceCurrencyHKDButton,
            MenuPriceCurrencyHUFButton,
            MenuPriceCurrencyILSButton,
            MenuPriceCurrencyISKButton,
            MenuPriceCurrencyJPYButton,
            MenuPriceCurrencyMADButton,
            MenuPriceCurrencyMXNButton,
            MenuPriceCurrencyNOKButton,
            MenuPriceCurrencyNZDButton,
            MenuPriceCurrencyPLNButton,
            MenuPriceCurrencyQARButton,
            MenuPriceCurrencyRONButton,
            MenuPriceCurrencyRSDButton,
            MenuPriceCurrencySARButton,
            MenuPriceCurrencySEKButton,
            MenuPriceCurrencySGDButton,
            MenuPriceCurrencyTHBButton,
            MenuPriceCurrencyTRYButton,
            MenuPriceCurrencyUSDButton,
            MenuPriceCurrencyZARButton,
            MenuPriceCurrencyALLButton,

            // SELECTORS

            InputPin1,
            InputPin2,
            InputPin3,
            InputPin4,
            InputPin5,
            InputPin6,
            LoginAsButton,
            MainInputPlaceHolderMail,
            MenuCloseAdvertisementButton
        }



    }
}
