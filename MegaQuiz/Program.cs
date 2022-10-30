using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
using System.IO;

namespace MegaQuiz
{
    internal class Program
    {
        public static bool CheckPlayerStat(Player player)
        {
            int chosen = 0;
            ConsoleKey symb;

            bool inSetting = false;

            while (true)
            {
                Console.SetCursorPosition(0, 0);

                player.PrintColorStat(chosen, inSetting);

                symb = Console.ReadKey(true).Key;

                switch (symb)
                {
                    case ConsoleKey.DownArrow:
                        if (++chosen >= (inSetting ? 4 : 3)) chosen = 0;
                        break;
                    case ConsoleKey.UpArrow:
                        if (--chosen < 0) chosen = (inSetting ? 3 : 2);
                        break;
                    case ConsoleKey.Enter:
                        if (inSetting)
                        {
                            Console.Clear();
                            switch (chosen)
                            {
                                case 0:
                                    player.ChangePassword();
                                    Thread.Sleep(1500);
                                    Console.Clear();
                                    break;
                                case 1:
                                    player.ChangeDescription();
                                    Thread.Sleep(500);
                                    Console.Clear();
                                    break;
                                case 2:
                                    player.ChangeDate();
                                    Thread.Sleep(1500);
                                    Console.Clear();
                                    break;
                                case 3:
                                    inSetting = false;
                                    chosen = 0;
                                    break;
                            }
                            player.Save();
                        }
                        else
                        {
                            Console.Clear();
                            switch (chosen)
                            {
                                case 0:
                                    return true;
                                case 1:
                                    inSetting = true;
                                    break;
                                case 2:
                                    return false;
                            }
                        }
                        break;
                }
            }
        }

        public static int WithNoAccount()
        {
            int chosen = 0;
            ConsoleKey symb;
            int ans;

            while (true)
            {
                Console.SetCursorPosition(0, 0);

                Registration.PrintButtons(chosen);

                symb = Console.ReadKey(true).Key;

                switch (symb)
                {
                    case ConsoleKey.DownArrow:
                        if (++chosen >= 2) chosen = 0;
                        break;
                    case ConsoleKey.UpArrow:
                        if (--chosen < 0) chosen = 1;
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();
                        if (chosen == 0)
                            Registration.DoRegistration();
                        else
                        {
                            ans = Registration.DoLogin();
                            if (ans != -1)
                                return ans;
                        }
                        Console.Clear();
                        break;
                }
            }
        }

        //public static void 
        public static void Shuffle<T>(T[] array)
        {
            Random rng = new Random();

            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        static public Quiz LoadQuiz(string name)
        {
            if (!File.Exists(name)) return null;
            StreamReader sr = new StreamReader(name);
            string txt = sr.ReadToEnd();
            sr.Close();

            return JsonSerializer.Deserialize<Quiz>(txt);
        }

        static void Main(string[] args)
        {
            Registration.LoadPlayersFromList();
            
            Player you;

            while (true)
            {
                you = Registration.registeredPlayers[WithNoAccount()];
                you.Save();
                Registration.RefreshPlayersList();
 
                Console.Clear();
                if (Menu.InMenu(you) == -1)
                    return;
            }
        }
    }
}
