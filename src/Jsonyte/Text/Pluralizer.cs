namespace Jsonyte.Text
{
    internal static class Pluralizer
    {
        // Consider borrowing from https://github.com/dotnet/ef6/blob/main/src/EntityFramework/Infrastructure/Pluralization/EnglishPluralizationService.cs
        public static string Pluralize(string word)
        {
            if (word.EndsWith("s"))
            {
                return word;
            }

            return $"{word}s";
        }
    }
}
