using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cassette.RequireJS
{
    public static class RequireJSSettings
    {
        public static  Func<string, string> ConvertAssetPathToModulePathOverride { get; set; }

        internal static string ConvertAssetPathToModulePathCompleted(string assetPath)
        {
            if (ConvertAssetPathToModulePathOverride == null)
            {
                return assetPath;
            }

            return ConvertAssetPathToModulePathOverride(assetPath);
        }
    }
}
