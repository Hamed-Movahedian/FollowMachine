using System.Collections;
using System.Collections.Generic;
using FMachine.Shapes.Sockets;

namespace FMachine.Shapes.Nodes
{
    public abstract class Node : BoxShape
    {
        #region Public fields

        public List<OutputSocket> InputSocketList = new List<OutputSocket>();
        public List<InputSocket> OutputSocketList = new List<InputSocket>();
        public InputSocket DefaultOutputSocket;
        public bool Active = true;
        public OutputSocket EnteredSocket { get; set; }

        #endregion


        #region Run
        internal IEnumerator RunBase()
        {
            Graph.RunningNode = this;
            yield return Run();

            if (DefaultOutputSocket.IsConnected)
                yield return (Graph as FollowMachine).RunNode(DefaultOutputSocket.GetNextNode());

            
        }
        protected virtual IEnumerator Run()
        {
            return null;
        }
        public abstract Node GetNextNode();


        #endregion

    }
}