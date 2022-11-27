using System;
using System.Collections.Generic;
using System.Linq;

namespace DoorManager.Service.Extensions;

public static class StringExtensions
{
    public const string DefaultSeperator = ",";

    public static IEnumerable<string> SplitTextToArray(this string inputText, string[] seperators = null)
    {
        if (seperators == null)
        {
            seperators = new[] { DefaultSeperator };
        }

        return inputText.Split(seperators, StringSplitOptions.TrimEntries).Where(x => !string.IsNullOrEmpty(x));
    }
}
