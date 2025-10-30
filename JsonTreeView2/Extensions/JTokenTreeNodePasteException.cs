using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTreeView2.Extensions
{
    public class JTokenTreeNodePasteException : AggregateException
    {
        public JTokenTreeNodePasteException(Exception sourceException)
            : base(sourceException)
        {
        }
    }
}
