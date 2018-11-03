using System;
using UnityEngine;

namespace FollowMachineDll.Utility.Bounder
{
    [Serializable]
    public class BoundData
    {
        public string Lable;
        public BoundSourceEnum BoundSource;
        public string Value;
        public GameObject BoundGameObject;
        public string TypeName;
        public BoundTypeEnum BoundType = BoundTypeEnum.Get;

        private Bounder _bounder;

        public BoundData(string lable, BoundSourceEnum boundSource, string value, string typeName)
        {
            Lable = lable;
            BoundSource = boundSource;
            Value = value;
            TypeName = typeName;
        }

        public BoundData(
            string lable, 
            BoundSourceEnum boundSource, 
            BoundTypeEnum boundType,
            string value, 
            string typeName, 
            GameObject gameObject) : this(lable,boundSource,typeName,value)
        {
            BoundGameObject = gameObject;
            BoundType = boundType;
        }


        public object GetValue()
        {
            if (BoundSource == BoundSourceEnum.Constant)
                return SupportedTypes.Convert(Value, TypeName);

            if (_bounder == null)
                _bounder = new Bounder(BoundGameObject, Value);

            return _bounder.GetValue();
        }
    }

    public enum BoundSourceEnum
    {
        Constant, GameObject, Variable
    }
    public enum BoundTypeEnum
    {
        Get, Set
    }
}
