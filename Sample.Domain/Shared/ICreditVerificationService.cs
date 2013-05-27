namespace Sample.Domain.Shared
{
    public interface ICreditVerificationService
    {
        bool IsValidCreditCard(string nameOnCard, string cardNumber);
    }
}
