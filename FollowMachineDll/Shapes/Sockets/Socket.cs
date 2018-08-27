using System.Collections.Generic;
using FMachine.SettingScripts;
using FMachine.Shapes.Nodes;
using FollowMachineDll.Utility;
using UnityEngine;

namespace FMachine.Shapes.Sockets
{
    public abstract class Socket : BoxShape
    {
        public Node Node;
        protected int Index;
        public List<Edge> EdgeList = new List<Edge>();

        public abstract SocketSetting SocketSetting { get; }

        public virtual void OnCreate(Graph graph,Node node)
        {
            Graph = graph;
            Node = node;
        }

        public override void Draw()
        {
            base.Draw();
            Rect.size = SocketSetting.Size;
        }



        protected void CreateEdge(InputSocket inputSocket, OutputSocket outputSocket)
        {
            Edge edge = Graph.Repository.CreateEdge(inputSocket);
            edge.InputSocket = inputSocket;
            edge.OutputSocket = outputSocket;
            inputSocket.EdgeList.Add(edge);
            outputSocket.EdgeList.Add(edge);
        }

        public override void Delete()
        {
            var edges = new List<Edge>(EdgeList);

            foreach (var edge in edges)
                edge.Delete();

            DestroyImmediate(this);
        }
    }
}