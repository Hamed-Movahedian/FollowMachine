using System;
using FMachine;
using FMachine.Shapes;
using FMachine.Shapes.Nodes;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Base;
using FollowMachineDll.Shapes;
using FollowMachineDll.Shapes.Nodes;
using FollowMachineEditor.EditorObjects.EditorGraph;
using FollowMachineEditor.EditorObjects.EditorShapes;
using FollowMachineEditor.EditorObjects.EditorShapes.EditorBoxShapes;
using FollowMachineEditor.EditorObjects.EditorShapes.EditorBoxShapes.EditorNodes;
using FollowMachineEditor.EditorObjects.EditorShapes.EditorBoxShapes.EditorSockets;

namespace FollowMachineEditor.EditorObjectMapper
{
    public static class EditorObjectMapper
    {

        #region Base class mapping
        public static EGroup Editor(this Group shape)
        {
            return (EGroup)(shape.EObject ?? (shape.EObject = new EGroup(shape)));
        }

        public static EEdge Editor(this Edge edge)
        {
            if(edge==null)
                throw new NullReferenceException($"Edge is null!!!");
            return (EEdge)(edge.EObject ?? (edge.EObject = new EEdge(edge)));
        }

        public static EInputSocket Editor(this InputSocket inputSocket)
        {
            return (EInputSocket)(inputSocket.EObject ?? (inputSocket.EObject = new EInputSocket(inputSocket)));
        }

        public static ESocket Editor(this Socket socket)
        {
            if (socket.EObject != null)
                return (ESocket) socket.EObject;

            switch (socket)
            {
                case InputSocket inputSocket:
                    return inputSocket.Editor();
                case OutputSocket outputSocket:
                    return outputSocket.Editor();
            }

            throw new InvalidCastException($"Type {socket.GetType().Name} can't cast to editor counterpart!");
        }

        public static EOutputSocket Editor(this OutputSocket outputSocket)
        {
            return (EOutputSocket)(outputSocket.EObject ?? (outputSocket.EObject = new EOutputSocket(outputSocket)));
        }

        public static EGraph Editor(this Graph graph)
        {
            return (EGraph)(graph.EObject ?? (graph.EObject = new EGraph(graph)));
        }

        public static EFollowMachine Editor(this FollowMachine followMachine)
        {
            return (EFollowMachine)(followMachine.EObject ?? (followMachine.EObject = new EFollowMachine(followMachine)));
        }

        #endregion

        // *************** Node mapping



        private static ENode CreateEditorNode(Node node)
        {
            switch (node)
            {
                case DynamicActionNode dynamicActionNode:
                    return new EDynamicActionNode(dynamicActionNode);
                case ActionNode actionNode:
                    return new EActionNode(actionNode);
                case DialogeNode dialogeNode:
                    return new EDialogeNode(dialogeNode);
                case EntryPointNode entryPointNode:
                    return new EEntryPointNode(entryPointNode);
                case FollowMachineNode followMachineNode:
                    return new EFollowMachineNode(followMachineNode);
                case InputNode inputNode:
                    return new EInputNode(inputNode);
                case OutputNode outputNode:
                    return new EOutputNode(outputNode);
                case WindowNode windowNode:
                    return new EWindowNode(windowNode);
                case AnimationNode animationNode:
                    return new EAnimationNode(animationNode);
                case EventNode eventNode:
                    return new EEventNode(eventNode);
                case NullNode nullNode:
                    return new ENullNode(nullNode);
                case ProgressNode progressNode:
                    return new EProgressNode(progressNode);
                case ServerNode serverNode:
                    return new EServerNode(serverNode);
                case SetProperty setProperty:
                    return new ESetProperty(setProperty);
            }

            throw new InvalidCastException($"Type {node.GetType().Name} can't cast to editor counterpart!");
        }

        public static T Editor<T>(this Node node) where T:ENode
        {
            return (T)(node.EObject ?? (node.EObject = (EObject) Activator.CreateInstance(typeof(T), node)));
        }
        public static ENode Editor(this Node node)
        {
            return (ENode)(node.EObject ?? (node.EObject = CreateEditorNode(node)));
        }
        public static EActionNode Editor(this ActionNode node) => 
            Editor<EActionNode>(node);

        public static EDynamicActionNode Editor(this DynamicActionNode node) =>
            Editor<EDynamicActionNode>(node);

        public static EDialogeNode Editor(this DialogeNode node) =>
            Editor<EDialogeNode>(node);

        public static EAnimationNode Editor(this AnimationNode node) =>
            Editor<EAnimationNode>(node);

        public static EEventNode Editor(this EventNode node) =>
            Editor<EEventNode>(node);

        public static EEntryPointNode Editor(this EntryPointNode node) =>
            Editor<EEntryPointNode>(node);

        public static EFollowMachineNode Editor(this FollowMachineNode node) =>
            Editor<EFollowMachineNode>(node);

        public static EInputNode Editor(this InputNode node) =>
            Editor<EInputNode>(node);

        public static ENullNode Editor(this NullNode node) =>
            Editor<ENullNode>(node);
        public static EOutputNode Editor(this OutputNode node) =>
            Editor<EOutputNode>(node);

        public static EProgressNode Editor(this ProgressNode node) =>
            Editor<EProgressNode>(node);

        public static EServerNode Editor(this ServerNode node) =>
            Editor<EServerNode>(node);

        public static ESetProperty Editor(this SetProperty node) =>
            Editor<ESetProperty>(node);

        public static EWindowNode Editor(this WindowNode node) =>
            Editor<EWindowNode>(node);

    }
}
