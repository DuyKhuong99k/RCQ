using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using JsonTreeView2.Generic;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace JsonTreeView2
{
    sealed class JArrayTreeNode : JTokenTreeNode
    {
         #region >> Properties

        public JArray JArrayTag => Tag as JArray;

        #endregion

        #region >> Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JArrayTreeNode"/> class.
        /// </summary>
        public JArrayTreeNode(JArray jArray)
            : base(jArray)
        {
            ContextMenuStrip = SingleInstanceProvider<JArrayContextMenuStrip>.Value;
        }

        #endregion

        #region >> JTokenTreeNode

        /// <inheritdoc />
        public override void AfterCollapse()
        {
            base.AfterCollapse();

            Text = $@"[{JArrayTag.Type}] {GetAbstractTextForTag()}";
        }

        /// <inheritdoc />
        public override void AfterExpand()
        {
            base.AfterExpand();

            Text = $@"[{JArrayTag.Type}]";
        }

        #endregion
    }
}
