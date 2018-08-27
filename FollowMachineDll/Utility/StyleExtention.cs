using UnityEngine;

namespace FMachine.Utility
{
    public static class StyleExtention
    {
        public static float GetStyleWith(this GUIStyle style, string text)
        {
            float max;
            float min;
            style.CalcMinMaxWidth(new GUIContent(text), out min, out max);
            return
                style.padding.left +
                style.padding.right +
                style.border.left +
                style.border.right +
                max;
        }
        public static float GetStyleHeight(this GUIStyle style, string text)
        {
            float max;
            float min;
            var content = new GUIContent(text);
            style.CalcMinMaxWidth(content, out min, out max);

            return
                style.padding.top +
                style.padding.bottom +
                style.CalcHeight(content,max);
        }
    }
}
