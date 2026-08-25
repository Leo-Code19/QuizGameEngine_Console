using QuizGameEngine_1_.Models;
using QuizGameEngine_1_.Repos;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizGameEngine_1_.Services
{
    public class MenuService
    {
        private readonly InputHelper inputHelper = new();
        private readonly QuizService quizService = new();
        private readonly LeaderboardRepos leaderboardRepos = new LeaderboardRepos();

        public void ShowMenu() // methods that show menu 
        {

            bool Running = true;
            while (Running)
            {
                Console.WriteLine("============================================================================");
                Console.WriteLine("=\t\t\t\tQuiz Game Engine\t\t\t   =     ");
                Console.WriteLine("============================================================================");

                Console.WriteLine("\n1. Start Quiz\n2. View Leaderboard\n3. View Previous Results\n4. Exit");

                int input = inputHelper.isValidSelection(1, 4, "\nSelect an option: "); // get user input and evaluate it using isValidSelection method from inputhelper class and store var input

                switch (input) 
                {
                    case 1:
                        quizService.StartQuiz();
                        break;
                    case 2:
                        ViewLeaderboard();
                        break;
                    case 3:
                        ViewPreviousResult();
                        break;
                    case 4:
                        Running = false;
                        Console.WriteLine("Exiting...");
                        break ;
                }
            if (Running)
                {
                    Console.ReadLine();
                    Console.Clear();
                }
            }

        }

        private void ViewLeaderboard()
        {
            Console.Clear();

            List<QuizResult> results = leaderboardRepos.LoadResults();

            if (results.Count == 0)
            {
                Console.WriteLine("No records found");
                return;
            }
            var leaderboard = results
                .OrderByDescending(r => r.score)
                .ThenByDescending(r => r.Percentage)
                .Take(3)
                .ToList();

            Console.WriteLine("===========================================================================");
            Console.WriteLine("=\t\t\t\t  LEADERBOARD\t\t\t\t  =");
            Console.WriteLine("===========================================================================");
            Console.WriteLine();

            Console.WriteLine($"{"Rank",-6} {"Player",-20} {"Score",-10} {"Category", -20} {"Percentage"}");

            int rank = 1;

            foreach (var result in leaderboard)
            {
                Console.WriteLine(
                    $"{rank,-6} {result.PlayerName,-20} {result.score,-10} {result.category, -20} {result.Percentage:F2}%");

                rank++;
            }
        }

        private void ViewPreviousResult()
        {
            bool back = false;

            while (!back)
            {
                Console.Clear();

                Console.WriteLine("=================================");
                Console.WriteLine("       PREVIOUS RESULTS");
                Console.WriteLine("=================================");
                Console.WriteLine("1. View All Results\n2. Search by Player name\n3. Search by Category\n4. Search by score\n5. Search by Date\n6. Back Menu");

                int input = inputHelper.isValidSelection(1, 6, "Select an option: ");
                switch (input)
                {
                    case 1:
                        Console.Clear();
                        DisplayResults(leaderboardRepos.LoadResults());
                        break;

                    case 2:
                        SearchByPlayerName();
                        break;

                    case 3:
                        SearchByCategory();
                        break;

                    case 4:
                        SearchByScore();
                        break;

                    case 5:
                        SearchByDate();
                        break;

                    case 6:
                        back = true;
                        break;
                }
            if (!back)
                {
                    Console.ReadLine();
                    Console.Clear();
                }
            }
        }

        public void DisplayResults(List<QuizResult> results)
        {
            Console.Clear();

            if (results.Count == 0)
            {
                Console.WriteLine("No results found.");
                return;
            }

            Console.WriteLine("===================================================================================================");
            Console.WriteLine($"{"Player",-15} {"Score",-8} {"Correct",-11} {"%",-8} {"Category",-15} {"Difficulty",-15} {"Completed"}");
            Console.WriteLine("===================================================================================================");

            foreach (var result in results)
            {
                Console.WriteLine(
                    $"{result.PlayerName,-17}" +
                    $"{result.score,-11}" +
                    $"{result.CorrectAnswer,-7}" +
                    $"{result.Percentage:F2}%".PadRight(10) +
                    $"{result.category,-20}" +
                    $"{result.Difficulty,-10}" +
                    $"{result.CompletedAt:g}");
            }
        }
        

        public void SearchByPlayerName()
        {
            Console.Clear();
            var name = inputHelper.isEmptyAndWhitespaces("Enter player name: ");
            var results = leaderboardRepos.LoadResults()
                .Where(r => r.PlayerName.Contains(name ?? "", StringComparison.OrdinalIgnoreCase))
                .ToList();

            DisplayResults(results);
        }

        public void SearchByCategory()
        {
            Console.Clear();
            var choice = inputHelper.isValidSelection(1, 4, "Enter category (1. Programming, 2. Science, 3. Mathematics, 4. General Knowledge): ");

            string category = choice switch
            {
                1 => "Programming",
                2 => "Science",
                3 => "Mathematics",
                4 => "General Knowledge",
                _ => ""
            };

            var results = leaderboardRepos.LoadResults()
                .Where(r => r.category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .ToList();

            DisplayResults(results);
        }

        public void SearchByScore()
        {
            var results = leaderboardRepos.LoadResults()
                .OrderByDescending(r => r.score)
                .ToList();

            DisplayResults(results);
        }

        public void SearchByDate()  
        {
            var results = leaderboardRepos.LoadResults()
                .OrderByDescending(r => r.CompletedAt)
                .ToList();

            DisplayResults(results);
        }

        
    }
}
