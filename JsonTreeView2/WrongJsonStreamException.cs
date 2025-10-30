using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTreeView2
{
    public class WrongJsonStreamException : Exception
    {
        public WrongJsonStreamException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
