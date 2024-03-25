using LIB.BusinessObjects;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.SessionState;

namespace Weblib.Helpers
{
    public class CultureHelper
    {
        protected HttpSessionState session;

        //constructor   
        public CultureHelper(HttpSessionState httpSessionState)
        {
            session = httpSessionState;
        }
        // Properties  
        public static Language Language
        {
            get
            {
                var llang = new Language() { Culture = Thread.CurrentThread.CurrentUICulture.Name };
                return (Language)llang.Populate(llang).Values.FirstOrDefault();
            }
            set
            {

                if (value != null && value.Id>0 && !string.IsNullOrEmpty(value.Culture))
                {
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(value.Culture);
                }
                else
                {
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
                }

                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;
            }
        }
    }
}
