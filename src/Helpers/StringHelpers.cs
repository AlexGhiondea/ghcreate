using System;

namespace Creator.Helpers
{
    public static class StringHelpers
    {
        public static string EncodeString(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            bool hasDoubleQuotes = input.IndexOf("\"") >= 0;
            // if we have a comma or a newline or a quote, include it in double quotes.
            bool hasComma = input.IndexOf(",") >= 0;
            bool hasNewLine = input.IndexOf(Environment.NewLine) >= 0;

            if (hasComma || hasNewLine || hasDoubleQuotes)
            {
                // needs to be encoded
                if (hasDoubleQuotes)
                {
                    input = input.Replace("\"", "\"\"");
                }
                return "\"" + input + "\"";
            }

            return input;
        }
    }
}
