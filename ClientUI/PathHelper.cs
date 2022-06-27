using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientUI
{
    internal static class PathHelper
    {
        internal static string ExeDir => new FileInfo(typeof(PathHelper).Assembly.Location).DirectoryName;
    }
}
