using System;

namespace Sample.Domain.V2
{
    public class FullName
    {
        private string firstName;
        private string familyName;

        public FullName(string firstName, string familyName)
        {
            FirstName = firstName;
            FamilyName = familyName;
        }

        public FullName(FullName fullName)
        {
            FirstName = fullName.FirstName;
            FamilyName = fullName.FamilyName;
        }

        public string FirstName
        {
            get { return firstName; }
            private set
            {
                if(string.IsNullOrEmpty(value))
                    throw new Exception("First name is required");

                firstName = value;
            }
        }

        public string FamilyName
        {
            get { return familyName; }
            private set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Exception("Family name is required");

                familyName = value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", FirstName, FamilyName);
        }
    }
}