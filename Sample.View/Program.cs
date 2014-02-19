using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace Sample.View
{
    class Program
    {
        static void Main(string[] args)
        {
            var thread = new Thread(ListenToJobViewUpdates);
            thread.Start();
        }

        private static void ListenToJobViewUpdates()
        {
            using (var pipeStream = new NamedPipeServerStream("EventSourcingSample"))
            {
                pipeStream.WaitForConnection();
                using (var sr = new StreamReader(pipeStream))
                {
                    Console.WriteLine("JobView Updated:");
                    string message;
                    while ((message = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(message);
                    }
                }
            }
            ListenToJobViewUpdates();
        }
    }
}
