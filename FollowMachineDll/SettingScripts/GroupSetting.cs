using UnityEngine;

namespace FollowMachineDll.SettingScripts
{
    [CreateAssetMenu(menuName = "FollowMachine/GroupSetting")]
    public class GroupSetting : ScriptableObject
    {
        #region TempStyle

        private  GUIStyle _tempStyle;

        public  GUIStyle TempStyle =>
            _tempStyle ?? (_tempStyle = new GUIStyle(Style));

        #endregion

        public GUIStyle Style;

        public void SetupZoom(float zoom)
        {
            zoom = Mathf.Max(0.3f, zoom);
            TempStyle.fontSize = (int) (Style.fontSize / zoom);
            TempStyle.padding.top = (int) (Style.padding.top / zoom);
            TempStyle.padding.left = (int) (Style.padding.left / zoom);
        }
    }
}