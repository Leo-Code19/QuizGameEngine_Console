using System;
using System.Collections.Generic;
using System.Text;
using QuizGameEngine_1_.Models;

namespace QuizGameEngine_1_.Services
{
    public class QuestionReviewService
    {
        public void QuestionReviewShow(List<QuestionReview> reviewList)

        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("=\t\tQUESTION REVIEW\t\t=");
            Console.WriteLine("========================================\n");

            for (int i = 0; i < reviewList.Count; i++)
            {
                var review = reviewList[i];
                var question = review.Question;

                Console.WriteLine($"Question {i + 1}");
                Console.WriteLine($"Question: {question.QuestionText}");

                if (review.PlayerAnswer == null)
                {
                    Console.WriteLine("Your Answer: No Answer (Time Out)");
                }
                else if (review.PlayerAnswer == 'S')
                {
                    Console.WriteLine("Your Answer: Skipped");
                }
                else
                {
                    int playerIndex = review.PlayerAnswer.Value - 'A';

                    Console.WriteLine($"Your Answer: {review.PlayerAnswer}. {question.Choices[playerIndex]}");
                }

                char correctLetter = (char)('A' + question.CorrectAnswerIndex);

                Console.WriteLine($"Correct Answer: {correctLetter}. {question.Choices[question.CorrectAnswerIndex]}");

                Console.WriteLine($"Explanation: {question.Explanation}");

                Console.WriteLine(new string('-', 50));
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }
    } 
}
