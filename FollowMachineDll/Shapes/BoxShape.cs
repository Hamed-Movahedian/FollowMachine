using System.Collections.Generic;
using FollowMachineDll.Utility;
using UnityEngine;

namespace FMachine.Shapes
{
    public abstract class BoxShape : Shape
    {
        public Rect Rect;

        public string Info = "";

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

        public virtual void Move(Vector2 delta)
        {
            Rect.position += delta;
        }

        public virtual void EndMove()
        {
            Rect.position = new Vector2(Mathf.Round(Rect.position.x / 10) * 10, Mathf.Round(Rect.position.y / 10) * 10);

        }

        public virtual void OnInspector()
        {
            if (EditorTools.Instance.PropertyField(this, "Info"))
                name = Info + " (" + GetType().Name + ")";
        }
    }
}