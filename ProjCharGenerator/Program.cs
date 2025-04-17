using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace generator
{
    public class BigramGenerator
    {
        private readonly Dictionary<string, Dictionary<string, int>> bigrams = new();
        private readonly Random random = new();

        public void LoadBigrams(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || !char.IsDigit(line.Trim()[0]))
                    continue;
                
                var parts = line.Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
                
                if (parts.Length < 3) continue;
                
                var bigram = parts[1].Trim().ToLower(); 
                if (bigram.Length != 2) continue;
                
                var firstChar = bigram[0].ToString();
                var secondChar = bigram[1].ToString();
            
                if (!int.TryParse(parts[2].Trim(), out int frequency))
                    continue;
                
                if (!bigrams.ContainsKey(firstChar))
                    bigrams[firstChar] = new Dictionary<string, int>();
                
                bigrams[firstChar][secondChar] = frequency;
            }
        }

        public string GenerateText(int length, int maxLineLength = 100)
        {
            if (bigrams.Count == 0)
                throw new InvalidOperationException("Bigrams not loaded");
            
            var result = new StringBuilder();
            var currentLineLength = 0;
            string current = GetRandomStartChar();
            result.Append(current);
            currentLineLength += current.Length;

            for (int i = 1; i < 2*length; i++)
            {
                if (!bigrams.ContainsKey(current))
                    current = GetRandomStartChar();
                
                var possibleNext = bigrams[current];
                current = ChooseNextChar(possibleNext);
                
                if (currentLineLength + current.Length > maxLineLength)
                {
                    result.AppendLine();
                    currentLineLength = 0;
                }
                result.Append(current);
                currentLineLength += current.Length;
            }
            return result.ToString();
        }

        private string GetRandomStartChar()
        {
            var keys = bigrams.Keys.ToList();
            return keys[random.Next(keys.Count)];
        }

        private string ChooseNextChar(Dictionary<string, int> options)
        {
            var total = options.Values.Sum();
            var randomValue = random.Next(total);
            var currentSum = 0;
            
            foreach (var pair in options)
            {
                currentSum += pair.Value;
                if (randomValue < currentSum)
                    return pair.Key;
            }
            
            return options.Keys.First();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var generator = new BigramGenerator();
            generator.LoadBigrams("bigrams.txt");
            var generatedText = generator.GenerateText(1000); 
            string resualt_path = Path.Combine(Directory.GetCurrentDirectory(), "..", "Results", "gen-1.txt");
            File.WriteAllText(resualt_path, generatedText);
        }
    }
}

