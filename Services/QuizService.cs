using QuizGameEngine_1_.Enum;
using QuizGameEngine_1_.Models;
using QuizGameEngine_1_.Repos;
using QuizGameEngine_1_.Services;
using System.Diagnostics;


namespace QuizGameEngine_1_.Services
{
    public class QuizService
    {
        private readonly QuestionRepos questionRepos = new QuestionRepos();
        private readonly InputHelper inputHelper = new InputHelper();
        private readonly LeaderboardRepos leaderboardRepos = new LeaderboardRepos();
        private readonly Timer timer = new Timer();
        private readonly QuestionReviewService questionReviewService = new QuestionReviewService();

        public void StartQuiz()
        {
            var allQuestion = questionRepos.LoadQuestions();

            if (allQuestion.Count == 0)
            {
                Console.WriteLine("No available question");
                return;
            }
            Console.Clear();
            string PlayerName = inputHelper.isEmptyAndWhitespaces("Enter Your Name: ");

            Console.WriteLine("\n1. Programming");
            Console.WriteLine("2. Science");
            Console.WriteLine("3. Mathematics");
            Console.WriteLine("4. Genearal Knowledge");
            Console.WriteLine("5. Mixed Categories");
            int categoryChoice = inputHelper.isValidSelection(1, 5, "\nSelect Category: ");

            Console.WriteLine("\n1. Easy");
            Console.WriteLine("2. Medium");
            Console.WriteLine("3. Hard");
            Console.WriteLine("4. Mixed Difficulty");
            int difficultyChoice = inputHelper.isValidSelection(1, 4, "\nSelect Difficulty: ");

            Console.WriteLine("\n1. 5 Questions");
            Console.WriteLine("2. 10 Questions");
            Console.WriteLine("3. 20 Questions");
            int NumberOfQuestions = inputHelper.isValidSelection(1, 3, "\nSelect Number of Questions: ");

            int NumberOfQuestionsChoice = NumberOfQuestions switch // selecting number of questions
            {
                1 => 5,
                2 => 10,
                3 => 20,
                _ => 10

            };

            string category = GetCategory(categoryChoice);
            LevelDifficulty? difficulty = GetDifficulty(difficultyChoice);
            List<Question> quizQuestion = GetQuizQuestions(allQuestion, category, difficulty, NumberOfQuestionsChoice);

            if (quizQuestion.Count < NumberOfQuestionsChoice) // Testing Not enough questions available
            {
                Console.WriteLine();
                Console.WriteLine("Not enough question available for your selected category and difficulty");
                return;
            }

            int CorrectAnswer = 0;
            int score = 0;
            int streak = 0;
            int streakBonus = 0;
            bool removeTwoUsed = false;
            bool skipUsed = false;
            bool extraTimeUsed = false;

            List<QuestionReview> reviewList = new List<QuestionReview>();
            reviewList.Clear();
            Console.Clear();
            for (int i = 0; i < quizQuestion.Count; i++) //loop for showing each question
            {
                
                Question question = quizQuestion[i];

                Stopwatch stopwatch = Stopwatch.StartNew();
                int timeLimit = timer.GetTimed(question.Difficulty);


                char? answer = null;
                List<int> hiddenChoices = new List<int>();

                while (true)
                {
                    if (stopwatch.Elapsed.TotalSeconds >= timeLimit)
                    {
                        answer = null;
                        break;
                    }

                    Console.Clear();

                    Console.WriteLine("==========================================".PadRight(42) + "Lifelines===============");
                    Console.WriteLine("=" + $"\tQuestion {i + 1} of {quizQuestion.Count}".PadRight(35) + $"1. Remove Two{(removeTwoUsed ? "[USED]" : "")}".PadRight(23) + "=");
                    Console.WriteLine("=" + $"\tCategory: {question.Category}".PadRight(35) + $"2. Skip Question{(skipUsed ? "[USED]" : "")}".PadRight(23) + "=");
                    Console.WriteLine("=" + $"\tDifficulty: {question.Difficulty}".PadRight(35) + $"3. Add Extra Time{(extraTimeUsed ? "[USED]" : "")}".PadRight(23) + "=");
                    Console.WriteLine("==================================================================");

                    Console.WriteLine(question.QuestionText);
                    Console.WriteLine();

                    char letter = 'A';

                    for (int j = 0; j < question.Choices.Count; j++)
                    {
                        if (hiddenChoices.Contains(j))
                            Console.WriteLine($"{letter}. ----------");
                        else
                            Console.WriteLine($"{letter}. {question.Choices[j]}");

                        letter++;
                    }

                    Console.WriteLine();

                    answer = timer.ReadAnswerWithTimer(timeLimit ,question.Choices.Count,stopwatch);

                    // 1. Remove Two Wrong Answers
                    if (answer == '1')
                    {
                        if (!removeTwoUsed)
                        {
                            removeTwoUsed = true;
                            hiddenChoices = RemoveTwoWrongAnswers(question);
                        }
                        else
                        {
                            Console.WriteLine("\n50:50 has already been used.");
                            Thread.Sleep(1000);
                        }

                        continue;
                    }

                    // 2. Skip Question
                    if (answer == '2')
                    {
                        if (!skipUsed)
                        {
                            skipUsed = true;
                            answer = 'S';
                            Console.WriteLine("\nQuestion skipped!");
                            Thread.Sleep(1000);

                            goto NextQuestion;  
                        }

                        Console.WriteLine("\nSkip has already been used.");
                        Thread.Sleep(1000);
                        continue;
                    }

                    // 3. Add Extra Time
                    if (answer == '3')
                    {
                        if (!extraTimeUsed)
                        {
                            extraTimeUsed = true;

                            timeLimit += 10;    // add 10 seconds

                            Console.WriteLine("\n+10 seconds added!");
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            Console.WriteLine("\nExtra Time has already been used.");
                            Thread.Sleep(1000);
                        }

                        continue;
                    }

                    break;
                }

                Console.WriteLine();

                NextQuestion:
                reviewList.Add(new QuestionReview
                {
                    Question = question,
                    PlayerAnswer = answer
                });

                if (answer == null)
                {
                    Console.WriteLine("No answer submitted.");
                }
                else if (answer == 'S')
                {
                    Console.WriteLine("Question skipped.");
                }
                else
                {
                    int selectedIndex = answer.Value - 'A';

                    if (selectedIndex == question.CorrectAnswerIndex)
                    {
                        Console.WriteLine("Correct!");
                        CorrectAnswer++;
                        score += GetQuestionPoints(question.Difficulty);
                        streak++;
                        // Check for streak bonus
                        if (streak == 3)
                        {
                            score += 10;
                            streakBonus += 10;
                            Console.WriteLine("Three correct answers in a row!");
                            Console.WriteLine("Bonus: +10 points");
                        }
                        else if (streak > 3)
                        {
                            score += 10;
                            streakBonus += 10;
                            Console.WriteLine("Continuous correct answers in a row!");
                            Console.WriteLine("Bonus: +10 points");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nIncorrect");
                        streak = 0; // Break the streak
                        char correctLetter = (char)('A' + question.CorrectAnswerIndex);

                        Console.WriteLine($"Correct Answer: {correctLetter}. {question.Choices[question.CorrectAnswerIndex]}");
                        Console.WriteLine($"Explanation: {question.Explanation}");
                    }
                }

                Console.Write("\nPress Enter for the next questions");
                Console.ReadLine();

            }
            int IncorrectAnswer = quizQuestion.Count - CorrectAnswer;
            double percentage = (double)CorrectAnswer / quizQuestion.Count * 100;

            string performance;

            if (percentage >= 90)
                performance = "Excellent";
            else if (percentage >= 80)
                performance = "Very Good";
            else if (percentage >= 70)
                performance = "Good";
            else if (percentage >= 60)
                performance = "Passed";
            else
                performance = "Needs Improvement";

            QuizResult result = new QuizResult
            { 
                PlayerName = PlayerName,
                score = score,
                TotalQuestions = quizQuestion.Count,
                CorrectAnswer = CorrectAnswer,
                Percentage = percentage,
                category = category,
                Difficulty = difficulty?.ToString() ?? "Mixed",
                CompletedAt = DateTime.Now
            };

            leaderboardRepos.SaveResult(result);


            Console.Clear();

            Console.WriteLine("=================================");
            Console.WriteLine("          QUIZ RESULT");
            Console.WriteLine("=================================\n");

            Console.WriteLine($"Player: {PlayerName}");
            Console.WriteLine($"Total Questions: {quizQuestion.Count}");
            Console.WriteLine($"Correct Answers: {CorrectAnswer}");
            Console.WriteLine($"Incorrect Answers: {IncorrectAnswer}");
            if (streakBonus > 0)
            {
                Console.WriteLine($"Final Score: {score} (+{streakBonus} Streak Bonus)");
            }
            else
            {
                Console.WriteLine($"Final Score: {score}");
            }
            Console.WriteLine($"Percentage: {percentage:F2}%");
            Console.WriteLine($"\nPerformance: {performance}");

            while (true)
            {
                Console.Write("\nDo you want to review your answers? (Y/N): ");
                string? choice = Console.ReadLine()?.Trim().ToUpper();

                if (choice == "Y")
                {
                    questionReviewService.QuestionReviewShow(reviewList);
                    break;
                }
                else if (choice == "N")
                {
                    return; // Return to Main Menu
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter Y or N.");
                }
            }
        }

        private LevelDifficulty? GetDifficulty(int choice)// selecting difficulty
        {
            return choice switch
            {
                1 => LevelDifficulty.Easy,
                2 => LevelDifficulty.Medium,
                3 => LevelDifficulty.Hard,
            };
        }

        private string GetCategory(int choice)// Selecting category
        {
            return choice switch
            {
                1 => "Programming",
                2 => "Science",
                3 => "Mathematics",
                4 => "General Knowledge",
                5 => "Mixed Categories",
            };
        }

        private List<Question> GetQuizQuestions (List<Question> allQuestion, string category, LevelDifficulty? difficulty, int NumberOfQuestionsChoice) //method that filter question
        {
            IEnumerable<Question> filtered = allQuestion;

            if (category != "Mixed Categories")
            {
                filtered = filtered.Where(q => q.Category == category); // filtered by category
            }

            if (difficulty != null)
                filtered = filtered.Where (q => q.Difficulty == difficulty);// filtered by difficulty

            return filtered
                .OrderBy(q => Guid.NewGuid()) // Shuffling 
                .Take(NumberOfQuestionsChoice) // Only show selceted number of questions
                .ToList(); // filtered is still in IEnumerable so to convert use this, convert to List<Question>
        }

        
        private int GetQuestionPoints(LevelDifficulty difficulty)
        {
            return difficulty switch
            {
                LevelDifficulty.Easy => 10,
                LevelDifficulty.Medium => 20,
                LevelDifficulty.Hard => 30,
                _ => 0
            };
        }

        private List<int> RemoveTwoWrongAnswers(Question question)
        {
            Random random = new Random();

            return Enumerable.Range(0, question.Choices.Count)
                .Where(i => i != question.CorrectAnswerIndex)
                .OrderBy(x => random.Next())
                .Take(2)
                .ToList();
        }
    }
}
