using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder TrimEnd(this StringBuilder sb, char ch = ' ')
        {
            if (sb == null || sb.Length == 0) return sb;


            if (sb[sb.Length - 1] == ch)
            {
                sb.Length--;
            }

            return sb;
        }
    }
}
