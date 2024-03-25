using System.Collections.Generic;
using System.Web.Optimization;

namespace WebLib.UI
{
    public partial class AsIsBundleOrderer : IBundleOrderer
    {
        public virtual IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }
    }
}
