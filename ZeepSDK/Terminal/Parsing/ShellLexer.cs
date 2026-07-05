using System.Collections.Generic;
using System.Text;

namespace ZeepSDK.Terminal.Parsing;

internal static class ShellLexer
{
    public static List<string> Tokenize(string input)
    {
        var tokens = new List<string>();
        if (string.IsNullOrWhiteSpace(input))
            return tokens;

        var builder = new StringBuilder();
        bool inQuotes = false;

        for (var i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (inQuotes)
            {
                if (c == '\\' && i + 1 < input.Length)
                {
                    builder.Append(input[++i]);
                    continue;
                }

                if (c == '"')
                {
                    tokens.Add(builder.ToString());
                    builder.Clear();
                    inQuotes = false;
                    continue;
                }

                builder.Append(c);
                continue;
            }

            if (char.IsWhiteSpace(c))
            {
                if (builder.Length > 0)
                {
                    tokens.Add(builder.ToString());
                    builder.Clear();
                }

                continue;
            }

            if (c == '"')
            {
                if (builder.Length > 0)
                {
                    tokens.Add(builder.ToString());
                    builder.Clear();
                }

                inQuotes = true;
                continue;
            }

            builder.Append(c);
        }

        if (builder.Length > 0)
            tokens.Add(builder.ToString());

        return tokens;
    }
}
