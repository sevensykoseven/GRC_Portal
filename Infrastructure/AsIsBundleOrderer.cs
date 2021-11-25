using System.Collections.Generic;
using System.Web.Optimization;

namespace protean.Infrastructure
{

    /// <summary>
    /// Used to return the order of the bundle as each script/style is added.
    /// </summary>
    public sealed class AsIsBundleOrderer : IBundleOrderer
    {
        public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }
    }
}