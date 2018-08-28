using System;

namespace FollowMachineDll.Attributes
{
    public class FollowMachineAttribute : Attribute
    {
        #region Info

        private string _info ;

        public string Info
        {
            get => _info;
            set => _info = value;
        }

        #endregion

        #region string

        private string _outputs ;

        public string Outputs
        {
            get { return _outputs; }
            set { _outputs = value; }
        }

        #endregion

        public FollowMachineAttribute(string info, string outputs="")
        {
            _info = info;
            _outputs = outputs;
        }


    }
}
