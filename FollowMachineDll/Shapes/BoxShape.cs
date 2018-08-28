using System.Collections.Generic;
using FMachine.SettingScripts;
using FollowMachineDll.Utility;
using UnityEngine;

namespace FMachine.Shapes
{
    public abstract class BoxShape : Shape
    {
        public Rect Rect;

        [Header("Descriptions :")]
        public string Name = "";

        public virtual void OnCreate(Graph graph, Vector2 position)
        {
            base.OnCreate(graph);
            Rect.position = position;
        }



        public override IMouseInteractable IsMouseOver(Vector2 mousePosition)
        {
            if (Rect.Contains(mousePosition))
                return this;

            return null;
        }

        
        protected virtual string FilterName(string name)
        {
            return name;
        }

        public override bool IsInBox(Rect rect)
        {
            return rect.Contains(Rect.min) && rect.Contains(Rect.max);
        }
    }
}