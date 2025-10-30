using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonTreeView2.Generic;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace JsonTreeView2
{
    sealed class JObjectTreeNode : JTokenTreeNode
    {
        #region >> Properties

        public JObject JObjectTag => Tag as JObject;

        #endregion

        #region >> Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JObjectTreeNode"/> class.
        /// </summary>
        public JObjectTreeNode(JObject jObject)
            : base(jObject)
        {
            ContextMenuStrip = SingleInstanceProvider<JObjectContextMenuStrip>.Value;
        }

        #endregion

        #region >> JTokenTreeNode

        /// <inheritdoc />
        public override void AfterCollapse()
        {
            base.AfterCollapse();

            Text = $@"{{{JObjectTag.Type}}} {GetAbstractTextForTag()}";
        }

        /// <inheritdoc />
        public override void AfterExpand()
        {
            base.AfterExpand();

            Text = $@"{{{JObjectTag.Type}}}";
        }

        #endregion
    }
}
