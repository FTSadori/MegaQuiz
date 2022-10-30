using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace MegaQuiz
{
    internal class Quiz
    {
        public string Topic { get; set; }
        public int MaxPoints { get { return (Questions.Count == 0) ? 0 : (Math.Min(20, Questions.Count) * Questions[0].Points); } }
        public Dictionary<string, int> PlayersPoints { get; private set; }
        public List<Question> Questions { get; private set; }

        public Quiz()
        {
            Topic = "";
            PlayersPoints = new Dictionary<string, int>();
            Questions = new List<Question>();
        }
        public Quiz(string topic, Dictionary<string, int> playersPoints, List<Question> questions)
        {
            PlayersPoints = new Dictionary<string, int>();
            Questions = new List<Question>();

            PlayersPoints = PlayersPoints.Concat(playersPoints).ToDictionary(item => item.Key, item => item.Value);
            Topic = topic;
            Questions = Questions.Concat(questions).ToList();
        }

        public void AddQuestion(Question question)
        {
            Questions.Add(question);
        }

        // Додати статистику у топ
        public void AddPlayerStat(string name, int points)
        {
            if (PlayersPoints.ContainsKey(name))
                PlayersPoints[name] = Math.Max(points, PlayersPoints[name]);
            else
                PlayersPoints[name] = points;
            PlayersPoints = (from entry in PlayersPoints orderby entry.Value descending select entry).ToDictionary(item => item.Key, item => item.Value);
        }

        // Зберегти вікторину у створений для неї файл
        public void Save()
        {
            StreamWriter sw = new StreamWriter($"{Topic}.txt");

            sw.WriteLine(Topic);
            sw.WriteLine(JsonSerializer.Serialize(PlayersPoints));
            foreach (Question q in Questions)
            {
                sw.WriteLine(q.Task);
                sw.WriteLine(q.Points);
                sw.WriteLine(JsonSerializer.Serialize(q.Options));
            }

            sw.Close();
        }

        // Оновити Mega Quiz (необхідно після оновлення/додавання інших вікторин)
        public static void RedoMegaQuiz()
        {
            Quiz quiz = Load("Mega Quiz");
            quiz.Questions = new List<Question>();

            foreach (string n in Menu.names)
            {
                if (n == quiz.Topic) continue;

                quiz.Questions = quiz.Questions.Concat(Quiz.Load(n).Questions).ToList();
            }

            quiz.Save();
        }

        // Загрузити вікторину із файлу (вводиться назва вікторини)
        public static Quiz Load(string quizName)
        {
            Quiz quiz = new Quiz();

            if (!File.Exists($"{quizName}.txt")) return null;

            StreamReader sr = new StreamReader($"{quizName}.txt");

            quiz.Topic = sr.ReadLine();
            quiz.PlayersPoints = JsonSerializer.Deserialize<Dictionary<string, int>>(sr.ReadLine());
            while (!sr.EndOfStream)
            {
                string task = sr.ReadLine();
                int point = Convert.ToInt32(sr.ReadLine());
                quiz.AddQuestion(new Question(task, point, JsonSerializer.Deserialize<Dictionary<string, bool>>(sr.ReadLine())));
            }

            sr.Close();

            return quiz;
        }

        // Виконання вікторини. Функція викликає функції Question.DoQuestion() для кожного питання в массиві
        public int DoQuiz(string playerName)
        {
            Question[] questions = Questions.ToArray();
            Program.Shuffle(questions);
            int sum = 0;

            for (int i = 0; i < Math.Min(questions.Length, 20); ++i)
            {
                Console.Clear();
                Color.Write($"█▓▒░ Вiкторина на тему ", ConsoleColor.Yellow);
                Color.Write($"{Topic}", ConsoleColor.DarkYellow);
                Color.WriteLine($" ({i + 1}/{Math.Min(questions.Length, 20)}) ░▒▓█\n", ConsoleColor.Yellow);

                sum += questions[i].DoQuestion();
            }

            AddPlayerStat(playerName, sum);
            return sum;
        }

        // Вивід топа 20
        public void ColorPrintTop20(string yourName)
        {
            Color.Write($"█▓▒░ Статистика вiкторини на тему ", ConsoleColor.Yellow);
            Color.Write($"{Topic}", ConsoleColor.DarkYellow);
            Color.WriteLine($" ░▒▓█\n", ConsoleColor.Yellow);

            for (int i = 0; i < Math.Min(PlayersPoints.Count, 20); ++i)
            {
                ConsoleColor color = ConsoleColor.White;

                if (PlayersPoints.ElementAt(i).Key == yourName)
                    color = ConsoleColor.Magenta;
                else if (PlayersPoints.ElementAt(i).Value == MaxPoints)
                    color = ConsoleColor.DarkYellow;

                Color.WriteLine($"░▒▓█ "
                    + String.Format("{0, -3} {1,-15} {2,-4} █▓▒░", i + 1, PlayersPoints.ElementAt(i).Key, PlayersPoints.ElementAt(i).Value),
                    color);
            }
        }
    }
}
