using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaQuiz
{
    internal class Color
    {
        // Вводить чорний рядок, починаючи із символу col рядка row
        public static void DeleteWordFrom(int col, int row, string line)
        {
            Console.SetCursorPosition(col, row);
            foreach (var el in line) Color.Write("█", ConsoleColor.Black);
            Console.SetCursorPosition(col, row);
        }
        // Вводить рядок потрібного кольору шрифта та фоном
        public static void WriteLine(string line, ConsoleColor foregColor, ConsoleColor backgColor = ConsoleColor.Black)
        {
            Console.BackgroundColor = backgColor;
            Console.ForegroundColor = foregColor;
            Console.WriteLine(line);
            Console.ResetColor();
        }
        // Вводить рядок потрібного кольору шрифта та фоном
        public static void Write(string line, ConsoleColor foregColor, ConsoleColor backgColor = ConsoleColor.Black)
        {
            Console.BackgroundColor = backgColor;
            Console.ForegroundColor = foregColor;
            Console.Write(line);
            Console.ResetColor();
        }

        // Головний колір рангу
        public static Dictionary<char, ConsoleColor> RankInformation = new Dictionary<char, ConsoleColor>() {
            { 'E', ConsoleColor.DarkGray },
            { 'D', ConsoleColor.White },
            { 'C', ConsoleColor.Green },
            { 'B', ConsoleColor.Blue },
            { 'A', ConsoleColor.DarkMagenta },
            { 'S', ConsoleColor.DarkYellow },
        };

        // Додатковий колір рангу
        public static Dictionary<char, ConsoleColor> RankInformation2 = new Dictionary<char, ConsoleColor>() {
            { 'E', ConsoleColor.Gray },
            { 'D', ConsoleColor.Gray },
            { 'C', ConsoleColor.Yellow },
            { 'B', ConsoleColor.Cyan },
            { 'A', ConsoleColor.Magenta },
            { 'S', ConsoleColor.Yellow },
        };
    }
}
