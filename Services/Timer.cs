using QuizGameEngine_1_.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace QuizGameEngine_1_.Services
{
    public class Timer
    {

        public int GetTimed(LevelDifficulty difficulty)
        {
            return difficulty switch
            {
                LevelDifficulty.Easy => 15,
                LevelDifficulty.Medium => 30,
                LevelDifficulty.Hard => 60,
                _ => 30
            };
        }

        public char? ReadAnswerWithTimer(int seconds, int numberOfChoices, Stopwatch stopwatch)
        {
            string input = "";
            Console.Write("\nYour Answer: ");

            while (stopwatch.Elapsed.TotalSeconds < seconds)
            {
                // Display remaining time
                TimeSpan remaining = TimeSpan.FromSeconds(seconds) - stopwatch.Elapsed;

                int timerline = Console.CursorTop;
                Console.SetCursorPosition(0, timerline);
                Console.Write($"Your Answer ({remaining.Seconds:00}.{remaining.Milliseconds / 100:0}s): {input}");

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.Enter)
                    {
                        if (input.Length == 0)
                        {
                            continue;
                        }
                        char answer = input[0];

                        // Lifeline
                        if (answer == '1' || answer == '2' || answer == '3')
                        {
                            Console.WriteLine();
                            return answer;
                        }

                        if (answer >= 'A' && answer < 'A' + numberOfChoices)
                        {
                            Console.WriteLine();
                            return answer;
                        }


                        Console.WriteLine("\nInvalid input. Please enter valid choices");
                        input = ""; 
                        continue;
                    }

                    char ch = char.ToUpper(key.KeyChar);
                    // Ignore spaces and Enter
                    if (char.IsWhiteSpace(ch))
                        continue;

                    input = ch.ToString();
                }

                Thread.Sleep(50);
            }

            Console.WriteLine("\n\nTime's Up!");
            return null;
        }
    }
}
