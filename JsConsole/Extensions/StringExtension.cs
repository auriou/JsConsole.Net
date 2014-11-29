using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JsConsole.Extensions
{
    public static class StringExtension
    {
        public static bool Contains(this string mystring, List<string> listString)
        {
            return listString.Any(mystring.Contains);
        }

        public static bool IsEmpty(this string mystring)
        {
            return String.IsNullOrWhiteSpace(mystring);
        }

        public static string EncryptCesar(this string phrase, int decalage)
        {
            return String.Concat(phrase.Select(p => (char)(p + decalage)));
        }

        public static string DecryptCesar(this string phrase, int decalage)
        {
            return String.Concat(phrase.Select(p => (char)(p - decalage)));
        }

        public static string EncodeBase64(this string phrase)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(phrase));
        }

        public static string DecodeBase64(this string phrase)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(phrase));
        }
    }
}