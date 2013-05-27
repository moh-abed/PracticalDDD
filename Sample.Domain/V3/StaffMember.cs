using System;
using Sample.Domain.Shared;

namespace Sample.Domain.V3
{
    public class StaffMember
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        private StaffMember(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public static StaffMember Register(string name)
        {
            Printer.Print(ConsoleColor.Cyan);
            return new StaffMember(Guid.NewGuid(), name);
        }
    }
}
