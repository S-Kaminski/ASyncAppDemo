using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asyncin4PPD3M0
{
    public class ProgressReportModel
    {
        public List<DemoClass> WebsitesDownloaded { get; set; } = new List<DemoClass>();
        public int Percentage { get; set; } = 0;
    }
}
