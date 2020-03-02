using LearnAndRead.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LearnAndRead
{
    static class Program
    {
        static void Main()
        {
            Console.WriteLine("Path to text file: ");
            var path = Console.ReadLine();
            if (path == string.Empty)
                path = "text.txt";

            if (path == "u")
            {
                LoadUnknownWords();
                return;
            }

            if (path == "r")
            {
                LoadWordsToRepeat();
                return;
            }

            var allWords = SortWords(File.ReadAllText(path).Replace("\r\n", " ").Replace('\n', ' ').Split(' '))
                .Where(w => Storage.NeedToLearn(w.Key)).ToList();

            var words = allWords.Where(w => !Storage.IsAdded(w.Key)).ToList();
            if (words.Count == 0)
                words = allWords;

            Console.WriteLine($"{words.Count} words.");
            Console.ReadLine();

            var count = 1;
            foreach (var word in words)
            {
                Console.Clear();
                Console.WriteLine(count++);
                Console.Write($"{word.Key} ({word.Value})");
                var key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        Storage.AddWord(new Domain.Word(word.Key, Domain.WordStatus.Learned));
                        break;
                    case ConsoleKey.R:
                        Storage.AddWord(new Domain.Word(word.Key, Domain.WordStatus.NeedToRepeat));
                        break;
                    case ConsoleKey.N:
                        Storage.AddWord(new Domain.Word(word.Key, Domain.WordStatus.NotAWord));
                        break;
                    case ConsoleKey.Z:
                        Storage.AddWord(new Domain.Word(word.Key, Domain.WordStatus.Unknown));
                        break;
                    default:
                        break;
                }
            }
        }

        private static void LoadUnknownWords()
        {
            var words = Storage.GetUnknownWords().OrderBy(w => w).ToList();

            Console.WriteLine($"{words.Count} words.");
            Console.ReadLine();

            var count = 1;
            foreach (var word in words)
            {
                Console.Clear();
                Console.WriteLine(count++);
                Console.Write(word);
                var key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        Storage.AddWord(new Domain.Word(word, Domain.WordStatus.Learned));
                        break;
                    case ConsoleKey.R:
                        Storage.AddWord(new Domain.Word(word, Domain.WordStatus.NeedToRepeat));
                        break;
                    case ConsoleKey.N:
                        Storage.AddWord(new Domain.Word(word, Domain.WordStatus.NotAWord));
                        break;
                    case ConsoleKey.Z:
                        Storage.AddWord(new Domain.Word(word, Domain.WordStatus.Unknown));
                        break;
                    default:
                        break;
                }
            }
        }

        private static void LoadWordsToRepeat()
        {
            var words = Storage.GetWordsToRepeat().OrderBy(w => w).ToList();

            Console.WriteLine($"{words.Count} words.");
            Console.ReadLine();

            var count = 1;
            foreach (var word in words)
            {
                Console.Clear();
                Console.WriteLine(count++);
                Console.Write(word);
                var key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        Storage.AddWord(new Domain.Word(word, Domain.WordStatus.Learned));
                        break;
                    case ConsoleKey.R:
                        Storage.AddWord(new Domain.Word(word, Domain.WordStatus.NeedToRepeat));
                        break;
                    case ConsoleKey.N:
                        Storage.AddWord(new Domain.Word(word, Domain.WordStatus.NotAWord));
                        break;
                    case ConsoleKey.Z:
                        Storage.AddWord(new Domain.Word(word, Domain.WordStatus.Unknown));
                        break;
                    default:
                        break;
                }
            }
        }

        private static IEnumerable<KeyValuePair<string, int>> SortWords(IEnumerable<string> words)
        {
            var pattern = new Regex("[^a-z-]");

            var wordGroups = words
                .Select(w => w.ToLowerInvariant())
                .Select(w => pattern.Replace(w, ""))
                .Select(w => w.Trim())
                .Select(w => w.Trim('-'))
                .GroupBy(w => w)
                .Select(wordGroup => new
                {
                    Word = wordGroup.Key,
                    Frequency = wordGroup.Count()
                })
                .OrderByDescending(wordGroup => wordGroup.Frequency)
                .ThenBy(wordGroup => wordGroup.Word);

            return wordGroups
                .Where(w => w.Word.Length > 0)
                .Select(wordGroup => new KeyValuePair<string, int>(wordGroup.Word, wordGroup.Frequency));
        }
    }
}
