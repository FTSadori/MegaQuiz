using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace MegaQuiz
{
    internal static class Menu
    {
        public const int cols = 100;
        public const int rows = 40;
        public const string quizNamesFile = "quizNames.txt";
        public static List<string> names;

        // Заповнює список назв вікторин із файлу
        static Menu()
        {
            if (!File.Exists(quizNamesFile)) return;

            StreamReader sr = new StreamReader(quizNamesFile);
            names = new List<string>();

            while (!sr.EndOfStream) names.Add(sr.ReadLine());

            sr.Close();
        }

        public static void PrintQuizMenu(int page, int choise)
        {
            Console.SetCursorPosition(25, 6);
            Color.WriteLine($" -====###### Список вiкторин (стр. {page + 1}) ######====-", ConsoleColor.Yellow);

            int start = 10 * page;
            for (int i = 0; i < Math.Min(names.Count - 10 * page, names.Count); ++i)
            {
                Console.SetCursorPosition(30, 8 + i * 2);
                if (names[start + i] == "Mega Quiz")
                    Color.WriteLine(String.Format("-=##@@ {0, -20} --- @@##=-", names[start + i], Quiz.Load(names[start + i]).MaxPoints),
                 (choise == i) ? ConsoleColor.DarkMagenta : ConsoleColor.Magenta);
                else
                Color.WriteLine(String.Format("-=##@@ {0, -20} {1, -3} @@##=-", names[start + i], Quiz.Load(names[start + i]).MaxPoints),
                     (choise == i) ? ConsoleColor.Cyan : ConsoleColor.White);
            }
        }

        public static void DoQuizMenu(Player you)
        {
            if (names.Count == 0) return;

            ColorPrint(false);

            int maxPage = names.Count / 10 + ((names.Count % 10 != 0) ? 1 : 0);
            int chosen = 0;
            int page = 0;
            int maxOnPage = Math.Min(names.Count, 10);

            ConsoleKey symb;

            while (true)
            {
                PrintQuizMenu(page, chosen);

                symb = Console.ReadKey(true).Key;

                switch (symb)
                {
                    case ConsoleKey.DownArrow:
                        if (++chosen >= maxOnPage) chosen = maxOnPage - 1;
                        break;
                    case ConsoleKey.UpArrow:
                        if (--chosen < 0) chosen = 0;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (--page < 0)
                            page = 0;

                        if (page == maxPage - 1)
                            maxOnPage = ((names.Count % 10 == 0) ? 10 : names.Count % 10);
                        else
                            maxOnPage = 10;
                        break;
                    case ConsoleKey.RightArrow:
                        if (++page >= maxPage)
                            page = maxPage - 1;

                        if (page == maxPage - 1)
                            maxOnPage = ((names.Count % 10 == 0) ? 10 : names.Count % 10);
                        else
                            maxOnPage = 10; 
                        break;
                    case ConsoleKey.Enter:
                        Console.Clear();

                        Quiz quiz = Quiz.Load(names[chosen]);
                        you.AddXp(quiz.DoQuiz(you.Login));
                        
                        Console.Clear();
                        quiz.ColorPrintTop20(you.Login);

                        symb = Console.ReadKey(true).Key;

                        you.Save();
                        quiz.Save();
                        return;
                    case ConsoleKey.Escape:
                        return;
                }
            }
        }
        // Дуже гарний вивід меню
        public static void ColorPrint(bool printLogo)
        {
            Console.SetWindowSize(cols, rows);

            if (!(File.Exists("Logo.txt") && File.Exists("Tent.txt"))) return;
            StreamReader sr = new StreamReader("Logo.txt");
            string[] logo = sr.ReadToEnd().Split('\n');
            int logoSize = logo.Length;
            sr.Close();

            sr = new StreamReader("Tent.txt");
            string tent = sr.ReadToEnd();
            sr.Close();


            Color.WriteLine(tent, ConsoleColor.DarkYellow);

            if (!printLogo) return;

            for (int i = 0; i < logoSize; ++i)
            {
                Console.SetCursorPosition(15, 9 + i);
                Color.WriteLine(logo[i], ConsoleColor.Yellow);
            }

            ColorPrintButtons(0);
        }

        public static void ColorPrintButtons(int choise)
        {
            Console.SetCursorPosition(34, 20);
            Color.WriteLine($"==####@@      Грати     @@####==", (choise == 0) ? ConsoleColor.DarkYellow : ConsoleColor.Yellow);

            Console.SetCursorPosition(34, 22); 
            Color.WriteLine($"==####@@     Аккаунт    @@####==", (choise == 1) ? ConsoleColor.DarkYellow : ConsoleColor.Yellow);

            Console.SetCursorPosition(34, 24);
            Color.WriteLine($"==####@@    Лiдерборд   @@####==", (choise == 2) ? ConsoleColor.DarkYellow : ConsoleColor.Yellow);

            Console.SetCursorPosition(34, 26);
            Color.WriteLine($"==####@@      Вихiд     @@####==", (choise == 3) ? ConsoleColor.DarkYellow : ConsoleColor.Yellow);
        }

        public static int InMenu(Player you)
        {
            int chosen = 0;
            ConsoleKey symb;

            Console.SetCursorPosition(0, 0);
            ColorPrint(true);
            while (true)
            {
                ColorPrintButtons(chosen);

                symb = Console.ReadKey(true).Key;

                Console.SetCursorPosition(0, 0);
                switch (symb)
                {
                    case ConsoleKey.DownArrow:
                        if (++chosen >= 4) chosen = 0;
                        break;
                    case ConsoleKey.UpArrow:
                        if (--chosen < 0) chosen = 3;
                        break;
                    case ConsoleKey.Enter:
                        switch (chosen)
                        {
                            case 0:
                                DoQuizMenu(you);
                                Registration.registeredPlayers.Sort((a, b) => {
                                    if (a.Xp > b.Xp) return -1;
                                    if (a.Xp < b.Xp) return 1;
                                    return 0;
                                });
                                ColorPrint(true);
                                break;
                            case 1:
                                Console.Clear();
                                if (!Program.CheckPlayerStat(you))
                                    return 0;
                                break;
                            case 2:
                                ColorPrintLeaderBoard(you.Login);
                                break;
                            case 3:
                                Console.Clear();
                                return -1;
                        }
                        Console.SetCursorPosition(0, 0);
                        ColorPrint(true);
                        break;
                }
            }
        }

        public static void ColorPrintLeaderBoard(string yourNick)
        {
            ColorPrint(false);

            Console.SetCursorPosition(25, 4);
            Color.WriteLine(" -====###### Загальний  лiдерборд ######====-", ConsoleColor.Yellow);

            for (int i = 0; i < Math.Min(Registration.registeredPlayers.Count, 20); ++i)
            {
                Console.SetCursorPosition(25, 5 + i);

                PrintPlayerStat(i);
            }

            Console.SetCursorPosition(25, 27);
            Color.WriteLine(" -====##### Ваше положення в топi ######====-", ConsoleColor.Yellow);
            Console.SetCursorPosition(25, 28);
            int iii = Registration.registeredPlayers.IndexOf(Registration.registeredPlayers.Where(x => x.Login == yourNick).FirstOrDefault());
            PrintPlayerStat(iii);

            ConsoleKey symb = Console.ReadKey(true).Key;
            
            while (symb != ConsoleKey.Escape)
                symb = Console.ReadKey(true).Key;
        }

        public static void PrintPlayerStat(int i)
        {
            Player player = Registration.registeredPlayers[i];
            char rank = Color.RankInformation.ElementAt(player.Rank).Key;

            Color.WriteLine(String.Format("==####@@ {0, -3} {1} {2,-15} {3,-6} @@####==", i + 1,
                rank,
                player.Login.Substring(0, Math.Min(15, player.Login.Length)),
                player.Xp),
                Color.RankInformation[rank]);
        }
    }
}
