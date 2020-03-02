using System;

namespace LearnAndRead.Domain
{
    public sealed class Word
    {
        public const char Separator = '⍆';

        public Word(string value, WordStatus status)
        {
            Value = value;
            Status = status;
        }

        public string Value { get; set; }
        public WordStatus Status { get; set; }

        public string MakeLine()
        {
            return $"{Value}{Separator}{Convert.ToByte(Status)}";
        }
    }

    public enum WordStatus : byte
    {
        Unknown = 0,
        Learned = 1,
        NeedToRepeat = 2,
        NotAWord = 3
    }
}
