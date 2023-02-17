namespace WordFinderApp.Helpers
{
    internal class ReadTextHelper
    {
        public static List<string> ReadTextFromFile(string filePath)
        {
            List<string> textLines = new List<string>();
            using (StreamReader sr = new StreamReader(filePath))
            {
                string? line;
                while((line = sr.ReadLine()) != null)
                {
                    textLines.Add(line);
                }
            }
            return textLines;
        }

        public static string? GetSearchRegExpression(int userChoise, string searchWord)
        {
            switch (userChoise)
            {
                case 1:
                    // Common search: all matches.
                    return @$"\w*{searchWord}\w*";
                case 2:
                    // Exact search: exactly this word.
                    return @$"\b{searchWord}\b";
                default:
                    return null;
            }
        }
    }
}
