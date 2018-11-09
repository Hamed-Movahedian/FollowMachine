using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Bind.Internal
{
    [Serializable]
    public class SMemberInfo
    {
        [SerializeField]
        private SType _declaringType;

        [SerializeField]
        private string _name;

        [SerializeField]
        private MemberTypes _memberType;

        public List<ConstValue> Parameters;

        private MemberInfo _info;

        public SMemberInfo(MemberInfo memberInfo)
        {
            Value = memberInfo;
        }

        public SMemberInfo(SMemberInfo sMemberInfo)
        {
            _declaringType=new SType(sMemberInfo._declaringType);
            _name = sMemberInfo._name;
            _memberType = sMemberInfo._memberType;
            Parameters = new List<ConstValue>();
            sMemberInfo.Parameters.ForEach(p=>Parameters.Add(new ConstValue(p)));
            _info = sMemberInfo._info;
        }

        public MemberInfo Value
        {
            get
            {
                if (_info != null)
                    return _info;

                switch (_memberType)
                {
                    case MemberTypes.Field:
                        _info = _declaringType.Value.GetField(_name);
                        break;
                    case MemberTypes.Property:
                        _info = _declaringType.Value.GetProperty(_name);
                        break;
                    case MemberTypes.Method:
                        _info = _declaringType.Value.GetMethod(_name, Parameters.Select(p => p.Type.Value).ToArray());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return _info;
            }
            set
            {
                _info = value;

                _declaringType = new SType(_info.DeclaringType);

                _name = _info.Name;

                _memberType = _info.MemberType;

                if (_memberType == MemberTypes.Method)
                {
                    Parameters = MethodInfo.GetParameters().Select(p => new ConstValue(p.ParameterType)).ToList();
                }
                else
                {
                    Parameters = new List<ConstValue>();
                }
            }
        }


        public Type Type
        {
            get
            {
                switch (_memberType)
                {
                    case MemberTypes.Field:
                        return FieldInfo.FieldType;
                    case MemberTypes.Property:
                        return PropertyInfo.PropertyType;
                    case MemberTypes.Method:
                        return MethodInfo.ReturnType;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public MethodInfo MethodInfo => (MethodInfo)Value;
        public FieldInfo FieldInfo => (FieldInfo)Value;
        public PropertyInfo PropertyInfo => (PropertyInfo)Value;
        public bool IsMethod => _memberType == MemberTypes.Method;

        public object GetValue(object obj)
        {
            switch (_memberType)
            {
                case MemberTypes.Field:
                    return FieldInfo.GetValue(obj);
                case MemberTypes.Property:
                    return PropertyInfo.GetValue(obj,null);
                case MemberTypes.Method:
                    return MethodInfo.Invoke(obj, Parameters.Select(p => p.Value).ToArray());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public void SetValue(object bindValue, object value)
        {
            switch (_memberType)
            {
                case MemberTypes.Field:
                    FieldInfo.SetValue(bindValue, value);
                    break;

                case MemberTypes.Property:
                    PropertyInfo.SetValue(bindValue, value,null);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public string ValueName()
        {
            var name = Value.Name;

            if (Value.MemberType == MemberTypes.Method)
                name += Parameters
                            .Aggregate("(", (c, p) =>
                                $"{c}{(c == "(" ? "" : ", ")}{p.Value}")
                        + ")";

            return name;
        }
    }
}