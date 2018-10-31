using System;
using System.Collections.Generic;
using FMachine.Shapes;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Shapes;
using FollowMachineEditor.EditorObjectMapper;
using UnityEditor;
using UnityEngine;

namespace FMachine
{
    public class ShapeRepository
    {
        private Graph _graph;

        public ShapeRepository(Graph graph)
        {
            _graph = graph;
        }

        public Shape Create(Type type, GameObject go = null)
        {
            if (go == null)
            {
                Transform repository = GetRepository();

                go = new GameObject(string.Format("{1} - ({0})", type.Name, repository.childCount + 1));

                Undo.RegisterCreatedObjectUndo(go,"");

                Undo.SetTransformParent(go.transform, repository,"");
            }

            var shape = Undo.AddComponent(go, type) as Shape;

            return shape;
        }

        private Transform GetRepository()
        {
            Transform repository = _graph.transform.Find("Repository");

            if (repository == null)
            {
                GameObject cgo = new GameObject("Repository");

                Undo.RegisterCreatedObjectUndo(cgo, "");

                Undo.SetTransformParent(cgo.transform,_graph.transform,"");

                repository = cgo.transform;
            }

            return repository;
        }

        public Node CreateNode(Type type, Vector2 position)
        {
            Node node = (Node)Create(type);

            Undo.RegisterCompleteObjectUndo(_graph,"Create Node");
            _graph.NodeList.Add(node);

            node.Editor().OnCreate(_graph, position);

            return node;
        }
        public Group CreateGroup(List<Node> nodes)
        {
            Group @group = (Group)Create(typeof(Group));

            Undo.RegisterCompleteObjectUndo(_graph,"");

            _graph.GroupList.Add(@group);

            @group.Editor().OnCreate(_graph,nodes);

            return @group;
        }

        public Socket CreateSocket(Node node, Type type)
        {
            Socket socket = (Socket)Create(type, node.gameObject);
            socket.Editor().OnCreate(_graph, node);
            return socket;
        }

        public Edge CreateEdge(Socket socket)
        {
            return (Edge)Create(typeof(Edge), socket.gameObject);
        }

        public FollowMachine CreateFollowMachine(string name)
        {
            var go = new GameObject(name);

            Undo.RegisterCreatedObjectUndo(go,"");

            Undo.SetTransformParent(go.transform, _graph.transform, "");

            return Undo.AddComponent<FollowMachine>(go);
        }

        public void Add(Node node)
        {
            Undo.SetTransformParent(node.transform,GetRepository(),"");
            _graph.NodeList.Add(node);
            node.Editor().SetGraph(_graph);
        }

        public void Remove(Node node)
        {
            //Undo.RecordObject(_graph,"");
            _graph.NodeList.Remove(node);
            
        }
    }
}