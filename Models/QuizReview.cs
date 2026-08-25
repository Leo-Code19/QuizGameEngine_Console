using System;
using System.Collections.Generic;
using System.Text;

namespace QuizGameEngine_1_.Models
{
    public class QuestionReview
    {
        public Question Question { get; set; }
        public char? PlayerAnswer { get; set; }
    }
}
