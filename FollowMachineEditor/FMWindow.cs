using FollowMachineDll.Utility;
using FollowMachineEditor.Utility;
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

            GraphStack.OnGUI();

            Canvas.OnGUI();

            EditorGUILayout.EndVertical();

        }
    }
}
