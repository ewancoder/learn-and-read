using LearnAndRead.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LearnAndRead.Infrastructure
{
    public static class Storage
    {
        private const string WordsFileName = "words.txt";
        private static Dictionary<string, Word> _words;

        static Storage()
        {
            if (File.Exists(WordsFileName))
            {
                try
                {
                    LoadWords();
                }
                catch
                {
                    File.Copy(WordsFileName, $"{WordsFileName}_backup${DateTime.Now.ToString("yyyyMMdd")}.txt");
                    CreateNewWords();
                }
            }
            else
            {
                CreateNewWords();
            }
        }

        public static IEnumerable<string> GetUnknownWords()
        {
            return _words
                .Where(w => w.Value.Status == WordStatus.Unknown)
                .Select(w => w.Key);
        }

        public static IEnumerable<string> GetWordsToRepeat()
        {
            return _words
                .Where(w => w.Value.Status == WordStatus.NeedToRepeat)
                .Select(w => w.Key);
        }

        public static bool IsAdded(string word)
        {
            return _words.ContainsKey(word);
        }

        public static bool NeedToLearn(string word)
        {
            if (!_words.ContainsKey(word))
                return true;

            return _words[word].Status != WordStatus.Learned && _words[word].Status != WordStatus.NotAWord;
        }

        public static void AddWord(Word word)
        {
            if (_words.ContainsKey(word.Value))
            {
                _words[word.Value].Status = word.Status;
            }
            else
            {
                _words.Add(word.Value, word);
            }

            SaveWords();
        }

        private static void CreateNewWords()
        {
            _words = new Dictionary<string, Word>();
        }

        private static void LoadWords()
        {
            var words = new List<Word>();

            using (var file = new StreamReader(WordsFileName))
            {
                string line;

                while ((line = file.ReadLine()) != null)
                {
                    var parts = line.Split(Word.Separator);
                    var word = new Word(parts[0], (WordStatus)Convert.ToByte(parts[1]));
                    words.Add(word);
                }
            }

            _words = words.ToDictionary(w => w.Value);
        }

        private static void SaveWords()
        {
            using (var file = new StreamWriter(WordsFileName))
            {
                foreach (var word in _words.Values)
                {
                    file.WriteLine(word.MakeLine());
                }
            }
        }
    }
}
