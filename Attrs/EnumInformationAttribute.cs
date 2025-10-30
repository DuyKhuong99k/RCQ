using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attrs
{
    [AttributeUsage(AttributeTargets.All)]
    public class EnumInformationAttribute : Attribute
    {
        public EnumInformationAttribute(string description)
        {
            this.Description = description;
        }

        public EnumInformationAttribute(string description, string view)
        {
            this.Description = description;
            this.View = view;
        }


        public string Description { get; set; }

        public string View { get; set; }
    }
}
