using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace KobeiD.Extensions
{
    public static class String
    {
        public static string SkipCharSequence(this string _base, char[] charSeq, int h = 0)
            => (h < charSeq.Length) ? ((_base[h] == charSeq[h]) ? SkipCharSequence(_base, charSeq, h + 1) : _base.Substring(h, _base.Length - h)) : _base.Substring(h, _base.Length - h);

        public static string SkipPreceedingAndChar(this string _base, char singular, int h = 0)
            => (h < _base.Length) ? (_base[h] == singular ? _base.Substring(h + 1, _base.Length - (h + 1)) : SkipPreceedingAndChar(_base, singular, h + 1)) : _base;

        /// <summary>
        /// Deletes everything after the first whitespace detected.
        /// </summary>
        /// <param name="_base"></param>
        /// <param name="h"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string DeleteFollowingWhiteSpaceA(this string _base, int h = 0, int a = 0)
            => (h < _base.Length) ? (_base[h] != '\x20' ? DeleteFollowingWhiteSpaceA(_base, h + 1) : DeleteFollowingWhiteSpaceA(_base, h + 1, a + 1)) : _base.Substring(0, _base.Length - a);

        /// <summary>
        /// If the string has multiple spaces/whitespaces, use this one.
        /// </summary>
        /// <param name="_base"></param>
        /// <param name="h"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string DeleteFollowingWhiteSpaceB(this string _base, int h = 0, int a = 0)
            => (h < _base.Length) ? ((_base[h] != '\x20') ? DeleteFollowingWhiteSpaceA(_base, h + 1) : ((h < _base.Length - 1) ? (_base[h + 1] != '\x20' ? DeleteFollowingWhiteSpaceA(_base, h + 1) : DeleteFollowingWhiteSpaceA(_base, h + 1, a + 1)) : DeleteFollowingWhiteSpaceA(_base, h + 1, a + 1))) : _base.Substring(0, _base.Length - a);

        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new StringBuilder();
            sb.Capacity = str.Length;
            foreach (char c in str)
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == '\'')
                    sb.Append(c);
            return sb.ToString();
        }

        public static string GetFileName(this string str, int dif = 4)
        {
            int length = str.Length - dif;
            for(int idx = length; idx > 0; idx--)
                if (str[idx] == '\\')
                    return str.Substring(idx + 1, length - (idx + 1));
            return null;
        }

        public static string ParseFromRange(string[] arr, int n, int r)
        {
            StringBuilder sb = new StringBuilder() { Capacity = r - n };
            for (int idx = n; idx < r; idx++)
                sb.Append(arr[idx]);
            return sb.ToString();
        }
    }
}
