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
        private readonly Dictionary<string, int> bigrams = new();
        private readonly Random random = new();
        private readonly Dictionary<string, int> frequency = new();
        private readonly string bigrams_path;
        private readonly string data_path;
        private readonly int length = 1000;
        private int total_summ;
        public BigramGenerator(string bigrams_path, string data_path)    
        {
            this.bigrams_path = bigrams_path;
            this.data_path = data_path;
            Load_bigrams();
        }

        private void Load_bigrams()
        {
            var lines = File.ReadAllLines(bigrams_path);
            
            foreach (var line in lines)
            {
                var parts = line.Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);        
                var bigram = parts[0].Trim().ToLower(); 
                if (bigram.Length != 2) continue;
                if (!int.TryParse(parts[1].Trim(), out int frequency))
                    continue;
                if (!bigrams.ContainsKey(bigram))
                    bigrams[bigram] = frequency;
            }
            total_summ = bigrams.Values.Sum();
        }

        public void Generare_load(string resualt_path) 
        {
            string generatedText = GenerateText();
            File.WriteAllText(resualt_path, generatedText);
            Make_data();
        }

        private string GenerateText(int maxLineLength = 100)
        {
            if (bigrams.Count == 0)
                throw new InvalidOperationException("Bigrams not loaded");
            var result = new StringBuilder();
            var currentLineLength = 0;

            for (int i = 0; i < length; i++)
            {
                string current = ChooseNextChar(bigrams);
                Add_frequency(current);
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
        private void Add_frequency(string current)
        {
            if (frequency.TryGetValue(current, out int count))
            {
                frequency[current] = count + 1;
            }
            else
                frequency.Add(current, 1);
        }

        private void Make_data()
        {
            using StreamWriter data = new StreamWriter(data_path, false, Encoding.UTF8);
            foreach (KeyValuePair<string, int> bigram in frequency)
            {
                data.WriteLine(bigram.Key + " " + ((double)bigram.Value / length).ToString() + " " + Math.Round((double)bigrams[bigram.Key] / total_summ, 5).ToString());
            }
        }

        private string ChooseNextChar(Dictionary<string, int> bigrams)
        {
            var randomValue = random.Next(total_summ);
            var currentSum = 0;
            
            foreach (var pair in bigrams)
            {
                currentSum += pair.Value;
                if (randomValue < currentSum)
                    return pair.Key;
            }
            
            return bigrams.Keys.First();
        }

        public Dictionary<string, int> Get_bigramsd() 
        {
            return bigrams;
        }

        public int Get_total_summ()
        {
            return total_summ;
        }

        public Dictionary<string, int> Get_frequency() 
        {
            return frequency;
        }
    }


    public class WordGenerator
    {
        private readonly Dictionary<string, int> wordFrequencies = new();
        private readonly Random random = new();
        public readonly Dictionary<string, int> frequency = new();
        private readonly string words_path;
        private readonly string data_path;
        private readonly int wordCount = 1000;
        private int total_summ;

        public WordGenerator(string words_path, string data_path)
        {
            this.words_path = words_path;
            this.data_path = data_path;
            Load_words();
        }

        private void Load_words()
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
            total_summ = wordFrequencies.Values.Sum();
        }

        public void Generare_load(string resualt_path) 
        {
            string generatedText = GenerateText();
            File.WriteAllText(resualt_path, generatedText);
            Make_data();
        }

        private string GenerateText(int maxLineLength = 100)
        {
            if (wordFrequencies.Count == 0)
                throw new InvalidOperationException("Word frequencies not loaded");
            
            var result = new StringBuilder();
            var currentLineLength = 0;
            
            for (int i = 0; i < wordCount; i++)
            {
                string current = ChooseNextWord(wordFrequencies);
                Add_frequency(current);
                if (currentLineLength > 0 && currentLineLength + current.Length + 1 > maxLineLength)
                {
                    result.AppendLine();
                    currentLineLength = 0;
                }
                else if (currentLineLength > 0)
                {
                    result.Append(" ");
                    currentLineLength++;
                }
                result.Append(current);
                currentLineLength += current.Length;
            }
            return result.ToString();
        }
        private void Add_frequency(string current)
        {
            if (frequency.TryGetValue(current, out int count))
            {
                frequency[current] = count + 1;
            }
            else
                frequency.Add(current, 1);
        }

        private void Make_data()
        {
            using StreamWriter data = new StreamWriter(data_path, false, Encoding.UTF8);
            foreach (KeyValuePair<string, int> word in frequency)
            {
                data.WriteLine(word.Key+" "+((double)word.Value / wordCount).ToString()+" " + Math.Round((double)wordFrequencies[word.Key] / total_summ,5).ToString());
            }
        }

        private string ChooseNextWord(Dictionary<string, int> wordFrequencies)
        {
            var randomValue = random.Next(total_summ);
            var currentSum = 0;
            
            foreach (var pair in wordFrequencies)
            {
                currentSum += pair.Value;
                if (randomValue < currentSum)
                    return pair.Key;
            }
            return wordFrequencies.Keys.First();
        }
        public Dictionary<string, int> Get_words() 
        {
            return wordFrequencies;
        }

        public int Get_total_summ()
        {
            return total_summ;
        }

        public Dictionary<string, int> Get_frequency() 
        {
            return frequency;
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
            string data_gen1_path = Path.Combine(resualt_path, "data-gen-1.txt");
            string data_gen2_path = Path.Combine(resualt_path, "data-gen-2.txt");
            if (File.Exists(gen1_path)) File.Delete(gen1_path);
            if (File.Exists(gen2_path)) File.Delete(gen2_path);
            if (File.Exists(data_gen1_path)) File.Delete(data_gen1_path);
            if (File.Exists(data_gen2_path)) File.Delete(data_gen2_path);

            var bigrams_generator = new BigramGenerator(bigrams_path, data_gen1_path);
            bigrams_generator.Generare_load(gen1_path); 

            var words_generator = new WordGenerator(words_path, data_gen2_path);
            words_generator.Generare_load(gen2_path); 
        }
    }
}

