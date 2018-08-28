using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FollowMachineDll.Attributes
{
    public class NodeAttribute : Attribute
    {
        #region MenuTitle

        private string _menuTitle = "";

        public string MenuTitle
        {
            get { return _menuTitle; }
            set { _menuTitle = value; }
        }

        #endregion
    }
}
