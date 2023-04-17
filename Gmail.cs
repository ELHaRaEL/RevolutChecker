using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Text;

namespace RevolutChecker
{
    public class Gmail
    {
        private readonly GmailService gmailService = new();
        private static readonly string[] scopes = { GmailService.Scope.MailGoogleCom };
        private readonly List<GmailMail> listOfGmailMails = new();
        private readonly string mailAddress;

        public GmailMail? ReturnAuthorizationMail(string client_ID)
        {
            GmailMail? authorizationMail = listOfGmailMails.FirstOrDefault(item => item.Client_id == client_ID);
            return authorizationMail;
        }

        private void AddMail(GmailMail gmailMail)
        {
            listOfGmailMails.Add(gmailMail);
        }


        public static byte[] FromBase64ForUrlString(string base64ForUrlInput)
        {
            int padChars = (base64ForUrlInput.Length % 4) == 0 ? 0 : (4 - (base64ForUrlInput.Length % 4));
            StringBuilder result = new(base64ForUrlInput, base64ForUrlInput.Length + padChars);
            _ = result.Append(string.Empty.PadRight(padChars, '='));
            _ = result.Replace('-', '+');
            _ = result.Replace('_', '/');
            return Convert.FromBase64String(result.ToString());
        }

        public void RemoveMail(string email_id)
        {
            GmailMail? itemToRemove = listOfGmailMails.SingleOrDefault(p => p.Id == email_id);
            if (itemToRemove != null)
            {
                _ = listOfGmailMails.Remove(itemToRemove);
                _ = this.gmailService.Users.Messages.Trash(mailAddress, itemToRemove.Id).Execute();
            }
        }

        public async Task CheckInbox(int delayMailRefresh, bool infinity)
        {
            bool infinitywhile = true;
            while (infinitywhile)
            {
                await Task.Run(async () =>
                {
                    UsersResource.MessagesResource.ListRequest inboxlistRequest = this.gmailService.Users.Messages.List(mailAddress);
                    inboxlistRequest.LabelIds = "INBOX";
                    inboxlistRequest.IncludeSpamTrash = false;
                    ListMessagesResponse EmailListResponse = inboxlistRequest.Execute();
                    if (EmailListResponse != null && EmailListResponse.Messages != null)
                    {
                        //loop through each email and get what fields you want
                        for (int i = 0; i < EmailListResponse.Messages.Count; i++)
                        {
                            if (listOfGmailMails.FindIndex(item => item.Id == EmailListResponse.Messages[i].Id) < 0)
                            {
                                Message email = EmailListResponse.Messages[i];
                                UsersResource.MessagesResource.GetRequest emailInfoRequest = this.gmailService.Users.Messages.Get(mailAddress, email.Id);
                                Message emailInfoResponse = emailInfoRequest.Execute();
                                if (emailInfoResponse != null)
                                {
                                    string from = string.Empty;
                                    string date = string.Empty;
                                    string subject = string.Empty;
                                    string decodedData = string.Empty;
                                    string link = string.Empty;
                                    string client_id = string.Empty;
                                    string nonce = string.Empty;
                                    string challenges = string.Empty;
                                    string code = string.Empty;

                                    //loop through the headers to get from,date,subject,body  
                                    foreach (MessagePartHeader? mParts in emailInfoResponse.Payload.Headers)
                                    {
                                        if (mParts.Name == "Date")
                                        {
                                            date = mParts.Value;
                                        }
                                        else if (mParts.Name == "From")
                                        {
                                            from = mParts.Value;
                                        }
                                        else if (mParts.Name == "Subject")
                                        {
                                            subject = mParts.Value;
                                        }
                                        if (date != "" && from != "")
                                        {
                                            if (emailInfoResponse.Payload.Parts != null)
                                            {
                                                foreach (MessagePart p in emailInfoResponse.Payload.Parts)
                                                {
                                                    if (p.MimeType == "text/html")
                                                    {
                                                        byte[] data = FromBase64ForUrlString(p.Body.Data);
                                                        string decodedString = Encoding.UTF8.GetString(data);
                                                        decodedData += decodedString;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                byte[] data = FromBase64ForUrlString(emailInfoResponse.Payload.Body.Data);
                                                decodedData = Encoding.UTF8.GetString(data);
                                            }
                                        }
                                    }
                                    DateTime dateTime = DateTime.Now;
                                    _ = DateTime.TryParse(date, out dateTime);
                                    if (from.Contains("Revolut <no-reply@revolut.com>") && decodedData.Contains("https://sso.revolut.com/challenges/"))
                                    {

                                        link = decodedData.Remove(0, decodedData.IndexOf("https://sso.revolut.com/challenges/"));
                                        link = link.Remove(link.IndexOf("\"")).Trim();

                                        client_id = link.Remove(0, link.IndexOf("client_id%3D") + 12).Trim();
                                        client_id = client_id.Remove(client_id.IndexOf("%")).Trim();

                                        code = link.Remove(0, link.IndexOf("code=") + 5).Trim();
                                        code = code.Remove(code.IndexOf("&")).Trim();

                                        challenges = link.Remove(0, link.IndexOf("challenges/") + 11).Trim();
                                        challenges = challenges.Remove(challenges.IndexOf("?")).Trim();


                                        nonce = link.Remove(0, link.IndexOf("nonce%3D") + 8).Trim();
                                        if (nonce.IndexOf("&") != -1)
                                        {
                                            nonce = nonce.Remove(nonce.IndexOf("&")).Trim();
                                        }
                                        AddMail(new GmailMail(emailInfoResponse.Id, from, subject, decodedData, dateTime, false, link, client_id, nonce, challenges, code));

                                    }
                                    else
                                    {
                                        AddMail(new GmailMail(emailInfoResponse.Id, from, subject, decodedData, dateTime, false, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
                                    }
                                }
                            }
                        }
                    }
                    await Task.Delay(delayMailRefresh);
                });
                if (infinity == false)
                {
                    infinitywhile = false;
                }
            }
        }


        public Gmail(string applicationName, string mailAddress, string pathToClientSecretJson)
        {
            this.mailAddress = mailAddress;
            this.gmailService = LoadJsonClientSecret(mailAddress, applicationName, pathToClientSecretJson);
        }

        private static GmailService LoadJsonClientSecret(string mailAddress, string applicationName, string pathToClientSecretJson)
        {
            GmailService gmailService = new();
            using (FileStream stream = new(pathToClientSecretJson + "\\client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                credPath = System.IO.Path.Combine(credPath, ".credentials/gmail-dotnet-quickstart.json");
                UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.FromStream(stream).Secrets, scopes, mailAddress, CancellationToken.None, new FileDataStore(credPath, true)).Result;
                gmailService = new(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = applicationName
                });
            }
            return gmailService;
        }
    }

    public class GmailMail
    {
        public string Id { get; }
        public string From { get; }
        public string Subject { get; }
        public string DecodedData { get; }
        public bool Use { get; }
        public string Link { get; }
        public string Client_id { get; }
        public string Nonce { get; }
        public string Challenges { get; }
        public string Code { get; }
        public DateTime Date { get; }


        public GmailMail(string id, string from, string subject, string decodedData, DateTime date, bool use, string link, string client_id, string nonce, string challenges, string code)
        {
            Id = id;
            From = from;
            Subject = subject;
            DecodedData = decodedData;
            Date = date;
            Use = use;
            Link = link;
            Client_id = client_id;
            Nonce = nonce;
            Challenges = challenges;
            Code = code;
        }
    }



}
