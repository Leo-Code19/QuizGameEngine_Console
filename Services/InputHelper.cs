using System;
using System.Collections.Generic;
using System.Text;

namespace QuizGameEngine_1_.Services
{
    public class InputHelper
    {
        public int isValidSelection(int min, int max, string input)// class that get check if the input is in valid range and return it in int 
        {
            while (true)
            {
                Console.Write(input);

                if (int.TryParse(Console.ReadLine(), out int value) && value >= min && value <= max)// Get the input of the user then safely  convert in into int type store into value then evaluate within the range 
                {
                    return value;// do this, if user input can be convertable to int type then only in specific range
                }
                //do this, if not
                Console.WriteLine("Invalid input");
            }
        }

        public string isEmptyAndWhitespaces(string input)// this method is to evaluate if null or only whitespace user input
        {
            while (true)
            {
                Console.Write(input);
                string stringInput = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(stringInput))// if user input is not null and whitespace only 
                {
                    return  stringInput.Trim();// do this 
                }
                Console.WriteLine("Invalid: input cannot be empty or spaces only");// if null and whitespace, do this
            }
        }
    }
}
