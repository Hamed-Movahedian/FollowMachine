using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bind.Internal;
using MgsCommonLib.Theme;
using UnityEngine;

namespace FollowMachineDll.DataTypes
{
    [Serializable]
    public class LText
    {
        [SerializeField]
        private bool _isConst;

        public string Text;

        public LText(bool isConst, string text)
        {
            _isConst = isConst;
            Text = text;
        }

        public bool IsConst
        {
            get => _isConst;
            set => _isConst = value;
        }

        public override string ToString()
        {
            return _isConst ? Text : ThemeManager.Instance.LanguagePack.GetLable(Text);
        }

        public static implicit operator string(LText lText)
        {
            return lText.ToString();
        }

    }
}
