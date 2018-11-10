using System;
using System.Collections.Generic;
using System.Linq;
using Bind.Internal;
using FollowMachineDll.Assets;
using UnityEngine;

namespace Bind
{
    [Serializable]
    public class GetValue
    {
        #region ValueType
        public enum ValueTypes
        {
            Const, Bind
        }

        [SerializeField]
        private ValueTypes _valueType = ValueTypes.Const;
        public ValueTypes ValueType
        {
            get { return _valueType; }
            set
            {
                if (_valueType == value)
                    return;
                _valueType = value;
                switch (value)
                {
                    case ValueTypes.Const:
                        if (ConstValue == null)
                            ConstValue = new ConstValue(BindValue.ReturnType);
                        break;
                    case ValueTypes.Bind:
                        if (BindValue == null)
                            BindValue = new BindGetValue(ConstValue.Type);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }

        #endregion

        #region BindValue
        [SerializeField]
        private List<BindGetValue> _bindValues=new List<BindGetValue>();
        public BindGetValue BindValue
        {
            get { return _bindValues.Any() ? _bindValues[0] : null; }
            set
            {
                if (_bindValues == null)
                    _bindValues = new List<BindGetValue>();

                if (_bindValues.Any())
                {
                    if (value == null)
                        _bindValues.Clear();
                    else
                        _bindValues[0] = value;
                }
                else
                {
                    if (value != null)
                        _bindValues.Add(value);
                }
            }
        }
        #endregion

        public string Lable="";

        #region ConstValue

        [SerializeField]
        private List<ConstValue> _constValues=new List<ConstValue>();

        public ConstValue ConstValue
        {
            get { return _constValues.Any() ? _constValues[0] : null; }
            set
            {
                if (_constValues == null)
                    _constValues = new List<ConstValue>();

                if (_constValues.Any())
                {
                    if (value == null)
                        _constValues.Clear();
                    else
                        _constValues[0] = value;
                }
                else
                {
                    if (value != null)
                        _constValues.Add(value);
                }
            }
        }
        #endregion

        public GetValue(Type type)
        {
            switch (ValueType)
            {
                case ValueTypes.Const:
                    ConstValue = new ConstValue(type);
                    break;
                case ValueTypes.Bind:
                    BindValue = new BindGetValue(type);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public GetValue(string lable, string assemblyQualifiedName) : this(Type.GetType(assemblyQualifiedName))
        {
            Lable = lable;
        }

        public GetValue(bool isBind, GameObject gameObject, string vString, Type type)
        {
            if (isBind)
            {
                _valueType = ValueTypes.Bind;
                BindValue=new BindGetValue(gameObject,vString,type);
            }
            else
            {
                _valueType = ValueTypes.Const;
                ConstValue= new ConstValue(vString, type);
            }
        }

        public object Value
        {
            get
            {
                switch (ValueType)
                {
                    case ValueTypes.Const:
                        return ConstValue.Value;
                    case ValueTypes.Bind:
                        return BindValue.Value;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Type Type
        {
            get
            {
                switch (ValueType)
                {
                    case ValueTypes.Const:
                        return ConstValue.Type;
                    case ValueTypes.Bind:
                        return BindValue.ReturnType;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool IsValid
        {
            get
            {
                switch (ValueType)
                {
                    case ValueTypes.Const:
                        return ConstValue!=null;
                    case ValueTypes.Bind:
                        return BindValue?.IsValid ?? false;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void ChangeType(Type type)
        {
            if (ConstValue == null)
                ConstValue = new ConstValue(type);

            if (BindValue == null)
                BindValue = new BindGetValue(type);

            if (ConstValue.Type.Value != type)
                ConstValue = new ConstValue(type);

            if (BindValue.ReturnType.Value != type)
                BindValue = new BindGetValue(type);


        }
    }
}