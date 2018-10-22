using System;
using System.Collections.Generic;
using FMachine.Shapes;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Shapes;
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

                go.transform.parent = repository;

            }

            var shape = go.AddComponent(type) as Shape;


            return shape;
        }

        private Transform GetRepository()
        {
            Transform repository = _graph.transform.Find("Repository");

            if (repository == null)
            {
                GameObject cgo = new GameObject("Repository");
                cgo.transform.parent = _graph.transform;
                repository = cgo.transform;
            }

            return repository;
        }

        public Node CreateNode(Type type, Vector2 position)
        {
            Node node = (Node)Create(type);

            _graph.NodeList.Add(node);

            node.OnCreate(_graph, position);

            return node;
        }
        public Group CreateGroup(List<Node> nodes)
        {
            Group @group = (Group)Create(typeof(Group));

            _graph.GroupList.Add(@group);

            @group.OnCreate(_graph,nodes);

            return @group;
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

        public void Add(Node node)
        {
            node.transform.parent = GetRepository();
            _graph.NodeList.Add(node);
            node.SetGraph(_graph);
        }

        public void Remove(Node node)
        {
            _graph.NodeList.Remove(node);
            
        }
    }
}