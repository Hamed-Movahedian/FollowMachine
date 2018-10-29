using System;
using UnityEngine;

namespace FollowMachineDll.Utility.Bounder
{
    [Serializable]
    public class BoundData
    {
        public string Lable;
        public BoundMethodEnum BoundMethod;
        public string Value;
        public GameObject BoundGameObject;
        private Bounder _bounder;
        public string TypeName;

        public BoundData(string lable, BoundMethodEnum boundMethod, string value, string typeName)
        {
            Lable = lable;
            BoundMethod = boundMethod;
            Value = value;
            TypeName = typeName;
        }

        public object GetValue()
        {
            if (BoundMethod == BoundMethodEnum.Constant)
                return SupportedTypes.Convert(Value, TypeName);

            if (_bounder == null)
                _bounder = new Bounder(BoundGameObject, Value);

            return _bounder.GetValue();
        }
    }

    public enum BoundMethodEnum
    {
        Constant, GameObject, Variable
    }
}
