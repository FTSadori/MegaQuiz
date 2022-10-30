using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace MegaQuiz
{
    internal class Registration
    {
        public static List<Player> registeredPlayers = new List<Player>();

        public static string AllPlayersLogins = "logins.txt";
        public static void PrintButtons(int choise)
        {
            Console.WriteLine("░▒▓█   Вхiд до аккаунту   █▓▒░\n");
           
            Color.WriteLine($"   ░▒▓█   Реєстрацiя   █▓▒░\n", (choise == 0) ? ConsoleColor.Green : ConsoleColor.Gray);
            Color.WriteLine($"   ░▒▓█      Вхiд      █▓▒░", (choise == 1) ? ConsoleColor.Green : ConsoleColor.Gray);

            Color.WriteLine("\nIнструкцiя: " +
                            "\n - Використовуйте кнопки стрiлок для вибору" +
                            "\n - Натиснiть Enter для переходу" +
                            "\n - Використовуйте стрiлку вправо для переходу на наступне питання вiкторини" +
                            "\n - Для виходу iз меню Лiдерборд та Грати, натиснiть Escape", ConsoleColor.Yellow);

        }

        public static void LoadPlayersFromList()
        {
            if (!File.Exists(AllPlayersLogins)) return;

            registeredPlayers = new List<Player>();
            StreamReader sr = new StreamReader(AllPlayersLogins);

            string name;
            while (!sr.EndOfStream)
            {
                name = sr.ReadLine();
                registeredPlayers.Add(Player.Load(name));
            }
            registeredPlayers.Sort((a, b) => { 
                if (a.Xp > b.Xp) return -1;
                if (a.Xp < b.Xp) return 1;
                return 0;
            });

            sr.Close();
        }

        public static void RefreshPlayersList()
        {
            StreamWriter sw = new StreamWriter(AllPlayersLogins);

            foreach (Player player in registeredPlayers)
                sw.WriteLine(player.Login);

            sw.Close();
        }

        // Процес входу в аккаунт
        public static int DoLogin()
        {
            Console.WriteLine("░▒▓█   Вхiд   █▓▒░\n");

            Color.WriteLine($"Логiн:            ", ConsoleColor.DarkYellow);
            Color.WriteLine($"Пароль:           ", ConsoleColor.DarkYellow);
            Color.WriteLine("\n - Для виходу введiть порожнiй логiн або ESC поки вводите пароль" +
                "\n - Пароль має мiстити як мiнiмум 8 символiв" +
                "\n - Для вводу рядка натиснiть Enter", ConsoleColor.Yellow);

            Console.SetCursorPosition(18, 2);
            string login = Console.ReadLine();
            if (login.Length == 0)
                return -1;
            Console.SetCursorPosition(18, 3);
            string password = Player.PrintPassword();
            if (password.Length == 0)
                return -1;

            for (int i = 0; i < registeredPlayers.Count; ++i)
            {
                if (registeredPlayers[i].Password == password && registeredPlayers[i].Login == login)
                    return i;
            }
            Console.SetCursorPosition(0, 4);
            Color.WriteLine("Невiрний логiн/пароль", ConsoleColor.Red);
            Thread.Sleep(1000);

            return -1;
        }
        // Процес реєстрації
        public static void DoRegistration()
        {
            Console.WriteLine("░▒▓█   Реєстрацiя   █▓▒░\n");

            Color.WriteLine($"Логiн:            ", ConsoleColor.DarkYellow);
            Color.WriteLine($"Пароль:           ", ConsoleColor.DarkYellow);
            Color.WriteLine($"Повторiть пароль: ", ConsoleColor.DarkYellow);
            Color.WriteLine($"Дата народження:  ", ConsoleColor.DarkYellow);

            Console.SetCursorPosition(0, 8);
            Color.WriteLine("\n - Для виходу введiть порожнiй логiн або ESC поки вводите пароль" +
                "\n - Пароль має мiстити як мiнiмум 8 символiв" +
                "\n - Для вводу рядка натиснiть Enter", ConsoleColor.Yellow);

            Console.SetCursorPosition(18, 2);
            string login = Console.ReadLine();
            while (!registeredPlayers.All(pl => pl.Login != login) 
                || login.Length == 0
                || !login.All(c => "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".Contains(c)))
            {
                if (login.Length == 0)
                    return;

                Console.SetCursorPosition(0, 7);
                if (!registeredPlayers.All(pl => pl.Login != login))
                    Color.WriteLine("Такий логiн вже iснує", ConsoleColor.Red);
                else
                    Color.WriteLine("Використовуйте лише латинський алфавiт", ConsoleColor.Red);
                Thread.Sleep(1000);
                Color.DeleteWordFrom(0, 7, "Використовуйте лише латинський алфавiт");
                Color.DeleteWordFrom(18, 2, login);
                login = Console.ReadLine();
            }
            Console.SetCursorPosition(18, 3);
            string pass = Player.PrintPassword();

            if (pass.Length == 0) return;

            Console.SetCursorPosition(18, 4);
            string pass2 = Player.PrintPassword();

            if (pass2.Length == 0) return;

            while (!(pass == pass2))
            {
                if (pass2.Length == 0) return;

                Console.SetCursorPosition(0, 7);
                Color.WriteLine("Паролi не зiвпали", ConsoleColor.Red);
                Thread.Sleep(1000);
                Color.DeleteWordFrom(0, 7, "Паролi не зiвпали");
                Color.DeleteWordFrom(18, 4, pass2);
                pass2 = Player.PrintPassword();
            }

            Console.SetCursorPosition(18, 5);
            string date = Console.ReadLine();
            DateTime dateTime;

            while (!DateTime.TryParse(date, out dateTime))
            {
                Console.SetCursorPosition(0, 7);
                Color.WriteLine("Неправильний формат", ConsoleColor.Red);
                Thread.Sleep(1000);
                Color.DeleteWordFrom(0, 7, "Неправильний формат");
                Color.DeleteWordFrom(18, 5, date);
                date = Console.ReadLine();
            }

            Player newPlayer = new Player(login, pass, dateTime);

            registeredPlayers.Add(newPlayer);
        }
    }
}
