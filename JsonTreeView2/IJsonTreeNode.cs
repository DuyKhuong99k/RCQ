using System.Windows.Forms;

namespace JsonTreeView2
{
    interface IJsonTreeNode
    {
        /// <summary>
        /// To be called whenever the node is collapsing
        /// </summary>
        void AfterCollapse();

        /// <summary>
        /// To be called whenever the node is expanding
        /// </summary>
        void AfterExpand();

        /// <summary>
        /// To be called whenever the value of the json text is changed
        /// </summary>
        TreeNode AfterJsonTextChange(string newJson);
    }
}
