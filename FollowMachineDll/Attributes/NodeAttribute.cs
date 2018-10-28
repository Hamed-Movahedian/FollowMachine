using System;

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
