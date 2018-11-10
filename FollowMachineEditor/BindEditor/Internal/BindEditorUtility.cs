using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bind;
using Bind.Internal;
using FollowMachineEditor.CustomInspectors;
using UnityEditor;
using UnityEngine;

namespace BindEditor.Internal
{
    public static class BindEditorUtility
    {
        internal static string FrindlyName(this SMemberInfo memberInfo)
        {
            return memberInfo.Value.FrindlyName();
        }

        internal static Type Type(this MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)memberInfo).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).PropertyType;
                default:
                    throw new ArgumentOutOfRangeException(memberInfo.MemberType.ToString());
            }
        }

        internal static string FrindlyName(this MemberInfo memberInfo)
        {
            var name = memberInfo.Name;

            if (memberInfo.MemberType == MemberTypes.Method)
                name += ((MethodInfo)memberInfo)
                        .GetParameters()
                        .Aggregate("(", (c, p) =>
                            $"{c}{(c == "(" ? "" : ", ")}{p.ParameterType.FrindlyName()} {p.Name}")
                        + ")";

            name += $" ({memberInfo.Type().FrindlyName()})";


            return name;
        }

        internal static readonly Dictionary<Type, string> FNameDictionary = new Dictionary<Type, string>
        {
            {typeof(Int32),"int" },
            {typeof(Single),"float" },
            {typeof(String),"string" },
        };

        internal static string FrindlyName(this Type type)
        {
            string name=type.Name;

            if (type.IsGenericType)
            {
                var genericTypeArguments = type.GetGenericArguments().ToList();

                var indexOfTilda = name.IndexOf("`", StringComparison.Ordinal);

                if (indexOfTilda == -1)
                    return name;

                name = name.Substring(0, indexOfTilda) + "<";

                for (var i = 0; i < genericTypeArguments.Count; i++)
                {
                    name += (i==0 ? "" : ", ")+ genericTypeArguments[i].FrindlyName();
                }

                name += ">";
                return name;
            }
            return FNameDictionary.TryGetValue(type, out name) ? name : type.Name;
        }

        internal static List<Type> GetComponentList(GameObject gameObject)
        {
            return new List<Type> { typeof(GameObject) }
                .Concat(gameObject
                .GetComponents<Component>()
                .Select(c => c.GetType())
                .Distinct())
                .ToList();
        }

        internal static void GetComponent(BindValue bindValue)
        {
            var types = BindEditorUtility.GetComponentList(bindValue.Source);

            int selectedIndex = 0;

            if (bindValue.ComponentType != null)
                selectedIndex = types.IndexOf(bindValue.ComponentType.Value);

            var index = EditorGUILayout.Popup(
                selectedIndex == -1 ? 0 : selectedIndex,
                types.Select(t => t.FrindlyName()).ToArray());

            if (bindValue.ComponentType == null)
                bindValue.ComponentType = new SType(types[index]);

            if (bindValue.ComponentType.Value != types[index])
                bindValue.ComponentType = new SType(types[index]);

        }

        internal static bool IsValid(MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return true;
                case MemberTypes.Method:
                    var methodInfo = ((MethodInfo) memberInfo);

                    if (methodInfo.IsGenericMethod)
                        return false;

                    if (methodInfo.ReturnType == typeof(void))
                        return false;

                    if (memberInfo.Name.StartsWith("set_"))
                        return false;

                    if (memberInfo.Name.StartsWith("get_"))
                        if (methodInfo.GetParameters().Length == 0)
                            return false;

                    foreach (var parameterInfo in methodInfo.GetParameters())
                    {
                        if (!ConstValueGUI.IsSupported(parameterInfo.ParameterType))
                            return false;
                    }

                    return true;
                case MemberTypes.Property:
                    return true;
                default:
                    return false;
            }
        }

        internal static List<MemberInfo> NewMembersList(BindValue bindValue)
        {
            return bindValue
                .FinalType
                .GetMembers(BindingFlags.Instance | BindingFlags.Public)
                .Where(BindEditorUtility.IsValid)
                .OrderBy(m=>m.FrindlyName())
                .ToList();
        }

        internal static string GetDescription(this BindValue bindValue)
        {
            if (!bindValue.MemberInfo.Any())
                return "Not Set";

            return bindValue.MemberInfo
                .Aggregate(
                    $"{bindValue.Source.name}.{bindValue.ComponentType.Value.FrindlyName()}",
                    (a, m) => $"{a}.{m.ValueName()}");
        }

        internal static void DescriptionGUI(this BindValue bindValue)
        {
            EditorGUILayout.LabelField(bindValue.GetDescription());
        }

        public static void AssinmentGUI(this Assinment bindUnit)
        {
            GUILayout.BeginHorizontal();
            SetValueGUI(bindUnit.SetValue);

            if (bindUnit.SetValue.IsValid)
            {
                if(bindUnit.GetValue==null)
                    bindUnit.GetValue=new GetValue(bindUnit.SetValue.FinalType);

                if (!ConstValueGUI.IsSupported(bindUnit.SetValue.FinalType))
                    bindUnit.GetValue.ValueType = GetValue.ValueTypes.Bind;

                bindUnit.GetValue.ChangeType(bindUnit.SetValue.FinalType);
                GUILayout.Label(EditorGUIUtility.IconContent("Profiler.LastFrame"));
                GetValueGUI(bindUnit.GetValue);
            }
            GUILayout.EndHorizontal();

        }

        public static void SetValueGUI(this BindSetValue setValue)
        {
            EditorGUILayout.BeginHorizontal((GUIStyle)"box");
            setValue.DescriptionGUI();
            if (GUILayout.Button("B", EditorStyles.miniButtonRight, GUILayout.Width(20)))
                BindWindow.Show(setValue);
            EditorGUILayout.EndHorizontal();

        }

        public static void GetValueGUI(this GetValue value)
        {
            if(value.Lable!="")
            GUILayout.Label(value.Lable);
            EditorGUILayout.BeginHorizontal((GUIStyle)"box");
            if (value.ValueType == GetValue.ValueTypes.Const)
            {
                value.ConstValue.OnGUI("Value");

                if (GUILayout.Button("B", EditorStyles.miniButtonRight, GUILayout.Width(20)))
                {
                    value.ValueType = GetValue.ValueTypes.Bind;
                    BindWindow.Show(value.BindValue);
                }
            }

            if (value.ValueType == GetValue.ValueTypes.Bind)
            {
                value.BindValue.DescriptionGUI();

                if (GUILayout.Button("B", EditorStyles.miniButtonRight, GUILayout.Width(20)))
                    BindWindow.Show(value.BindValue);

                if (GUILayout.Button("X", EditorStyles.miniButtonRight, GUILayout.Width(20)))
                {
                    if (ConstValueGUI.IsSupported(value.Type))
                        value.ValueType = GetValue.ValueTypes.Const;
                }

            }

            EditorGUILayout.EndHorizontal();
            return;


        }

    }
}
