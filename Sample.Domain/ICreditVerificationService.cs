namespace Sample.Domain
{
    public interface ICreditVerificationService
    {
        bool IsValidCreditCard(string nameOnCard, string cardNumber);
    }
}
