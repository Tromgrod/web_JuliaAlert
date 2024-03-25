using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace JuliaAlert.Helpers
{
    public class T
    {
        public static string Str(string alias, string ResourceFile, string defaultvalue="")
        {
            return LIB.Tools.Utils.Translate.GetTranslatedValue(alias, ResourceFile,!String.IsNullOrEmpty(defaultvalue)? defaultvalue: alias);
        }
    }
}