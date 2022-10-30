using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.IO;

namespace MegaQuiz
{
    internal class Player
    {
        public string Login { get; private set; }
        public string Password { get; private set; }
        public DateTime Birthday { get; private set; }
        public string Description { get; private set; }
        public int Rank { get; private set; }
        public int Xp { get; private set; }

        public Player()
        {
            Login = "";
            Password = "";
            Birthday = DateTime.Now;
            Description = "";
            Rank = 0;
            Xp = 0;
        }
        public Player(string login, string password, DateTime birthday, string aboutYourself = "")
        {
            Login = login;
            Password = password;
            if (birthday == null)
                Birthday = new DateTime(1960, 1, 1);
            else
                Birthday = birthday;
            Description = aboutYourself;
        }

        public void AddXp(int value)
        {
            if (Rank == 5 && Xp + value >= (10 * Math.Pow(3, Rank)))
            {
                Xp = Convert.ToInt32(10 * Math.Pow(3, Rank));
                return;
            }
            Xp += value;

            Color.WriteLine($"\n░▒▓█ Ви заробили {value} балiв! █▓▒░", ConsoleColor.Yellow);
            Thread.Sleep(1000);

            CheckRank();
        }

        public void Save()
        {
            StreamWriter sw = new StreamWriter($"Players\\{Login}.txt");

            sw.WriteLine($"{Login}\n{Password}\n{Birthday}\n{Rank}\n{Xp}\n{Description}");
            
            sw.Close();
        }

        public static Player Load(string login)
        {
            if (!File.Exists($"Players\\{login}.txt")) return null;

            StreamReader sr = new StreamReader($"Players\\{login}.txt");

            Player player = new Player();
            player.Login = sr.ReadLine();
            player.Password = sr.ReadLine();
            player.Birthday = Convert.ToDateTime(sr.ReadLine());
            player.Rank = Convert.ToInt32(sr.ReadLine());
            player.Xp = Convert.ToInt32(sr.ReadLine());
            player.Description = sr.ReadToEnd();

            sr.Close();

            return player;
        }

        public void PrintColorStat(int buttonChosed, bool setting)
        {
            ConsoleColor col = Color.RankInformation.ElementAt(Rank).Value;
            ConsoleColor col2 = Color.RankInformation2.ElementAt(Rank).Value;

            if (setting)
            {
                Console.SetCursorPosition(50, 3);
                Color.WriteLine($"░▒▓█  Змiнити пароль  █▓▒░", (buttonChosed == 0) ? col2 : ConsoleColor.DarkYellow);
                Console.SetCursorPosition(50, 5);
                Color.WriteLine($"░▒▓█   Змiнити опис   █▓▒░", (buttonChosed == 1) ? col2 : ConsoleColor.DarkYellow);
                Console.SetCursorPosition(50, 7);
                Color.WriteLine($"░▒▓█   Змiнити дату   █▓▒░", (buttonChosed == 2) ? col2 : ConsoleColor.DarkYellow);
                Console.SetCursorPosition(50, 9);
                Color.WriteLine($"░▒▓█      Назад       █▓▒░", (buttonChosed == 3) ? col2 : ConsoleColor.DarkYellow);
            }
            else
            {
                Console.SetCursorPosition(50, 3);
                Color.WriteLine($"░▒▓█       Меню       █▓▒░", (buttonChosed == 0) ? col2 : ConsoleColor.DarkYellow);
                Console.SetCursorPosition(50, 5);
                Color.WriteLine($"░▒▓█   Налаштування   █▓▒░", (buttonChosed == 1) ? col2 : ConsoleColor.DarkYellow);
                Console.SetCursorPosition(50, 7);
                Color.WriteLine($"░▒▓█ Вихiд з аккаунту █▓▒░", (buttonChosed == 2) ? col2 : ConsoleColor.DarkYellow);
            }

            Console.SetCursorPosition(0, 0);

            Color.Write($" ░▒▓█ Статистика користувача ", col2);
            Color.Write($"{Login}", ConsoleColor.DarkYellow);
            Color.WriteLine($" █▓▒░\n", col2);

            Color.WriteLine($"Ранг:   █▓▒░ {Color.RankInformation.ElementAt(Rank).Key} ░▒▓█", col);
            Color.WriteLine($"\nДосвiд: {Xp}/{ 10 * Math.Pow(3, Rank) }", col2);
            Color.WriteLine(String.Format("\nДень народження: {0:d}\n", Birthday), col2);

            Color.WriteLine($"░▒▓█ Про себе █▓▒░", col);
            Color.WriteLine($"\n{Description}", col2);

            Console.SetCursorPosition(0, 0);
        }

