using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FMachine.Editor
{
    [Serializable]
    public class GraphStack
    {
        public FMWindow Window;
        public List<int> GraphList = new List<int>();

        public GraphStack(FMWindow window)
        {
            Window = window;
        }

        public Graph CurrentGraph
        {
            get
            {
                return GraphList == null ? null :
              GraphList.Count == 0 ? null : (Graph)EditorUtility.InstanceIDToObject(GraphList[GraphList.Count - 1]);
            }
        }

        public void ResetTo(Graph graph)
        {
            GraphList.Clear();
            if (graph == null)
                return;
            GraphList.Add(graph.GetInstanceID());
            graph.OnShow();
        }

        public void OnGUI()
        {
            if (GraphList.Count == 0)
                GUILayout.Label("Select a Graph");
            else
            {
                EditorGUILayout.BeginHorizontal();
                for (var i = 0; i < GraphList.Count; i++)
                {
                    Object o = EditorUtility.InstanceIDToObject(GraphList[i]);
                    Graph graph = (Graph) o;

                    if (graph == null)
                        continue;
                    if (GUILayout.Button(new GUIContent(graph.name), GUILayout.ExpandWidth(false)))
                        Select(graph);

                    if (i < GraphList.Count - 1)
                        GUILayout.Label(">", GUILayout.ExpandWidth(false));
                }

                EditorGUILayout.EndHorizontal();
            }

        }

        private void Select(Graph graph)
        {
            var next = GraphList.IndexOf(graph.GetInstanceID()) + 1;
            while (GraphList.Count > next)
                GraphList.RemoveAt(next);
            graph.OnShow();
        }

        public void Add(FollowMachine followmachine)
        {
            if (followmachine != null)
                GraphList.Add(followmachine.GetInstanceID());
            followmachine.OnShow();
        }

        public void OnSelectinChange()
        {
            var selectedGraph = GetSelectedGraph();

            if (selectedGraph != null)
                ResetTo(selectedGraph);
        }
        private Graph GetSelectedGraph()
        {
            var gameObject = Selection.activeGameObject;

            if (gameObject == null)
                return null;

            return gameObject.GetComponent<Graph>();
        }

    }
}