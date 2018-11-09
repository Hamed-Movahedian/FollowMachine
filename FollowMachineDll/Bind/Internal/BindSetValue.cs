using System;
using System.Linq;
using UnityEngine;

namespace Bind.Internal
{
    [Serializable]
    public class BindSetValue : BindValue
    {
        public BindSetValue()
        {
        }

        public BindSetValue(GameObject pGameObject, string pString) : base(pGameObject, pString)
        {
        }

        public override bool IsValid => MemberInfo.Any() && !MemberInfo.Last().IsMethod;

        public object Value
        {
            set
            {
                var bindValue = BaseObject;

            
                for (var i = 0; i < MemberInfo.Count-1; i++)
                {
                    bindValue = MemberInfo[i].GetValue(bindValue);
                }

                MemberInfo.Last().SetValue(bindValue,value);
            }
        }

        public override BindValue Clone()
        {
            var bindSetValue = new BindSetValue();
            Copy(bindSetValue);
            return bindSetValue;

        }
    }
}