// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestResult.cs" company="GalexStudio">
//   Copyright 2013
// </copyright>
// <summary>
//   Defines the RequestResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.Helpers
{
    using System.Collections.Generic;

    public class RequestResult
    {
        public RequestResultType Result { get; set; }

        public string Message { get; set; }

        public string RedirectURL { get; set; }

        public List<string> ErrorFields { get; set; }

        public Dictionary<string,object> Data { get; set; }
    }
}