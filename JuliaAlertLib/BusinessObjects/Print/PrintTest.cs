using LIB.AdvancedProperties;
using LIB.Tools.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace JuliaAlertLib.BusinessObjects
{
    public class PrintTest: PrintBase
    {
        public PrintTest()
            : base(0)
        {
        }

        public PrintTest(long Id)
            : base(Id)
        {
        }

        public override string LoadReport(ControllerContext ControllerContext, ViewDataDictionary ViewData, TempDataDictionary TempData, ExportType type = ExportType.None)
        {
            throw new NotImplementedException();
        }
    }
}