        public static string PrintPassword()
        {
            string password = "";

            ConsoleKeyInfo symb = Console.ReadKey(true);
            bool isHide = true;

            while (!(symb.Key == ConsoleKey.Enter && password.Length >= 8) && symb.Key != ConsoleKey.Escape)
            {
                if (symb.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write(symb.KeyChar);
                    Color.Write("█", ConsoleColor.Black);
                    Console.Write(symb.KeyChar);
                }
                else if (Char.IsLetterOrDigit(symb.KeyChar))
                {
                    password += symb.KeyChar;
                    if (isHide) Console.Write('*');
                    else Console.Write(symb.KeyChar);
                }
                else if (symb.Key == ConsoleKey.UpArrow)
                {
                    isHide = !isHide;
                    foreach (var e in password)
                    {
                        Console.Write((char)ConsoleKey.Backspace);
                        Color.Write("█", ConsoleColor.Black);
                        Console.Write((char)ConsoleKey.Backspace);
                    }
                    if (isHide)
                    {
                        foreach (var e in password) Console.Write("*");
                    }
                    else
                    {
                        Console.Write(password);
                    }
                }

                symb = Console.ReadKey(true);
            }
            Console.WriteLine();

            if (symb.Key == ConsoleKey.Escape)
                return "";

            return password;
        }

        public void ChangePassword()
        {
            Color.Write($"░▒▓█ Змiнити пароль █▓▒░\n", ConsoleColor.DarkYellow);
            Console.Write("Минулий пароль: ");
            string old = Player.PrintPassword();
            if (old != Password)
            {
                Color.WriteLine((old != "") ? "Неправильний пароль" : "Вихiд", ConsoleColor.Red);
                return;
            }
            Console.Write("Новий пароль: ");
            string newpass = Player.PrintPassword();
            if (newpass == "")
            {
                Color.WriteLine("Вихiд", ConsoleColor.Red);
                return;
            }

            Console.Write("Повторiть новий пароль: ");
            if (Player.PrintPassword() != newpass)
            {
                Color.WriteLine("Паролi не зiвпали", ConsoleColor.Red);
                return;
            }
            Color.WriteLine("Пароль збережено", ConsoleColor.DarkYellow);
            Password = newpass;
        }

        public void ChangeDescription()
        {
            Color.Write($"░▒▓█ Змiнити опис █▓▒░\n", ConsoleColor.DarkYellow);
            Console.WriteLine("Введiть новий текст: " +
                "\n - для кiнця вводу введiть \"-\" в новому рядку" +
                "\n - використовуйте українську лiтеру \"i\"" +
                "\n - замiсть \"?\" використовуйте \"\\\\\"");
            string newdescription = "";
            string newline = Console.ReadLine();

            while (newline != "-")
            {
                newdescription += "\n" + newline;
                newline = Console.ReadLine();
            }

            if (newdescription.Length > 0)
                Description = newdescription.Substring(1, newdescription.Length - 1).Replace('?', 'i').Replace("\\\\", "?");
            else Description = "";
        }

        public void ChangeDate()
        {
            Color.Write($"░▒▓█ Змiнити дату █▓▒░\n", ConsoleColor.DarkYellow);
            Console.Write("Введiть нову дату (\"dd.mm.yyyy\"): ");
            string newDateStr = Console.ReadLine();
            DateTime dateTime;
            if (!DateTime.TryParse(newDateStr, out dateTime))
                Color.WriteLine("Неправильний формат", ConsoleColor.Red);
            else
            {
                Color.WriteLine("Дата збережена", ConsoleColor.DarkYellow);
                Birthday = dateTime;
            }
        }

        public void CheckRank()
        {
            if (Xp >= (10 * Math.Pow(3, Rank)))
            {
                ++Rank;
                Color.WriteLine($"\n░▒▓█ Ви досягли нового рангу! █▓▒░", Color.RankInformation.ElementAt(Rank).Value);
                Thread.Sleep(1000);
                CheckRank();
            }
            return;
        }
    }
}
