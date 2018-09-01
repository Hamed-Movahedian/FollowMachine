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
            Node selectedNode = null;

            if (GraphStack.CurrentGraph != null)
                selectedNode = GraphStack.CurrentGraph.SelectedNode;

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical();
                {
                    GraphStack.OnGUI();

                    Canvas.OnGUI();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(GUILayout.MinWidth(selectedNode == null ? 10 : Settings.InspectorWith));
                
                if (selectedNode != null)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    selectedNode.OnInspector();
                }
                else
                {
                    GUILayout.Label("");
                }

                EditorGUILayout.EndVertical();

 
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
