using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FollowMachineDll.Utility.Bounder
{
    public class BoundMember
    {
        private MemberInfo _memberInfo;
        private List<string> _parameterTexts;
        private ParameterInfo[] _parameterInfos = new ParameterInfo[0];

        public BoundMember(MemberInfo memberInfo)
        {
            _memberInfo = memberInfo;

            if (_memberInfo.MemberType == MemberTypes.Method)
            {
                _parameterInfos = (_memberInfo as MethodInfo).GetParameters();

                _parameterTexts = new List<string>();
                foreach (var parameterInfo in _parameterInfos)
                {
                    _parameterTexts.Add(SupportedTypes.GetDefault(parameterInfo.ParameterType).ToString());
                }
            }
        }

        public BoundMember(Type type, string name)
        {
            if (name.Contains('('))
            {
                string[] parts = name
                    .Replace("\"", "")
                    .Split(new char[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);

                MethodInfo methodInfo = (MethodInfo)type
                    .GetMember(parts[0], BindingFlags.Public | BindingFlags.Instance)
                    .Where(mi => mi.MemberType == MemberTypes.Method)
                    .Select(mi => (MethodInfo)mi)
                    .FirstOrDefault(mi => mi.GetParameters().Length == parts.Length - 1);

                if (methodInfo == null)
                    throw new Exception($"Method info by name {name} not found!!");

                _memberInfo = methodInfo;

                _parameterInfos = methodInfo.GetParameters();

                _parameterTexts = new List<string>();

                for (int i = 0; i < _parameterInfos.Length; i++)
                {
                    var parameterTypeName = _parameterInfos[i].ParameterType.Name;

                    if (!SupportedTypes.IsSupported(parameterTypeName))
                        throw new Exception($"Invalid parameter {parameterTypeName} in method {methodInfo.Name}");

                    _parameterTexts.Add(parts[i + 1]);
                }
            }
            else
            {
                _memberInfo = type
                    .GetMember(name, BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(mi =>
                        mi.MemberType == MemberTypes.Field |
                        mi.MemberType == MemberTypes.Property);

                _parameterInfos = new ParameterInfo[0];

                _parameterTexts = new List<string>();

                if (_memberInfo == null)
                    throw new Exception($"Member info by name {name} not found!!");
            }
        }

        public Type ReturnType
        {
            get
            {
                switch (_memberInfo.MemberType)
                {
                    case MemberTypes.Field:
                        return AsFieldInfo.FieldType;
                    case MemberTypes.Method:
                        return AsMethodInfo.ReturnType;
                    case MemberTypes.Property:
                        return AsPropertyInfo.PropertyType;
                }

                return null;
            }
        }

        public string Name
        {
            get
            {
                string text = _memberInfo.Name;

                if (_memberInfo.MemberType == MemberTypes.Method)
                {
                    text += "(";
                    for (var i = 0; i < _parameterTexts.Count; i++)
                    {
                        if (i > 0)
                            text += " ,";
                        if (_parameterInfos[i].ParameterType == typeof(string))
                            text += "\"" + _parameterTexts[i] + "\"";
                        else
                            text += _parameterTexts[i].ToString();
                    }

                    text += ")";
                }

                return text;
            }
        }

        public void OnGUI()
        {
            if (_parameterInfos.Length == 0)
                return;


#if UNITY_EDITOR
        GUILayout.Label("Parameters :", UnityEditor.EditorStyles.boldLabel);

#endif
            for (var i = 0; i < _parameterInfos.Length; i++)
            {
                var parameterInfo = _parameterInfos[i];

                if (_parameterTexts[i] == null)
                    _parameterTexts[i] = SupportedTypes.GetDefault(parameterInfo.ParameterType).ToString();

                _parameterTexts[i] = SupportedTypes.GUI(parameterInfo.Name, _parameterTexts[i], parameterInfo.ParameterType);
            }

            BounderUtilitys.BoldSeparator();

        }

        public object GetValue(object inObject)
        {
            if(_memberInfo==null)
                throw new Exception($"Invalid bound member");

            switch (_memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return AsFieldInfo.GetValue(inObject);
                case MemberTypes.Property:
                    return AsPropertyInfo.GetValue(inObject,null);
                default:
                    return AsMethodInfo.Invoke(inObject, ParameterObjects);
            }
        }

        public object[] ParameterObjects
        {
            get
            {
                var objects = new object[_parameterInfos.Length];

                for (int i = 0; i < objects.Length; i++)
                    objects[i] = SupportedTypes.Convert(_parameterTexts[i], _parameterInfos[i].ParameterType);

                return objects;
            }
        }

        public FieldInfo AsFieldInfo => _memberInfo as FieldInfo;
        public PropertyInfo AsPropertyInfo => _memberInfo as PropertyInfo;
        public MethodInfo AsMethodInfo => _memberInfo as MethodInfo;
    }
}