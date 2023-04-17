using System.Globalization;

namespace RevolutChecker
{
    public class OrderedCashTransfer
    {

        private readonly decimal cashAmount;
        private readonly string name;
        private readonly string surname;
        private readonly string nickname;
        private readonly RevolutCurriencies revolutCurriencies;
        private readonly bool firstTranfser;
        private readonly decimal possibleDevation;
        private readonly DateTime startOrderDate;
        private DateTime endOrderDate= DateTime.MinValue;
        private bool transferSucces=false;

        public OrderedCashTransfer(decimal cashAmount, string name, string surname, string nickname, RevolutCurriencies revolutCurriencies, bool firstTransfer = true, decimal possibleDeviation = 0)
        {
        this.cashAmount = cashAmount;
            this.name = name;
            this.surname = surname; 
            this.nickname = nickname;
            this.revolutCurriencies = revolutCurriencies;
            // ***HERE***
            // need to make list of trusted client, firstTransfer
            this.firstTranfser = firstTransfer;
            // need to make list of trusted client, firstTransfer
            // ***HERE***
            this.possibleDevation = possibleDeviation;
            this.startOrderDate = DateTime.Now;


        }

        public decimal CashAmount { get { return cashAmount; }}
        public string Name { get { return name; }}
        public string Surname { get {  return surname; }}
        public string Nickname { get { return nickname; }}
        public RevolutCurriencies RevolutCurriencies { get {  return revolutCurriencies; }}
        public bool  FirstTransfer {  get { return firstTranfser;} }
        public decimal PossibleDevation { get { return possibleDevation; }} 
        public DateTime StartOrderDate { get { return startOrderDate; }}
        public DateTime EndOrderDate { get { return endOrderDate; } }
        public bool TransferSucces { get {  return transferSucces; }}


        public void SetOrderAsSuccess()
        {
            transferSucces = true;
            endOrderDate = DateTime.Now;
        }

        // ***HERE***
        // function 
        // need to make list of trusted client
        // ***HERE***
    }

    public class FinancialTransaction
    {
        public string? Type { get; set; }
        public string? Product { get; set; }
        public DateTime? StartedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? Description { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Fee { get; set; }
        public string? Currency { get; set; }
        public string? State { get; set; }
        public decimal? Balance { get; set; }
    }

    public class FinancialParser
    {
        public static List<FinancialTransaction> ParseTransactions(string input)
        {
            string[] lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            List<FinancialTransaction> transactions = new();

            foreach (var line in lines[1..])
            {
                string[] values = line.Split(',');

                FinancialTransaction transaction = new()
                {
                    Type = values[0],
                    Product = values[1],
                    StartedDate = DateTime.Parse(values[2]),
                    CompletedDate = DateTime.Parse(values[3]),
                    Description = values[4],
                    Amount = decimal.Parse(values[5], CultureInfo.InvariantCulture),
                    Fee = decimal.Parse(values[6], CultureInfo.InvariantCulture),
                    Currency = values[7],
                    State = values[8],
                    Balance = decimal.Parse(values[9], CultureInfo.InvariantCulture)
                };
                transactions.Add(transaction);
            }

            //***HERE***
            //add encoding 

            return transactions;
        }
    }
    public class Prices
    {
        public Prices(bool print, DateTime lastChange, RevolutCurriencies revolutCurriencies, string price)
        {
            this.print = print;
            this.lastChange = lastChange;
            this.revolutCurriencies = revolutCurriencies;
            this.price = price;
        }

        bool print = false;
        DateTime lastChange = DateTime.MinValue;
        RevolutCurriencies revolutCurriencies = RevolutCurriencies.ALL;
        string price = string.Empty;

        public bool Print { get { return print; } set { print = value; } }
        public DateTime LastChange { get { return lastChange; } set { lastChange = value; } }
        public RevolutCurriencies Curriencies { get { return revolutCurriencies; } set { revolutCurriencies = value; } }
        public string Price { get { return price; } set { price = value; } }
    }
}
