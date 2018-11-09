using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bind.Internal;
using UnityEditor;
using UnityEngine;

namespace BindEditor.Internal
{
    internal class BindWindow : EditorWindow
    {
        private BindValue _targetBindValue;
        private BindValue _currentValue;

        public static void Show(BindValue bindValue)
        {
            var bindWindow = GetWindow<BindWindow>();
            bindWindow.SetNewTarget(bindValue);
        }

        private void SetNewTarget(BindValue bindValue)
        {
            _targetBindValue = bindValue;
            _currentValue = bindValue.Clone();
        }

        private void OnGUI()
        {
            _currentValue.Source =
                (GameObject)EditorGUILayout.ObjectField("Source", _currentValue.Source, typeof(GameObject), true);
        
            if (_currentValue.Source == null)
                return;

            BindEditorUtility.GetComponent(_currentValue);

            for (int i = 0; i < _currentValue.MemberInfo.Count; i++)
            {
                MemberGUI(i);
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
                AddMember();
            if (GUILayout.Button("Bind"))
                Bind();

        }

        private void Bind()
        {
            if (_currentValue.IsValid)
            {
                _currentValue.Copy(_targetBindValue);
                Close();
            }
            else
                EditorUtility.DisplayDialog("Binding", "Unable to bound!!", "OK");
        }

        private void AddMember()
        {
            List<MemberInfo> members = BindEditorUtility.NewMembersList(_currentValue);

            var menu = new GenericMenu();

            foreach (var m in members)
                menu.AddItem(
                    new GUIContent($"{m.DeclaringType.FrindlyName()}/{m.MemberType}/{m.FrindlyName()}"),
                    false,
                    o => { _currentValue.MemberInfo.Add(new SMemberInfo((MemberInfo)o)); },
                    m);

            menu.ShowAsContext();
        }

        private void MemberGUI(int index)
        {
            var memberInfo = _currentValue.MemberInfo[index];

            GUILayout.BeginVertical((GUIStyle)"box",GUILayout.ExpandWidth(true));
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(memberInfo.FrindlyName());

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("X", EditorStyles.miniButtonRight, GUILayout.Width(20)))
                    {
                        while (_currentValue.MemberInfo.Count > index)
                        {
                            _currentValue.MemberInfo.Remove(_currentValue.MemberInfo.Last());
                        }
                    }

                }
                GUILayout.EndHorizontal();

                if (memberInfo.IsMethod)
                {
                    var parameterInfos = memberInfo.MethodInfo.GetParameters();

                    for (var i = 0; i < memberInfo.Parameters.Count; i++)
                    {
                        memberInfo.Parameters[i].OnGUI(parameterInfos[i].Name);
                    }
                }
            }
            GUILayout.EndVertical();
        }
    }
}
