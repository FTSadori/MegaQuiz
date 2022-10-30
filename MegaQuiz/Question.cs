using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Threading;

namespace MegaQuiz
{
    internal class Question
    {
        public string Task { get; set; }
        public int Points { get; set; }
        public int OptionsCount { get { return Options.Count; } }
        public Dictionary<string, bool> Options { get; private set; }
        
        public Question()
        {
            Task = "Використай вже нормальний конструктор";
            Points = -69420;
            Options = new Dictionary<string, bool>();
            Options.Add("Звичайно!!!", true);
        }
        public Question(string task, int points, Dictionary<string, bool> options)
        {
            Task = task;
            Points = points;
            Options = new Dictionary<string, bool>();
            Options = Options.Concat(options).ToDictionary(item => item.Key, item => item.Value);
            Options = ShuffleDictionary(Options);
        }

        public static Dictionary<Key, Value> ShuffleDictionary<Key, Value>(Dictionary<Key, Value> d)
        {
            Random rand = new Random();
            return d.OrderBy(x => rand.Next()).ToDictionary(item => item.Key, item => item.Value);
        }

        public int DoQuestion()
        {
            bool[] ans = new bool[OptionsCount];
            for (int i = 0; i < OptionsCount; i++)
                ans[i] = false;

            int chosen = 0;
            ConsoleKey symb;

            while (true)
            {
                ColorPrint(chosen, ans);

                symb = Console.ReadKey(true).Key;

                switch (symb)
                {
                    case ConsoleKey.DownArrow:
                        if (++chosen >= OptionsCount) chosen = 0;
                        break;
                    case ConsoleKey.UpArrow:
                        if (--chosen < 0) chosen = OptionsCount - 1;
                        break;
                    case ConsoleKey.Enter:
                        ans[chosen] = !ans[chosen];
                        break;
                    case ConsoleKey.RightArrow:
                        if (Options.Where(item => item.Value == true).Count() == ans.Where(item => item == true).Count())
                            return CheckAnswers(ans);
                        break;
                }
            }
        }

        public void ColorPrint(int cursorIn, bool[] choosed, bool showAns = false)
        {
            Console.SetCursorPosition(0, 2);

            Console.WriteLine($" ░▒▓█ {Task} █▓▒░\n");

            Color.WriteLine($"░▒ Бали за тест    : {Points}\t▒░", ConsoleColor.Cyan);
            Color.WriteLine($"░▒ Вiрних варiантiв: {Options.Where(item => item.Value == true).Count()}\t▒░", ConsoleColor.Magenta);
        
            for (int i = 0; i < Options.Count; ++i)
            {
                if (!showAns)
                {
                    Color.Write("\n ░▒▓█", (choosed[i]) ? ConsoleColor.Blue : ConsoleColor.White);
                    Color.WriteLine($" {(char)('a' + i)}. {Options.ElementAt(i).Key}",
                        (choosed[i]) ? ConsoleColor.Blue : ConsoleColor.White,
                        (cursorIn == i) ? ConsoleColor.DarkYellow : ConsoleColor.Black);
                }
                else
                {
                    Color.WriteLine($"\n ░▒▓█ {(char)('a' + i)}. {Options.ElementAt(i).Key}",
                        Options.ElementAt(i).Value == true ? ConsoleColor.Green : ConsoleColor.White);
                }
            }
        }

        public int CheckAnswers(bool[] choosed)
        {
            if (Enumerable.Range(0, Options.Count).All(x => choosed[x] == Options.ElementAt(x).Value))
            {
                ColorPrint(-1, choosed, true);
                Thread.Sleep(1000);
                return Points;
            }
            return 0;
        }

        public override string ToString()
        {
            return Task; 
        }
    }
}
