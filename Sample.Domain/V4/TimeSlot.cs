using System;
using System.Globalization;

namespace Sample.Domain.V4
{
    public class TimeSlot
    {
        public TimeSlot(DateTime from, DateTime to)
        {
            if (to < from)
                throw new Exception("Timeslot is invalid (to is earlier than from)");

            From = from;
            To = to;
        }

        public TimeSlot(TimeSlot slot)
        {
            From = slot.From;
            To = slot.To;
        }

        DateTime from;
        public DateTime From
        {
            get { return from; }
            private set { from = value; }
        }

        private DateTime to;
        public DateTime To
        {
            get { return to; }
            private set
            {
                if(value < from)
                    throw new Exception("Timeslot is invalid (to is earlier than from)");

                to = value;
            }
        }

        public bool IsInRange(DateTime time)
        {
            return (time >= From && time <= to);
        }

        public void Shift(TimeSpan amount)
        {
            From = From.Add(amount);
            To = To.Add(amount);
        }

        public TimeSpan ToTimespan()
        {
            return to - from;
        }

        public override string ToString()
        {
            return from.ToString(CultureInfo.InvariantCulture) + " - " + to.ToString(CultureInfo.InvariantCulture);
        }
    }
}