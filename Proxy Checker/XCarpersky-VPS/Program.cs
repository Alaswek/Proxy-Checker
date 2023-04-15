using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Leaf.xNet;
using System.Diagnostics.Eventing.Reader;

namespace XCarpersky_VPS
{
    internal class Program
    {
        private static int working = 0;
        private static int failed = 0;


        static void Main(string[] args)
        {
            string logo = @"
            █─█ █▀▀ █▀▀█ █▀▀█ █▀▀█ █▀▀ █▀▀█ █▀▀ █─█ █──█ 
            ▄▀▄ █── █▄▄█ █▄▄▀ █──█ █▀▀ █▄▄▀ ▀▀█ █▀▄ █▄▄█ 
            ▀─▀ ▀▀▀ ▀──▀ ▀─▀▀ █▀▀▀ ▀▀▀ ▀─▀▀ ▀▀▀ ▀─▀ ▄▄▄█
               by AlaSwek03043#5324 for Patched.To" + "\n\n";
            Console.Clear();
            Console.WriteLine(logo);
            Console.WriteLine("[*] Welcome to xCarpersky");
            Console.WriteLine("[1] Proxy Checker \n");
            while (true)
            {
                string option = Console.ReadLine();
                if (option == "1")
                {
                    string url = "http://google.com"; // Adresa URL a paginii web către care se trimit cererile
                    string proxyFile = "proxy.txt"; // Numele fișierului text care conține lista de proxy-uri
                    List<string> proxyList = LoadProxyList(proxyFile); // Lista de proxy-uri citite din fișier

                    // Pentru fiecare proxy, creați un fir de execuție separat care să trimită cererea HTTP folosind acel proxy
                    foreach (string proxyUrl in proxyList)
                    {
                        Thread thread = new Thread(() =>
                        {
                            try
                            {
                                HttpRequest request = new HttpRequest();
                                request.Proxy = HttpProxyClient.Parse(proxyUrl); // Setează proxy-ul pentru cererea HTTP

                                // Trimiteți cererea HTTP folosind proxy-ul specificat
                                HttpResponse response = request.Get(url);

                                Interlocked.Increment(ref working);
                                Console.WriteLine($"[Working]: {proxyUrl}");

                                UpdateTitle(); // actualizeaza titlul in mod constant

                                if (response.StatusCode == HttpStatusCode.OK) // Salveaza proxy-urile care functioneaza intr-un fisier Working.txt
                                {
                                    File.AppendAllText("working.txt", proxyUrl + Environment.NewLine);
                                }
                            }
                            catch (Exception ex)
                            {
                                Interlocked.Increment(ref failed);
                                Console.WriteLine($"[Failed]: {proxyUrl}: {ex.Message}");

                                UpdateTitle();
                            }
                        });

                        thread.Start(); // Pornim momentul de execuție
                    }
                    Console.ReadKey();
                } else
                {
                    Console.WriteLine("Error!");
                }
            } 
        }
        static void UpdateTitle()
        {
            // Actualizează titlul aplicației cu valorile curente pentru "working" și "failed"
            Console.Title = $"xCarpersky - Working: {working} - Failed: {failed}";
        }

        static List<string> LoadProxyList(string fileName)
        {
            List<string> proxyList = new List<string>();

            try
            {
                // Citeste continutul fisierului
                string[] lines = File.ReadAllLines(fileName);

                // Adauga fiecare linie (care contine un proxy) in lista de proxy-uri
                foreach (string line in lines)
                {
                    proxyList.Add(line);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare la citirea fișierului {fileName}: {ex.Message}");
            }

            return proxyList;
        }
    }
}



