using System;
using UnityEngine;

namespace Bind.Internal
{
    [Serializable]
    public class SType
    {
        private Type _type;

        [SerializeField]
        private string _assemblyQualifiedName;

        public SType(Type type)
        {
            Value = type;
        }

        public SType() { }

        public Type Value
        {
            get { return _type ?? (_type = _assemblyQualifiedName==null ? null : Type.GetType(_assemblyQualifiedName)); }
            set
            {
                _type = value;
                _assemblyQualifiedName = _type.AssemblyQualifiedName;
            }
        }
        public static implicit operator Type(SType sType)  
        {
            return sType.Value;
        }
    }
}