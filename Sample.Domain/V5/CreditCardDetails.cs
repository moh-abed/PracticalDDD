using System;

namespace Sample.Domain.V5
{
    public class CreditCardDetails
    {
        private string nameOnCard;
        private string cardNumber;

        public CreditCardDetails(string nameOnCard, string cardNumber)
        {
            NameOnCard = nameOnCard;
            CardNumber = cardNumber;
        }

        public CreditCardDetails(CreditCardDetails cardDetails)
        {
            NameOnCard = cardDetails.NameOnCard;
            CardNumber = cardDetails.CardNumber;
        }

        public string NameOnCard
        {
            get { return nameOnCard; }
            private set
            {
                if(string.IsNullOrEmpty(value))
                    throw new Exception("Name on cared is required");

                nameOnCard = value;
            }
        }

        public string CardNumber
        {
            get { return cardNumber; }
            private set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Exception("Card number is required");

                cardNumber = value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", NameOnCard, CardNumber);
        }
    }
}