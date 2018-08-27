using System;
using FMachine.Shapes;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
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
                Transform category = _graph.transform.Find("Repository");

                if (category == null)
                {
                    GameObject cgo = new GameObject("Repository");
                    cgo.transform.parent = _graph.transform;
                    category = cgo.transform;
                }

                go = new GameObject(string.Format("{1} - ({0})", type.Name, category.childCount + 1));

                go.transform.parent = category;

            }

            var shape = go.AddComponent(type) as Shape;

            if (shape != null)
                shape.enabled = false;

            return shape;
        }

        public Node CreateNode(Type type, Vector2 position)
        {
            Node node = (Node)Create(type);

            _graph.NodeList.Add(node);

            node.OnCreate(_graph, position);

            return node;
        }

        public Socket CreateSocket(Node node, Type type)
        {
            Socket socket = (Socket)Create(type, node.gameObject);
            socket.OnCreate(_graph, node);
            return socket;
        }

        public Edge CreateEdge(Socket socket)
        {
            var edge = (Edge)Create(typeof(Edge), socket.gameObject);
            return edge;
        }

        public FollowMachine CreateFollowMachine(string name)
        {
            var go = new GameObject(name);
            go.transform.parent = _graph.transform.parent;
            return go.AddComponent<FollowMachine>();
        }
    }
}