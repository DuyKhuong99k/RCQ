using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTreeView2.Generic
{
    sealed class SingleInstanceProvider<T> where T : class, new()
    {
        #region >> Fields

        static T _instance;

        #endregion

        #region >> Properties

        /// <summary>
        /// Get the <typeparamref name="T"/> instance stored in this singleton.
        /// The instance is created on the first call.
        /// </summary>
        public static T Value => _instance ?? (_instance = new T());

        #endregion
    }
}
