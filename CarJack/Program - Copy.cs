using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CarJack
{
    class Program
    {
        static List<string> words = new List<string>();

        static bool checkWord(string a)
        {
            foreach (string i in words)
                if (String.Compare(a, i) == 0)
                    return true;             
            return false;
        }


        private static void Swap(ref char a, ref char b)
        {
            if (a == b) return;

            a ^= b;
            b ^= a;
            a ^= b;
        }

        public static void GetPer(char[] list, ref string mirror)
        {
            int x = list.Length - 1;
            GetPer(list, ref mirror , 0, x);
        }

        private static void GetPer(char[] list, ref string mirror, int k, int m)
        {
            string temp = new string(list);
            if (k == m)
            {
                if (checkWord(temp))
                    mirror = temp;
            }
            else
                for (int i = k; i <= m; i++)
                {
                    Swap(ref list[k], ref list[i]);
                    GetPer(list, ref mirror, k + 1, m);
                    Swap(ref list[k], ref list[i]);
                }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Loading dictionary 'english.txt'");
            
            using (StreamReader sr = new StreamReader("custom.txt"))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                    words.Add(line);
            }
            Console.WriteLine("Dictionary loaded successfully!");

            VAMemory vam = new VAMemory("gta_sa");
            IntPtr address_hot = (IntPtr)0x16AE4E59; //0468C0F
            IntPtr address_chat = (IntPtr)0x0455C0F6; //04AFC0F6
            bool check = vam.CheckProcess();
            if (check == true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("gta_sa.exe process found!");
                Console.ForegroundColor = ConsoleColor.White;

                Char delimiter = '~';

                //Thread t = new Thread(new ThreadStart(thread));
                string a = vam.ReadStringASCII(address_hot, 64).Split(delimiter)[0];
                string word = a;
                Console.WriteLine("Word address found, scramble: " + word);
                GetPer(a.ToCharArray(), ref word);
                if (word == a && !checkWord(word))
                {
                    Console.WriteLine("ERROR!!! Could not find word for scramble: " + a);
                }
                else
                {
                    Console.WriteLine("Word found:" + word);
                    vam.WriteStringASCII(address_chat, "|  " + word + "  |    ");
                }

                while (true)
                {
                    string b = vam.ReadStringASCII(address_hot, 64).Split(delimiter)[0];
                    if (b == a)
                        continue;

                    a = b;
                    word = a;
                    Console.WriteLine("Word changed, scramble: " + word);
                    GetPer(a.ToCharArray(), ref word);

                    if (word == a && !checkWord(word))
                    {
                        Console.WriteLine("ERROR!!! Could not find word for scramble: " + a);
                        continue;
                    }

                    Console.WriteLine(word);
                    vam.WriteStringASCII(address_chat, "|  " + word + "  |    ");
                    System.Threading.Thread.Sleep(200);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("gta_sa.exe not found!");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}