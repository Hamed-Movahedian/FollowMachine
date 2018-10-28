using FollowMachineDll.Base;
using UnityEngine;

namespace FMachine.Shapes
{
    public abstract class Shape : MonoBehaviour
    {
        public EObject EObject;

        public Graph Graph;

        public bool IsSelected = false;

        public bool IsHover = false;
    }
}