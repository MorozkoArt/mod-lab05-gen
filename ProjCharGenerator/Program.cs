using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace generator
{
    public class BigramGenerator
    {
        private readonly Dictionary<string, Dictionary<string, int>> bigrams = new();
        private readonly Random random = new();

        private readonly string bigrams_path;
        public BigramGenerator(string bigrams_path)    
        {
            this.bigrams_path = bigrams_path;
            LoadBigrams();
        }

        private void LoadBigrams()
        {
            var lines = File.ReadAllLines(bigrams_path);
            
            foreach (var line in lines)
            {
                var parts = line.Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);        
                var bigram = parts[0].Trim().ToLower(); 
                if (bigram.Length != 2) continue;
                var firstChar = bigram[0].ToString();
                var secondChar = bigram[1].ToString();
                if (!int.TryParse(parts[1].Trim(), out int frequency))
                    continue;
                if (!bigrams.ContainsKey(firstChar))
                    bigrams[firstChar] = new Dictionary<string, int>();
                bigrams[firstChar][secondChar] = frequency;
            }
        }

        public void Generare_load(string resualt_path) 
        {
            string generatedText = GenerateText();
            File.WriteAllText(resualt_path, generatedText);
        }

        public string GenerateText(int length = 1000, int maxLineLength = 100)
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


    public class WordGenerator
    {
        private readonly Dictionary<string, int> wordFrequencies = new();
        private readonly Random random = new();
        private readonly string words_path;

        public WordGenerator(string words_path)
        {
            this.words_path = words_path;
            LoadWordFrequenciesFromText();
        }

        public void LoadWordFrequenciesFromText()
        {
            var lines = File.ReadAllLines(words_path);
            
            foreach (var line in lines)
            {
                var parts = line.Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);    
                if (parts.Length < 2) continue;
                
                var word = parts[0].Trim();
                string freqStr = parts[1].Trim();
                if (double.TryParse(freqStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double frequency))
                {
                    int intFrequency = (int)(frequency * 100);
                    wordFrequencies[word] = intFrequency;
                }
            }
        }

        public void Generare_load(string resualt_path) 
        {
            string generatedText = GenerateText();
            File.WriteAllText(resualt_path, generatedText);
        }

        public string GenerateText(int wordCount = 1000, int maxLineLength = 100)
        {
            if (wordFrequencies.Count == 0)
                throw new InvalidOperationException("Word frequencies not loaded");
            
            var result = new StringBuilder();
            var currentLineLength = 0;
            var totalFrequency = wordFrequencies.Values.Sum();
            
            for (int i = 0; i < wordCount; i++)
            {
                var randomValue = random.Next(totalFrequency);
                var currentSum = 0;
                string selectedWord = "";
                
                foreach (var pair in wordFrequencies)
                {
                    currentSum += pair.Value;
                    if (randomValue < currentSum)
                    {
                        selectedWord = pair.Key;
                        break;
                    }
                }
                
                if (currentLineLength > 0 && currentLineLength + selectedWord.Length + 1 > maxLineLength)
                {
                    result.AppendLine();
                    currentLineLength = 0;
                }
                else if (currentLineLength > 0)
                {
                    result.Append(" ");
                    currentLineLength++;
                }
                
                result.Append(selectedWord);
                currentLineLength += selectedWord.Length;
            }
            return result.ToString();
        }
    }

    public class TextFileProcessor
    {
        public void CleanFile(string inputPath, string outputPath, int index1, int index2, int minelem)
        {
            var uniqueLemmas = new HashSet<string>();
            var outputLines = new List<string>();       
            foreach (var line in File.ReadLines(inputPath))
            {
                if (string.IsNullOrWhiteSpace(line) || !char.IsDigit(line.Trim()[0]))
                    continue;
                var parts = line.Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries); 
                if (parts.Length < minelem) continue;         
                var lemma = parts[index1].Trim().ToLower();
                var frequency = parts[index2].Trim();
                outputLines.Add($"{lemma} {frequency}");
            }
            File.WriteAllLines(outputPath, outputLines, Encoding.UTF8);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string res_path = Path.Combine(Directory.GetCurrentDirectory(), "res");

            string bigrams_path = Path.Combine(res_path, "bigrams.txt");
            string words_path = Path.Combine(res_path, "words.txt");
            if (!File.Exists(bigrams_path) || !File.Exists(words_path))
            {
                var processor = new TextFileProcessor();
                if (!File.Exists(bigrams_path))
                    processor.CleanFile(Path.Combine(res_path, "bigrams_r.txt"), bigrams_path, 1, 2, 4);                   
                if (!File.Exists(words_path))
                    processor.CleanFile(Path.Combine(res_path, "words_r.txt"), words_path, 1, 4, 5);
            }

            string resualt_path = Path.Combine(Directory.GetCurrentDirectory(), "..", "Results");
            string gen1_path = Path.Combine(resualt_path, "gen-1.txt");
            string gen2_path = Path.Combine(resualt_path, "gen-2.txt");
            if (File.Exists(gen1_path)) File.Delete(gen1_path);
            if (File.Exists(gen2_path)) File.Delete(gen2_path);

            var bigrams_generator = new BigramGenerator(bigrams_path);
            bigrams_generator.Generare_load(gen1_path); 

            var words_generator = new WordGenerator(words_path);
            words_generator.Generare_load(gen2_path); 
        }
    }
}

