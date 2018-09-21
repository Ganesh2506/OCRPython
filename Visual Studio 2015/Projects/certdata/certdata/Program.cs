using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace certdata
{
    class Program
    {
        static void Main(string[] args)
        {
            // The path to the certificate.
            string Certificate = @"C:\Work\certificates\GorillaQCWebApp.cer";

            // Load the certificate into an X509Certificate object.
            X509Certificate cert = X509Certificate.CreateFromCertFile(Certificate);

            // Get the value.
            byte[] results = cert.GetRawCertData();

            // Display the value to the console.
            foreach (byte b in results)
            {
                Console.Write(b);
            }
            Console.ReadLine();
        }
    }
}
