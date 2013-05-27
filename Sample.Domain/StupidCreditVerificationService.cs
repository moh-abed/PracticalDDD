namespace Sample.Domain
{
    public class StupidCreditVerificationService
    {
        public bool IsValidCreditCard(string nameOnCard, string cardNumber)
        {
            return !string.IsNullOrEmpty(nameOnCard) && !string.IsNullOrEmpty(cardNumber);
        }
    }
}
