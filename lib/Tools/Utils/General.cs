// --------------------------------------------------------------------------------------------------------------------
// <copyright file="General.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the General type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.Tools.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;

    using LIB.Tools.BO;

    /// <summary>
    /// The general.
    /// </summary>
    public static class General
    {
        /// <summary>
        /// The add to the front of dictionary.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="itemToAdd">
        /// The item to add.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public static Dictionary<long, ItemBase> AddToTheFrontOfDictionary(Dictionary<long, ItemBase> source, ItemBase itemToAdd)
        {
            var resultCollection = new Dictionary<long, ItemBase> { { itemToAdd.Id, itemToAdd } };

            foreach (var item in source.Values)
            {
                resultCollection.Add(item.Id, item);
            }

            return resultCollection;
        }

        /// <summary>
        /// The string to integer.
        /// </summary>
        /// <param name="inp">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int StringToInt(string inp)
        {
            int res;
            int.TryParse(inp, out res);

            return res;
        }

        /// <summary>
        /// The trace write.
        /// </summary>
        /// <param name="val">
        /// The val.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool TraceWrite(object val)
        {
            if (null == HttpContext.Current || null == new HttpContextWrapper(HttpContext.Current))
            {
                return false;
            }

            new HttpContextWrapper(HttpContext.Current).Trace.Write(val.ToString());
            return true;
        }

        public static bool TraceWarn(object val)
        {
            if (null == new HttpContextWrapper(HttpContext.Current))
            {
                return false;
            }

            new HttpContextWrapper(HttpContext.Current).Trace.Warn(val.ToString());
            return true;
        }

        /// <summary>
        /// The hash string.
        /// </summary>
        /// <param name="sessionId">
        /// The p session id.
        /// </param>
        /// <returns>
        /// The <see cref="char[]"/>.
        /// </returns>
        public static char[] HashString(string sessionId)
        {
            char[] arrRet = { '0', '0' };
            if (null == sessionId)
            {
                return arrRet;
            }
            if (0 == sessionId.Length)
            {
                return arrRet;
            }

            // hash function will calculate the sum of all chars in pSessionId and get a number between 0 and 99 out of it
            var arrChars = sessionId.ToCharArray();
            var checkSum = arrChars.Sum(t => (int)t);
            var remainder = checkSum % 100;
            var strRemainder = string.Format("{0:00}", remainder);
            if (strRemainder.Length == 2)
            {
                return strRemainder.ToCharArray();
            }

            throw new ApplicationException("HashSessionId generated wrong hash");
        }

        /// <summary>
        /// The get md 5 hash.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var builder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return builder.ToString();
        }

        /// <summary>
        /// The get absolute path.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="request">
        /// The o request.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetAbsolutePath(string path, HttpRequest request)
        {
            var ipnurl = new StringBuilder(100);
            ipnurl.Append(request.IsSecureConnection ? "https://" : "http://");

            ipnurl.Append(request.Url.Authority);
            ipnurl.Append(request.ApplicationPath);

            // check for trailing slash, if it's not there - add it
            if ('/' != ipnurl[ipnurl.Length - 1])
            {
                ipnurl.Append('/');
            }
            ipnurl.Append(path);
            return ipnurl.ToString();
        }
    }
}