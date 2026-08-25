using System;
using System.Collections.Generic;
using System.Text;

namespace QuizGameEngine_1_.Models
{
    public class QuizResult
    {
        public string PlayerName { get; set; } = string.Empty;
        public int score { get; set; }
        public  int TotalQuestions { get; set; }
        public int CorrectAnswer { get; set; }
        public double Percentage { get; set; }
        public string category { get; set; } = string.Empty ;
        public string Difficulty { get; set; }= string.Empty;
        public DateTime CompletedAt { get; set; }
    }
}
