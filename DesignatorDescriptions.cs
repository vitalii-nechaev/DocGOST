using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocGOST
{
    class DesignatorDescriptions
    {
        public DesignatorDescriptions(string Designator, string Description, string DescriptionPlural)
        {
            this.Designator = Designator;
            this.Description = Description;
            this.DescriptionPlural = DescriptionPlural;
        }
        public string Designator { get; set; }
        public string Description { get; set; }
        public string DescriptionPlural { get; set; }
    }
}
