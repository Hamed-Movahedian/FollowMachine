using System;
using Bind.Internal;
using UnityEngine;

namespace Bind
{
    [Serializable]
    public class Assinment
    {
        public BindSetValue SetValue=new BindSetValue();
        public GetValue GetValue;

        public Assinment()
        {
        }

        public Assinment(GameObject pGameObject, string pString, bool isBind, GameObject vGameObject, string vString)
        {
            SetValue=new BindSetValue(pGameObject,pString);
            GetValue=new GetValue(isBind,vGameObject,vString,SetValue.FinalType);
        }

        public void Assign()
        {
            if(!SetValue.IsValid)
                throw new Exception("Invalid SetValue!");

            if(!GetValue.IsValid)
                throw new Exception("Invalid GetValue!");

            SetValue.Value = GetValue.Value;

        }
    }
}