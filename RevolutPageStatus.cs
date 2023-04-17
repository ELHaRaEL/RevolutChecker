namespace RevolutChecker
{
    internal class RevolutPageStatus
    {

        public enum PageStatus
        {
            LoggedOut,
            SomethingWrong,
            LoginToRevolutMain,
            LoginToRevolutSpan,
            LoginSelectAccount,
            VerifyYourIdentityVia,
            VerifyByAppRevolut,
            InsertPIN6,
            InsertPIN4,
            MainPage,
            CheckMail,
            //          GeneratingCVS,   
            BlankPage,
            Error
        }

        public readonly static string[] XPathsPageStatus = new string[] {
        "//h2[contains(text(), 'You have been logged out')]",                                 //  "//h2[contains(text(), 'Wylogowaliśmy Cię')]",
        "//span//div//span[contains(.,'Something went wrong, please try again later')]",      //  "//span//div//span[contains(.,'Coś poszło nie tak. Spróbuj później.')]",
        "//main//h1[contains(.,'Log in to Revolut')]",                                        //  "//main//h1[contains(.,'Zaloguj się do Revolut')]",
        "//span//h1[contains(.,'Log in to Revolut')]",                                        //  "//span//h1[contains(.,'Zaloguj się do Revolut')]",
        "//span//span[contains(.,'Select account to continue')]",                             //  "//span//span[contains(.,'Wybierz konto, aby kontynuować')]",
        "//span[contains(text(),'Verify your identity via')]",                                //  "//span[contains(text(),'Zweryfikuj tożsamość poprzez')]",
        "//span[contains(text(),'Confirm with Revolut app')]",                                //  "//span[contains(text(),'Potwierdź za pomocą aplikacji Revolut')]",
        "input[aria-label*='Code input 6']",                                                  //  "input[aria-label*='Wprowadzenie kodu 6']",
        "input[aria-label*='Code input 1']",                                                  //  "input[aria-label*='Wprowadzenie kodu 1']",
        "[data-testid='homePageHeader']",                                                     //  "[data-testid='homePageHeader']",
        "//div//div//div//span[contains(.,'Check your email on this device')]",               //  "//div//div//div//span[contains(.,'Sprawdź skrzynkę e-mail na tym urządzeniu')]",
        //"//span//span[contains(., 'Your statement is being generated')]"                    //  "//span//span[contains(., 'Trwa generowanie wyciągu')]"
    };
    }


}
