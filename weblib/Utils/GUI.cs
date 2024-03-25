using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weblib.Utils
{
    public class GUI
    {
        public static string LoadDashBoradButtonClass(int ind)
        {
            var result = "btn-primary";

            //Random rand= new Random();

            switch (ind)
            {
                case 0:
                    return "btn-primary";
                case 1:
                    return "btn-success";
                case 2:
                    return "btn-info";
                case 3:
                    return "btn-warning";
                case 4:
                    return "btn-danger";
                case 5:
                    return "btn-default";
            }

            return result;
        }
    }
}
