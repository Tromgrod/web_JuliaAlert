using LIB.AdvancedProperties;
using LIB.Tools.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace JuliaAlertLib.BusinessObjects
{
    public abstract class PrintBase : ModelBase
    {
        public PrintBase()
            : base(0)
        {
        }

        public PrintBase(long Id)
            : base(Id)
        {
        }

        public PrintBase(Dictionary<string, string> Filters, ControllerContext ControllerContext, ViewDataDictionary ViewData, TempDataDictionary TempData)
            : base(0)
        {
            this.Filters = Filters;
            LoadReport(ControllerContext, ViewData, TempData);
        }

        [Db(_Editable = false, _Ignore = true, _Populate = false)]
        public Dictionary<string, string> Filters { get; set; }

        [Db(_Editable = false, _Ignore = true, _Populate = false)]
        public string PrintTemplate { get; set; }

        [Db(_Editable = false, _Ignore = true, _Populate = false)]
        public string FileDownloadName { get; set; }

        [Db(_Editable = false, _Ignore = true, _Populate = false)]
        public string StylesFile { get; set; }

        public abstract string LoadReport(ControllerContext ControllerContext, ViewDataDictionary ViewData, TempDataDictionary TempData, ExportType type= ExportType.None);
    }
    public enum ExportType : int
    {
        None =0,
        Word = 1
    }
}
