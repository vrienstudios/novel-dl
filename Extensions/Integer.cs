﻿using System;
using System.Collections.Generic;
using System.Text;

namespace KobeiD.Extensions
{
    public static class Integer
    {
		public static int LeadingIntegralCount(this char[] str, int h = 0)
				=> h != str.Length ? ((str[h] >= '0' && str[h] <= '9') ? LeadingIntegralCount(str, h + 1) : h) : h;

        /// <summary>
        /// Get first integrels in a sequence
        /// </summary>
        /// <param name="str"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public static int FirstLIntegralCount(this char[] str, int h = 0, int num = 0, bool f = false)
            => h != str.Length ? (str[h] >= '0' && str[h] <= '9' ? FirstLIntegralCount(str, h + 1, (num * 10) + int.Parse(str[h].ToString()), true) : f == true ? num : FirstLIntegralCount(str, h + 1, num, f)) : (num);

		public static int CountFollowingWhiteSpace(this string str, int h, int i = 0)
				=> (h > 0) ? (str[h] == '\x20' ? CountFollowingWhiteSpace(str, h - 1, i + 1) : i) : i;

        /// <summary>
        /// Gets Greatest Common factors from one number. TODO: Implement odd-prime number factors with ferment's theorem
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int[] GCFS(this int n)
        {
            List<int> f = new List<int>();
            for (int x = 1; x <= n; x++)
                if (n % x == 0)
                    f.Add(x);
            int m = f.Count - 1;
            return f[f.Count / 2] * f[f.Count / 2] == n ? new int[] { f[f.Count / 2], f[f.Count / 2] } : f.Count > 3 ? new int[] { f[m / 2], f[(m / 2) + 1] } : f.Count < 3 ? new int[] { f[0], f[1] } : new int[] { f[m / 2], f[m / 2] };
        }

        public static int[] GetPrimeFactors(int n)
        {
            switch (n)
            {
                case 1: return new int[] { 1, 1 };
                case 2: return new int[] { 2, 1 };
                default:
                    {
                        double a = Math.Ceiling(Math.Sqrt(n));
                        return null;
                    }
            }
        }
    }
}
