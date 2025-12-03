using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Models
{
    class About
    {
        public string Title => AppInfo.Name;
        public string Version => AppInfo.VersionString;

        public string MoreInfoUr1 => "https://thecodercave.com";
    }
}
