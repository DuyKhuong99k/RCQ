using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonTreeView2.Generic;
using Newtonsoft.Json.Linq;

namespace JsonTreeView2
{
    sealed class JValueTreeNode : JTokenTreeNode
    {
        #region >> Properties

        public JValue JValueTag => Tag as JValue;

        #endregion

        #region >> Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JValueTreeNode"/> class.
        /// </summary>
        /// <param name="jValue"></param>
        public JValueTreeNode(JToken jValue)
            : base(jValue)
        {
            ContextMenuStrip = SingleInstanceProvider<JValueContextMenuStrip>.Value;
        }

        #endregion

        #region >> JTokenTreeNode

        /// <inheritdoc />
        public override void AfterCollapse()
        {
            base.AfterCollapse();

            Text = GetAbstractTextForTag();
        }

        /// <inheritdoc />
        public override void AfterExpand()
        {
            base.AfterExpand();

            Text = GetAbstractTextForTag();
        }

        #endregion
    }
}
