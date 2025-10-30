using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTreeView2.Extensions
{
    public class JTokenTreeNodeDeleteException: AggregateException
    {
        public JTokenTreeNodeDeleteException(Exception sourceException)
            : base(sourceException)
        {
        }
    }
}
