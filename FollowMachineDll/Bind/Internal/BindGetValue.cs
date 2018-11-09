using System;
using System.Linq;
using UnityEngine;

namespace Bind.Internal
{
    [Serializable]
    public class BindGetValue : BindValue
    {
        public SType ReturnType;

        public BindGetValue(Type type)
        {
            ReturnType = new SType(type);
        }

        public BindGetValue(GameObject pGameObject, string pString, Type type) : base(pGameObject, pString)
        {
            ReturnType=new SType(type);
        }

        public override bool IsValid => MemberInfo.Any() && ReturnType.Value.IsAssignableFrom(MemberInfo.Last().Type);
        public override BindValue Clone()
        {
            var bindGetValue = new BindGetValue(ReturnType);
            Copy(bindGetValue);
            return bindGetValue;
        }

        public object Value
        {
            get
            {
                var value = BaseObject;

                foreach (var memberInfo in MemberInfo)
                {
                    value = memberInfo.GetValue(value);
                }

                return value;
            }
        }
    }
}