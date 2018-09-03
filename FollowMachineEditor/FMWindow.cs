using FMachine.Shapes.Nodes;
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

        public GraphStack GraphStack;

        #endregion


        protected override void Initialize()
        {
            if (EditorTools.Instance == null)
                EditorTools.Instance = new EditorToolsImplimantation(this);

            //EditorToolsImplimantation.SetInstance();

            EditorApplication.playmodeStateChanged += Repaint;
            if (GraphStack == null)
                GraphStack = new GraphStack(this);
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
