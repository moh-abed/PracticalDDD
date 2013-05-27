using System;
using System.Text.RegularExpressions;

namespace Sample.Domain.V2
{
    public class Email
    {
        private string emailString;

        public Email(string emailString)
        {
            EmailString = emailString.ToLower();
        }

        public Email(Email email)
        {
            EmailString = email.EmailString.ToLower();
        }

        private string EmailString
        {
            get { return emailString.ToLower(); }
            set
            {
                if(string.IsNullOrEmpty(value))
                    throw new Exception("Email can not be empty string");

                if (!ValidateEmail(value))
                    throw new ArgumentException("Email is invalid.");

                emailString = value.ToLower();
            }
        }

        private static bool ValidateEmail(string value)
        {
            var emailRegex = new Regex(@"[\w-]+(\.?[\w-])*\@[\w-]+(\.[\w-]+)+", RegexOptions.IgnoreCase);

            return emailRegex.IsMatch(value);
        }

        public override string ToString()
        {
            return EmailString;
        }

        public static implicit operator string(Email email)
        {
            if (email == null)
                return null;
            return email.EmailString;
        }

        public static implicit operator Email(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            return new Email(email.ToLower());
        }

        public static bool IsValid(string email)
        {
            return ValidateEmail(email);
        }
    }
}