using System;
using System.Text.RegularExpressions;

namespace TryashtarUtils.Nbt
{
    public class SnbtOptions
    {
        public bool Minified;
        public bool IsJsonLike;
        public Predicate<string> ShouldQuoteKeys;
        public Predicate<string> ShouldQuoteStrings;
        public QuoteMode KeyQuoteMode;
        public QuoteMode StringQuoteMode;
        public bool NumberSuffixes;
        public bool ArrayPrefixes;
        public INewlineHandler NewlineHandling;

        public SnbtOptions Expanded()
        {
            this.Minified = false;
            return this;
        }

        public SnbtOptions WithHandler(INewlineHandler handler)
        {
            this.NewlineHandling = handler;
            return this;
        }

        private static readonly Regex StringRegex = new("^[A-Za-z0-9._+-]+$", RegexOptions.Compiled);

        public static SnbtOptions Default => new()
        {
            Minified = true,
            ShouldQuoteKeys = x => !StringRegex.IsMatch(x),
            ShouldQuoteStrings = x => true,
            KeyQuoteMode = QuoteMode.Automatic,
            StringQuoteMode = QuoteMode.Automatic,
            NumberSuffixes = true,
            ArrayPrefixes = true,
            NewlineHandling = EscapeHandler.Instance
        };
        public static SnbtOptions DefaultExpanded => Default.Expanded();

        public static SnbtOptions JsonLike => new()
        {
            Minified = true,
            IsJsonLike = true,
            ShouldQuoteKeys = x => true,
            ShouldQuoteStrings = x => x != "null",
            KeyQuoteMode = QuoteMode.DoubleQuotes,
            StringQuoteMode = QuoteMode.DoubleQuotes,
            NumberSuffixes = false,
            ArrayPrefixes = false,
            NewlineHandling = EscapeHandler.Instance
        };
        public static SnbtOptions JsonLikeExpanded => JsonLike.Expanded();

        public static SnbtOptions Preview => new()
        {
            Minified = true,
            ShouldQuoteKeys = x => false,
            ShouldQuoteStrings = x => false,
            NumberSuffixes = false,
            ArrayPrefixes = false,
            NewlineHandling = new ReplaceHandler("⏎")
        };

        public static SnbtOptions MultilinePreview => Preview.WithHandler(IgnoreHandler.Instance);
    }

    public enum QuoteMode
    {
        Automatic,
        DoubleQuotes,
        SingleQuotes
    }

    public interface INewlineHandler
    {
        string Handle();
    }

    public class IgnoreHandler : INewlineHandler
    {
        public static readonly IgnoreHandler Instance = new();
        public string Handle() => Environment.NewLine;
    }

    public class EscapeHandler : INewlineHandler
    {
        public static readonly EscapeHandler Instance = new();
        public string Handle() => Snbt.STRING_ESCAPE + "n";
    }

    public class ReplaceHandler : INewlineHandler
    {
        public readonly string Replacement;
        public ReplaceHandler(string replacement)
        {
            Replacement = replacement;
        }
        public string Handle() => Replacement;
    }
}
