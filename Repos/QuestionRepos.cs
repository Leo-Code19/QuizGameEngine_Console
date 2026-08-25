using QuizGameEngine_1_.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuizGameEngine_1_.Repos
{
    public class QuestionRepos
    {

        private readonly string filepath = "Data/questions.json"; // Find the json file

        public List<Question> LoadQuestions()
        {
            try
            {
                if (!File.Exists(filepath)) // check if json file is existing. TESTING Missing questions.json
                {
                    Console.WriteLine("Json file cannot be found");// if not do this
                    return new List<Question>();
                }
                string jsonString = File.ReadAllText(filepath);//if do exist to this -  Read all text in the json file

                if (string.IsNullOrEmpty(jsonString))// check if json file is empty
                {
                    Console.WriteLine("Json file is Empty"); // if do empty do this
                    return new List<Question>();
                }
                var options = new JsonSerializerOptions // configures how json serialize and deserialize
                {
                    PropertyNameCaseInsensitive = true, // accept any input regardless of casing
                    Converters =
                    {
                        new JsonStringEnumConverter() // One of the custom translation rules used during serialization and deserialization
                    }     // The specific built-in rule that instructs the serializer to map enum names directly to JSON strings, and vice versa.
                };

                List<Question>? question = JsonSerializer.Deserialize<List<Question>>(jsonString, options); // Deserialize means taking json content to object with the help option customize rule and return to List
                if (question == null)
                {
                    Console.WriteLine("Error: Failed to load questions.");
                    return new List<Question>();
                }

                if (HasDuplicateIds(question))
                {
                    Console.WriteLine("Error: Duplicate question IDs found");
                    return new List<Question>();
                }

                foreach (var q in question)
                {
                    if (!IsValidQuestion(q))
                    {
                        return new List<Question>();
                    }
                }

                if (question == null)
                {
                    Console.WriteLine("Error: failed to load question");
                    return new List<Question>();
                }
                return question;

            }

            catch (JsonException) // Only triggers on JSON-related errors, TESTING: Invalid JSON format
            {
                Console.WriteLine("Error: Invalid format of json");
                return new List<Question>();
            }
            catch (Exception ex)// Triggers on absolutely any error
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return new List<Question>();
            }
        }

        private bool HasDuplicateIds(List<Question> questions) // Validation for duplicate ids
        {
            return questions.GroupBy(q => q.Id).Any(group => group.Count() > 1);
        }

        private bool IsValidQuestion(Question question)
        {
            if (string.IsNullOrWhiteSpace(question.QuestionText))
            {
                Console.WriteLine($"Question ID {question.Id}: Question text cannot be empty.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(question.Category))
            {
                Console.WriteLine($"Question ID {question.Id}: Category cannot be empty.");
                return false;
            }

            if (question.Choices == null || question.Choices.Count < 2)
            {
                Console.WriteLine($"Question ID {question.Id}: At least 2 answer choices are required.");
                return false;
            }

            // Check for empty answer choices
            if (question.Choices.Any(choice => string.IsNullOrWhiteSpace(choice)))
            {
                Console.WriteLine($"Question ID {question.Id}: Invalid answer choices. Choices cannot be empty.");
                return false;
            }

            // Check for duplicate answer choices
            if (question.Choices.Distinct(StringComparer.OrdinalIgnoreCase).Count() != question.Choices.Count)
            {
                Console.WriteLine($"Question ID {question.Id}: Duplicate answer choices are not allowed.");
                return false;
            }

            if (question.CorrectAnswerIndex < 0 ||
                question.CorrectAnswerIndex >= question.Choices.Count)
            {
                Console.WriteLine($"Question ID {question.Id}: Invalid CorrectAnswerIndex ({question.CorrectAnswerIndex}). It must be between 0 and {question.Choices.Count - 1}.");
                return false;
            }

            return true;
        }
    }
    
}
