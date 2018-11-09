using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bind.Internal
{
    [Serializable]
    public abstract class BindValue
    {
        protected BindValue(GameObject pGameObject, string pString)
        {
            Source = pGameObject;
            var strings = pString.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            ComponentType = new SType(pGameObject
                .GetComponents(typeof(Component))
                .Select(c => c.GetType())
                .Single(t => t.Name == strings[0]));

            MemberInfo.Add(
                new SMemberInfo(
                    ComponentType.Value.GetMember(strings[1]).Single()
                ));

        }

        #region Source
        [SerializeField]
        private GameObject _source;
        public GameObject Source
        {
            get { return _source; }
            set
            {
                if (_source == value)
                    return;
                _source = value;

                ComponentType = null;
                _baseObject = null;
                MemberInfo.Clear();
            }
        }
        #endregion

        #region ComponentType
        [SerializeField]
        private SType _componentType;

        public SType ComponentType
        {
            get { return _componentType; }
            set
            {
                if (value == _componentType)
                    return;

                _componentType = value;
                _baseObject = null;
                MemberInfo.Clear();
            }
        } 
        #endregion

        public List<SMemberInfo> MemberInfo = new List<SMemberInfo>();

        public abstract bool IsValid { get; }

        #region BaseObject
        private object _baseObject;

        protected BindValue()
        {
            
        }

        protected object BaseObject
        {
            get
            {
                if (_baseObject != null)
                    return _baseObject;

                if (Source == null)
                    throw new Exception("Bind Source invalid!!");

                if (ComponentType == null)
                    throw new Exception("Bind Component invalid!!");

                _baseObject = Source;

                if (ComponentType.Value != typeof(GameObject))
                    _baseObject = Source.GetComponent(ComponentType);

                return _baseObject;
            }
        }


        public Type FinalType
        {
            get
            {
                if (MemberInfo.Any())
                    return MemberInfo.Last().Type;

                return ComponentType;
            }
        }

        #endregion

        public abstract BindValue Clone();

        public void Copy(BindValue bindValue)
        {
            bindValue.Source = Source;
            bindValue.ComponentType = ComponentType;
            bindValue.MemberInfo = new List<SMemberInfo>();
            MemberInfo.ForEach(mi=>bindValue.MemberInfo.Add(new SMemberInfo(mi)));
        }

    }
}
