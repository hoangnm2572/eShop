using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Services.Helpers
{
    public static class StringHelper
    {
        public static string GenerateUsername(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "storeuser";

            string normalized = input.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            string noAccents = sb.ToString().Normalize(NormalizationForm.FormC);

            noAccents = noAccents.Replace("Đ", "D").Replace("đ", "d");

            return Regex.Replace(noAccents, @"[^a-zA-Z0-9]", "").ToLower();
        }
    }
}
