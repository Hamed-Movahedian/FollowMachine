using FollowMachineDll.Utility;
using FollowMachineEditor.Utility;
using UnityEngine;

namespace FMachine.Editor
{
    class EditorZoomArea
    {
        private static Matrix4x4 _prevGuiMatrix;
        private static Vector3 _tmpScaleVector = new Vector3();
        private static Rect _tmpRect = new Rect();
        private static Rect _screenArea;

        public static Rect Begin(float zoomScale, Rect screenCoordsArea)
        {
            _screenArea = screenCoordsArea;

            GUI.EndGroup();

            Rect clippedArea = new Rect(
                screenCoordsArea.position,
                screenCoordsArea.size / zoomScale);

            clippedArea.y += GUI.skin.window.border.top;

            GUI.BeginGroup(clippedArea);
            _prevGuiMatrix = GUI.matrix;
            Matrix4x4 translation = Matrix4x4.TRS(clippedArea.TopLeft(), Quaternion.identity, Vector3.one);
            _tmpScaleVector.Set(zoomScale, zoomScale, 1.0f);
            Matrix4x4 scale = Matrix4x4.Scale(_tmpScaleVector);
            GUI.matrix = translation * scale * translation.inverse * GUI.matrix;
            return clippedArea;
        }

        public static void End()
        {
            GUI.matrix = _prevGuiMatrix;
            GUI.EndGroup();
            //_tmpRect.Set(0, SssmSettings.Setting.TopOffset, Screen.width, Screen.height);
            _tmpRect.Set(0, _screenArea.y, Screen.width, Screen.height);
            GUI.BeginGroup(_tmpRect);
        }
    }

}
