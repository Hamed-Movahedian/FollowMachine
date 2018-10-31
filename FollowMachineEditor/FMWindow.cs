using FollowMachineDll.Utility;
using FollowMachineEditor.Utility;
using FollowMachineEditor.Windows.FollowMachineExplorer;
using UnityEditor;
using UnityEngine;

namespace FMachine.Editor
{
    public class FMWindow : FMWindowBase
    {

        #region Canvas

        private FMCanvas _canvas = null;

        public FMCanvas Canvas
        {
            get { return _canvas ?? (_canvas = new FMCanvas(this)); }
        }


        #endregion

        #region GraphStack

        [SerializeField]
        private GraphStack _graphStack;

        private Rect _explorerButtonRect;

        public GraphStack GraphStack => _graphStack ?? (_graphStack = new GraphStack(this));

        #endregion

        protected override void Initialize()
        {
            if (EditorTools.Instance == null)
                EditorTools.Instance = new EditorToolsImplimantation(this);

            EditorApplication.playmodeStateChanged += Repaint;
        }

        private void OnSelectionChange()
        {
            GraphStack.OnSelectinChange();
        }

        protected override void PerformOnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                {
                    GraphStack.OnGUI();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(Settings.Icon, EditorStyles.miniButton))
                    {
                        var canvasRect = _canvas.WindowRect;
                        PopupWindow.Show(_explorerButtonRect,new FollowMachineTreeViewWindow());
                    }

                    if (Event.current.type == EventType.Repaint)
                    {
                        _explorerButtonRect = GUILayoutUtility.GetLastRect();
                        _explorerButtonRect.x -= 300-_explorerButtonRect.width;
                    }
                }
                GUILayout.EndHorizontal();
                Canvas.OnGUI();
            }
            EditorGUILayout.EndVertical();

        }
    }
}
