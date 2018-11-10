using Bind;
using BindEditor;
using FMachine.Shapes.Sockets;
using FollowMachineDll.Shapes.Nodes;
using FollowMachineDll.Utility;
using FollowMachineEditor.CustomInspectors;
using MgsCommonLib.Utilities;
using UnityEditor;
using UnityEngine;

namespace FollowMachineEditor.EditorObjects.EditorShapes.EditorBoxShapes.EditorNodes
{
    public class ESetProperty : ENode
    {
        private readonly SetProperty _setProperty;

        public ESetProperty(SetProperty setProperty) : base(setProperty)
        {
            _setProperty = setProperty;
        }


        protected override void Initialize()
        {
            AddInputSocket<OutputSocket>("");

            AddOutputSocket<InputSocket>("");
        }
        public override void OnInspector()
        {
            base.OnInspector();

            GUIUtil.BoldSeparator();

            foreach (var assinment in _setProperty.Assinments)
            {
                assinment.AssinmentGUI();
                if (GUILayout.Button("Remove"))
                {
                    _setProperty.Assinments.Remove(assinment);
                    return;
                }
                EditorGUILayout.Space();
                GUIUtil.BoldSeparator();
                EditorGUILayout.Space();
            }

            if (GUILayout.Button("Add"))
                _setProperty.Assinments.Add(new Assinment());
        }

    }
}
