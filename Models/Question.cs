using QuizGameEngine_1_.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizGameEngine_1_.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public LevelDifficulty Difficulty {  get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public List<string> Choices { get; set; } = new();
        public int CorrectAnswerIndex { get; set; }
        public string Explanation {  get; set; } = string.Empty;
    }
}
