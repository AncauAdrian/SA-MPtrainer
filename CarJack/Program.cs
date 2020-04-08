using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;

namespace CarJack
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        public static bool IsAnagram(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
                return false;
            if (s1.Length != s2.Length)
                return false;

            foreach (char c in s2)
            {
                int ix = s1.IndexOf(c);
                if (ix >= 0)
                    s1 = s1.Remove(ix, 1);
                else
                    return false;
            }   

            return string.IsNullOrEmpty(s1);
        }

        public static string FindWord(string word, ref List<string> words)
        {       
            foreach (string s in words)
            {
                if (IsAnagram(word, s))
                    return s;
            }
            return null;
        }

        public static IntPtr GetModuleAddress(string process, string module)
        {
            Process[] p = Process.GetProcessesByName(process);
            if (p.Count() == 0)
                return IntPtr.Zero;

            Process mine = p[0];

            foreach (ProcessModule Module in mine.Modules)
            {
                if (Module.ModuleName.Contains(module))
                {
                    Console.WriteLine((int)Module.BaseAddress);
                    return Module.BaseAddress;
                }
            }

            return IntPtr.Zero;
        }

        public static bool SendString(string s, string process)
        {
            const int WM_CHAR = 0x0102;
            const int WM_KEYUP = 0x0101;
            const int WM_KEYDOWN = 0x0100;

            Process[] p = Process.GetProcessesByName(process);
            if (p.Count() == 0)
                return false;

            foreach (char c in s)
            {
                PostMessage(p[0].MainWindowHandle, WM_CHAR, (int)c, 0);
            }
            return true;
        }

        public static void Thread2(ref VAMemory vam, ref List<string> words)
        {
            //Console.WriteLine("Thread2 -- Enter the address of the word:");
            //string s = Console.ReadLine();
            //int x = 0;
            //Int32.TryParse(s, out x);
            IntPtr last = (IntPtr)0x969110;  // Address to an array that stores player key presses
            bool check = false;
            string a = null;
            string word = null;

            Console.WriteLine("Thread2 Started");
            Process[] p = Process.GetProcessesByName("gta_sa");
            if (p.Count() == 0)
                return;

            IntPtr sampDLL = GetModuleAddress("gta_sa", "samp.dll");    // get base address of samp.dll module
            sampDLL += 0x17FBB8;                                        // adds the offset to find the charInputBox array

            string read = vam.ReadStringASCII(sampDLL, 12);             // reads the characters

            while (true)
            {
                if (vam.ReadByte(last) == '9')
                {
                    vam.WriteByte(last, 66);
                    if (check)
                    {
                        check = false;
                        Console.WriteLine("Hotwire false");
                        continue;
                    }
                    else
                    {
                        check = true;
                        Console.WriteLine("Hotwire true");
                    }
                }

                if (!check)
                    continue;

                a = vam.ReadStringASCII(sampDLL, 16);

                if (a[0] != '/' || a == "")
                {
                    continue;
                }
                a = a.Trim('\0');
                if (a[a.Length - 1] != '0')
                    continue;
                a = a.Split('/')[1];
                a = a.Trim('0');

                if (a.Length > 9)
                {
                    Console.WriteLine("Hotwire - String too big");
                    continue;
                }

                Console.WriteLine("Hotwire - New word found -- Scramble: " + a);
                word = FindWord(a, ref words);

                if (word == null)
                {
                    Console.WriteLine("Hotwire - Found solution: " + word);
                    string backspace = "";
                    for (int i = 0; i < a.Length + 4; i++)
                        backspace += (char)8;

                    SendString(backspace, "gta_sa");
                    word = "/uns " + a;
                    check = SendString(word, "gta_sa");

                    
                    using (StreamWriter sr = new StreamWriter("unknown.txt"))
                    {
                        sr.WriteLine(a);
                    }
                }
                else
                {
                    Console.WriteLine("Hotwire - Found solution: " + word);
                    string backspace = "";
                    for (int i = 0; i < a.Length + 4; i++)
                        backspace += (char)8;

                    SendString(backspace, "gta_sa");
                    word = "/uns " + word;
                    check = SendString(word, "gta_sa");
                }
                Thread.Sleep(200);
            }
            Console.WriteLine("Hotwire - Thread 2 ended");
        }

        public static unsafe void lastThread2(ref VAMemory vam, ref List<string> words)
        {
            //Console.WriteLine("Thread2 -- Enter the address of the word:");
            //string s = Console.ReadLine();
            //int x = 0;
            //Int32.TryParse(s, out x);
            IntPtr address_hot = (IntPtr)0x1697B9A1; //20A5EF00 167B0BB1
            IntPtr last = (IntPtr)0x969110;
            bool check = false;
            string a = null;
            string b = null;
            string word = null;

            Console.WriteLine("Thread2 Started");
            Process[] p = Process.GetProcessesByName("gta_sa");
            if (p.Count() == 0)
                return;

            IntPtr sampDLL = GetModuleAddress("gta_sa", "samp.dll");
            sampDLL += 0x2ACA14;

            IntPtr lastCmd = (IntPtr) vam.ReadInt32(sampDLL);
            lastCmd += 0x14E5;

            while (true)
            {
                if (vam.ReadByte(last) == '9')
                {
                    vam.WriteByte(last, 66);
                    if (check)
                    {
                        check = false;
                        Console.WriteLine("Hotwire false");
                        continue;
                    }
                    else
                    {
                        check = true;
                        Console.WriteLine("Hotwire true");
                    }
                }

                if (!check)
                    continue;

                a = vam.ReadStringASCII(lastCmd, 9).Trim('\0');

                if (a[0] == '/')
                    continue;

                vam.WriteStringASCII(lastCmd, "/");


                if (a.Length > 9)
                {
                    Console.WriteLine("Hotwire - String too big");
                    break;
                }

                if (a == b && b != null)
                    continue;

                b = a;
                Console.WriteLine("Hotwire - New word found -- Scramble: " + a);
                word = FindWord(a, ref words);

                if (word == null)
                {
                    Console.WriteLine("Hotwire - No solution found");
                    word = "TERROR";
                    check = SendString(word, "gta_sa");
                }
                else
                {
                    Console.WriteLine("Hotwire - Found solution: " + word);
                    word = "t/uns " + word;
                    check = SendString(word, "gta_sa");
                }
                Thread.Sleep(200);
            }
            Console.WriteLine("Hotwire - Thread 2 ended");
        }

        public static void oldThread2(ref VAMemory vam, ref List<string> words)
        {
            //Console.WriteLine("Thread2 -- Enter the address of the word:");
            //string s = Console.ReadLine();
            //int x = 0;
            //Int32.TryParse(s, out x);
            IntPtr address_hot = (IntPtr)0x1697B9A1; //20A5EF00 167B0BB1
            IntPtr last = (IntPtr)0x969110;
            char delimiter = '~';
            bool check = false;
            string a = null;
            string b = null;
            string word = null;

            Console.WriteLine("Thread2 Started");
            Process[] p = Process.GetProcessesByName("gta_sa");
            if (p.Count() == 0)
                return;

            while (true)
            {
                if (vam.ReadByte(last) == 'O')
                {
                    vam.WriteByte(last, 66);
                    if (check)
                    {
                        check = false;
                        Console.WriteLine("Hotwire false");
                        continue;
                    }
                    else
                    {
                        check = true;
                        Console.WriteLine("Hotwire true");
                    }
                }

                if (!check)
                    continue;

                //if (vam.ReadByte(last) == '2')
                //{
                //    a = vam.ReadStringASCII(last, 10);
                //    a = a.Split('1')[0];
                //    a = a.Split('2')[1];
                //    char[] temp = a.ToCharArray();
                //    Array.Reverse(temp);
                //    a = new string(temp);
                //    a = a.ToLower();
                //    Console.WriteLine(a);
                //    vam.WriteByte(last, 66);
                //}
                //else continue;

                a = vam.ReadStringASCII(address_hot, 16);
                if (!a.Contains<char>('~') || a == "")
                {
                    Console.WriteLine("Hotwire - Could not find the string");
                    break;
                }

                a = a.Split(delimiter)[0];
                if (a.Length > 9)
                {
                    Console.WriteLine("Hotwire - String too big");
                    break;
                }

                if (a == b && b != null)
                    continue;

                b = a;
                Console.WriteLine("Hotwire - New word found -- Scramble: " + a);
                word = FindWord(a, ref words);

                if (word == null)
                {
                    Console.WriteLine("Hotwire - No solution found");
                    word = "TERROR";
                    check = SendString(word, "gta_sa");
                }
                else
                {
                    Console.WriteLine("Hotwire - Found solution: " + word);
                    word = "t/uns " + word;
                    check = SendString(word, "gta_sa");                  
                }
                Thread.Sleep(200);
            }
            Console.WriteLine("Hotwire - Thread 2 ended");
        }

        public static void Thread1(ref VAMemory vam)
        {
            IntPtr last = (IntPtr)0x969110;
            IntPtr cars = (IntPtr)0x96914B;
            Console.WriteLine("Thread1 Started");
            while (true)
            {
                if (vam.ReadByte(last) == '0')
                {
                    vam.WriteByte(last, 66);
                    if (vam.ReadBoolean(cars))
                    {
                        vam.WriteBoolean(cars, false);
                        Console.WriteLine("Invisible Cars - Deactiv");
                    }
                    else
                    {
                        vam.WriteBoolean(cars, true);
                        Console.WriteLine("Invisible Cars - Activ");
                    }
                }
                Thread.Sleep(50);
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Loading dictionary 'english.txt'");
            List<string> words = new List<string>();

            using (StreamReader sr = new StreamReader("custom.txt"))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                    words.Add(line);
            }

            using (StreamReader sr = new StreamReader("english.txt"))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                    words.Add(line);
            }
            Console.WriteLine("English Dictionary loaded successfully!");

            VAMemory vam = new VAMemory("gta_sa");

            if(!vam.CheckProcess())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("gta_sa.exe not found!");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("gta_sa.exe process found!");
            Console.ForegroundColor = ConsoleColor.White;

            Thread t1 = new Thread(() => Thread1(ref vam));
            t1.Start();
            Thread t2 = new Thread(() => Thread2(ref vam, ref words));
            t2.Start();

            Console.WriteLine("Sa-Mp script started! Press any key to stop.");



            Console.ReadKey();
            t1.Abort();
            t2.Abort();
        }
    }
}