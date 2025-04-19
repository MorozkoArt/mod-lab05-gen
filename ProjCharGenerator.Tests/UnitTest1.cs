using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace generator.Tests
{
    public class BigramGeneratorTests
    {
        string res_path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "ProjCharGenerator", "res");
        string resualt_path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "Results");

        [Fact]
        public void Load_bigrams_test()
        {
            string bigrams_path = Path.Combine(res_path, "bigrams.txt");
            string data_gen1_path = Path.Combine(resualt_path, "data-gen-1.txt");
            var bigrams_generator = new BigramGenerator(bigrams_path, data_gen1_path);
            Dictionary<string, int> bigrams = bigrams_generator.Get_bigramsd();
            int total_summ = bigrams_generator.Get_total_summ();

            Assert.NotNull(bigrams); 
            Assert.NotEmpty(bigrams); 
            Assert.True(total_summ > 0); 
        }

        [Fact]
        public void Generate_text_test()
        {
            string bigrams_path = Path.Combine(res_path, "bigrams.txt");
            string data_gen1_path = Path.Combine(resualt_path, "data-gen-1.txt");
            string gen1_path = Path.Combine(resualt_path, "gen-1.txt");
            if (File.Exists(gen1_path)) File.Delete(gen1_path);
            if (File.Exists(data_gen1_path)) File.Delete(data_gen1_path);
            var bigrams_generator = new BigramGenerator(bigrams_path, data_gen1_path);
            bigrams_generator.Generare_load(gen1_path);

            Assert.True(File.Exists(gen1_path));
            var fileInfo = new FileInfo(gen1_path);
            Assert.True(fileInfo.Length > 0);
            var content = File.ReadAllText(gen1_path);
            var cleanContent = content.Replace("\r", "").Replace("\n", ""); 
            Assert.Equal(2000, cleanContent.Length);  
            Assert.True(cleanContent.All(char.IsLetter)); 
        }

        [Fact]
        public void Frequency_bigramm_test()
        {
            string bigrams_path = Path.Combine(res_path, "bigrams.txt");
            string data_gen1_path = Path.Combine(resualt_path, "data-gen-1.txt");
            string gen1_path = Path.Combine(resualt_path, "gen-1.txt");
            if (File.Exists(gen1_path)) File.Delete(gen1_path);
            if (File.Exists(data_gen1_path)) File.Delete(data_gen1_path);
            var bigrams_generator = new BigramGenerator(bigrams_path, data_gen1_path);
            bigrams_generator.Generare_load(gen1_path);
            Dictionary<string, int> frequency = bigrams_generator.Get_frequency();
            Assert.NotNull(frequency); 
            Assert.NotEmpty(frequency); 
        }

        [Fact]
        public void Data_bigramm_test()
        {
            string bigrams_path = Path.Combine(res_path, "bigrams.txt");
            string data_gen1_path = Path.Combine(resualt_path, "data-gen-1.txt");
            string gen1_path = Path.Combine(resualt_path, "gen-1.txt");
            if (File.Exists(gen1_path)) File.Delete(gen1_path);
            if (File.Exists(data_gen1_path)) File.Delete(data_gen1_path);
            var bigrams_generator = new BigramGenerator(bigrams_path, data_gen1_path);
            bigrams_generator.Generare_load(gen1_path);

            Assert.True(File.Exists(data_gen1_path));
            var fileInfo = new FileInfo(data_gen1_path);
            Assert.True(fileInfo.Length > 0);
        }

        [Fact]
        public void Generate_text_exception_test()
        {
            string _tempBigramsPath = Path.GetTempFileName();
            File.WriteAllText(_tempBigramsPath, "");
            var dataPath = "data.txt";
            var generator = new BigramGenerator(_tempBigramsPath, dataPath);
            var exception = Assert.Throws<InvalidOperationException>(() => generator.Generare_load("output.txt"));
            Assert.Equal("Bigrams not loaded", exception.Message);
        }
    }

    public class WordsGeneratorTests
    {
        string res_path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "ProjCharGenerator", "res");
        string resualt_path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "Results");

        [Fact]
        public void Load_words_test()
        {
            string words_path = Path.Combine(res_path, "words.txt");
            string data_gen2_path = Path.Combine(resualt_path, "data-gen-2.txt");
            var words_generator = new WordGenerator(words_path, data_gen2_path);
            Dictionary<string, int> word = words_generator.Get_words();
            int total_summ = words_generator.Get_total_summ();

            Assert.NotNull(word); 
            Assert.NotEmpty(word); 
            Assert.True(total_summ > 0); 
        }

        [Fact]
        public void Generate_text_test()
        {
            string words_path = Path.Combine(res_path, "words.txt");
            string data_gen2_path = Path.Combine(resualt_path, "data-gen-2.txt");
            string gen2_path = Path.Combine(resualt_path, "gen-2.txt");
            if (File.Exists(gen2_path)) File.Delete(gen2_path);
            if (File.Exists(data_gen2_path)) File.Delete(data_gen2_path);
            var words_generator = new WordGenerator(words_path, data_gen2_path);
            words_generator.Generare_load(gen2_path);
            Assert.True(File.Exists(gen2_path));
            var fileInfo = new FileInfo(gen2_path);
            Assert.True(fileInfo.Length > 0);
            var content = File.ReadAllText(gen2_path);
            var words = content.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            
            Assert.Equal(1000, words.Length);
            Assert.All(words, word => Assert.False(string.IsNullOrWhiteSpace(word)));
        }

        [Fact]
        public void Frequency_words_test()
        {
            string words_path = Path.Combine(res_path, "words.txt");
            string data_gen2_path = Path.Combine(resualt_path, "data-gen-2.txt");
            string gen2_path = Path.Combine(resualt_path, "gen-2.txt");
            if (File.Exists(gen2_path)) File.Delete(gen2_path);
            if (File.Exists(data_gen2_path)) File.Delete(data_gen2_path);
            var words_generator = new WordGenerator(words_path, data_gen2_path);
            words_generator.Generare_load(gen2_path);
            Dictionary<string, int> frequency = words_generator.Get_frequency();
            Assert.NotNull(frequency); 
            Assert.NotEmpty(frequency); 
        }

        [Fact]
        public void Data_words_test()
        {
            string words_path = Path.Combine(res_path, "words.txt");
            string data_gen2_path = Path.Combine(resualt_path, "data-gen-2.txt");
            string gen2_path = Path.Combine(resualt_path, "gen-2.txt");
            if (File.Exists(gen2_path)) File.Delete(gen2_path);
            if (File.Exists(data_gen2_path)) File.Delete(data_gen2_path);
            var words_generator = new WordGenerator(words_path, data_gen2_path);
            words_generator.Generare_load(gen2_path);

            Assert.True(File.Exists(data_gen2_path));
            var fileInfo = new FileInfo(data_gen2_path);
            Assert.True(fileInfo.Length > 0);
        }

        [Fact]
        public void Generate_text_exception_test()
        {
            string _temp_words_path = Path.GetTempFileName();
            File.WriteAllText(_temp_words_path, "");
            var dataPath = "data.txt";
            var generator = new WordGenerator(_temp_words_path, dataPath);
            var exception = Assert.Throws<InvalidOperationException>(() => generator.Generare_load("output.txt"));
            Assert.Equal("Word frequencies not loaded", exception.Message);
        }
    }
}