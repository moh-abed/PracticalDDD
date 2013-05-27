﻿using System;

namespace Sample.Domain
{
    public interface INotificationService
    {
        void SendEmail(string email, string subject, string body);
    }

    public class NotificationService : INotificationService
    {
        public void SendEmail(string email, string subject, string body)
        {
            Printer.Print(ConsoleColor.Green);
        }
    }
}
