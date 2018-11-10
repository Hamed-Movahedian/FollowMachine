using System;
using System.Collections.Generic;
using FollowMachineDll.DataTypes;
using Newtonsoft.Json;
using UnityEngine;
using Object = System.Object;

namespace Bind.Internal
{
    [Serializable]
    public class ConstValue
    {
        #region _constTypeUtils
        class ConstTypeUtil
        {
            public Func<object, string> Serialize;
            public Func<string, object> Deserialize;
        }

        private static Dictionary<Type, ConstTypeUtil> _constTypeUtils = new Dictionary<Type, ConstTypeUtil>
        {
            {typeof(Color),new ConstTypeUtil()
            {
                Serialize = (obj)=> JsonConvert.SerializeObject((Vector4)((Color) obj)),
                Deserialize = str => (Color) ((Vector4)JsonConvert.DeserializeObject(str, typeof(Vector4)))
            } },
            {typeof(string),new ConstTypeUtil()
            {
                Serialize = (obj)=>
                {
                    if(obj is string)
                        obj=new LText(true,(string) obj);

                    return JsonConvert.SerializeObject((LText) obj);
                },
                Deserialize = str => (LText)JsonConvert.DeserializeObject(str, typeof(LText))
            } }
        };
        #endregion

        public SType Type;

        [SerializeField]
        private string _valueString = "";

        [SerializeField] private UnityEngine.Object _unityObject;

        private object _value;

        public ConstValue(Type type)
        {
            Type = new SType(type);

            if (type == typeof(string))
                Value = new LText(false, "");
            else if (type.IsValueType)
                Value = Activator.CreateInstance(type);
            else
            {
                _value = null;
                _valueString = "null";
                _unityObject = null;
            }
        }

        public ConstValue(ConstValue constValue)
        {
            Type = new SType(constValue.Type);
            _valueString = constValue._valueString;
            _unityObject = constValue._unityObject;
            _value = constValue._value;
        }

        public ConstValue(string constValue, Type type)
        {
            Type = new SType(type);

            if (type == typeof(string))
            {
                constValue = JsonConvert.SerializeObject(new LText(false, constValue));
            }

            if (type == typeof(Boolean))
                constValue = JsonConvert.SerializeObject(Boolean.Parse(constValue));


            _valueString = constValue;
        }

        public Object Value
        {
            get => Type.Value == typeof(string) ? RawValue.ToString() : RawValue;
            set
            {
                if (Type.Value == typeof(string) && value is string)
                    ((LText)RawValue).Text = (string)value;
                else
                {
                    RawValue = value;
                }
            }
        }
        public Object RawValue
        {
            get
            {
                if (_value == null)
                {
                    if (Type.Value.IsSubclassOf(typeof(UnityEngine.Object)))
                        _value = _unityObject;
                    else if (_constTypeUtils.ContainsKey(Type))
                        _value = _constTypeUtils[Type].Deserialize(_valueString);
                    else
                        _value = JsonConvert.DeserializeObject(_valueString, Type.Value);
                }

                return _value;
            }
            set
            {
                if (_value == value)
                    return;

                if (value == null)
                {
                    _value = null;
                    _valueString = "null";
                    _unityObject = null;
                    return;
                }

                _value = value;

                _unityObject = value as UnityEngine.Object;

                if (_unityObject == null)
                {
                    if (_constTypeUtils.ContainsKey(Type))
                        _valueString = _constTypeUtils[Type].Serialize(_value);
                    else
                        _valueString = JsonConvert.SerializeObject(_value);
                }
            }
        }

    }
}