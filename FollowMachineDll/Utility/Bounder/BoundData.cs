using System;
using UnityEngine;

namespace FollowMachineDll.Utility.Bounder
{
    [Serializable]
    public class BoundData
    {
        public string Name;
        public bool IsBound;
        public Type Type;
        public string Value;
        public GameObject BoundGameObject;
        private Bounder _bounder;
        public string TypeName;

        public object GetValue() 
        {
            if (!IsBound)
                return SupportedTypes.Types[Type.Name].Convertor(Value);

            if (_bounder == null)
                _bounder = new Bounder(BoundGameObject, Value);

            return _bounder.GetValue(Type);
        }
    }
}
