using System.Text;

namespace ZeepSDK.Extensions;

/// <summary>
/// Provides extension methods for <see cref="StringBuilder"/>.
/// </summary>
public static class StringBuilderExtensions
{
    /// <summary>
    /// Appends the specified string value followed by a carriage return and line feed (CRLF) to the string builder.
    /// </summary>
    /// <param name="builder">The string builder to append to.</param>
    /// <param name="value">The string value to append.</param>
    /// <returns>A reference to this instance after the append operation.</returns>
    public static StringBuilder AppendCLRF(this StringBuilder builder, string value)
    {
        builder.Append(value);
        builder.Append("\r\n");
        return builder;
    }
}