using System;
using System.Collections;
using MgsCommonLib.UI;

namespace FollowMachineDll.Utility
{
    [Serializable]
    public class ProgressBarInfo
    {
        public MgsProgressWindow ProgressbarWindow;

        public string ProgressbarMessage;

        public bool ProgressbarShow = true;

        public bool ProgressbarHide = true;

        public IEnumerator Show()
        {
            if (ProgressbarWindow == null || !ProgressbarShow)
                return null;

            return ProgressbarWindow.Display(ProgressbarMessage, ProgressbarShow);
        }

        public IEnumerator Hide()
        {
            if (ProgressbarWindow == null || !ProgressbarHide)
                return null;

            return ProgressbarWindow.Hide();

        }
    }
}