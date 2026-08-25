using QuizGameEngine_1_.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace QuizGameEngine_1_.Repos
{
    public class LeaderboardRepos
    {
        private readonly string filepath =Path.Combine(Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName,"Data","Leaderboard.json");

        public void SaveResult(QuizResult result)
        {
            List<QuizResult> results = LoadResults();

            results.Add(result);

            string json = JsonSerializer.Serialize(results, new JsonSerializerOptions{WriteIndented = true});

            File.WriteAllText(filepath, json);
        }

        public List<QuizResult> LoadResults()
        {
            try
            {
                if (!File.Exists(filepath))
                {
                    Console.WriteLine("Leaderboard.json not found");
                    return new List<QuizResult>();
                }
                string json = File.ReadAllText(filepath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<QuizResult>();
                }
                List<QuizResult> results = JsonSerializer.Deserialize<List<QuizResult>>(json);

                return results ?? new List<QuizResult>();

            }
            catch
            {
                return new List<QuizResult>();
            }
        }

    }
}
