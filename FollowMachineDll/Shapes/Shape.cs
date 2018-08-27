using System;
using System.Collections.Generic;
using FMachine.SettingScripts;
using FollowMachineDll.Utility;
using UnityEngine;

namespace FMachine.Shapes
{
    public abstract class Shape : MonoBehaviour, IMouseInteractable
    {
        public Graph Graph;

        public bool IsSelected = false;

        public bool IsHover = false;

        protected bool IsHidden;

        public virtual void OnCreate(Graph graph)
        {
            Graph = graph;
        }
        public void Select()
        {
            if (IsSelected)
                return;

            EditorTools.Instance.Undo_RecordObject(this, "Selection Change");

            IsSelected = true;

            //Graph.BringToFront(this);

        }

        public void Deselect()
        {
            if (!IsSelected)
                return;

            EditorTools.Instance.Undo_RecordObject(this, "Selection Change");

            IsSelected = false;
        }

        public virtual IMouseInteractable IsMouseOver(Vector2 mousePosition)
        {
            return null;
        }

        public virtual void MouseDown(Vector2 mousePosition, Event currentEvent) { }

        public virtual void MouseDrag(Vector2 delta, Vector2 mousePosition, Event currentEvent) { }

        public virtual void MouseUp(Vector2 mousePosition, Event currentEvent) { }

        public virtual void MouseEnter(Vector2 mousePosition, Event currentEvent)
        {
            IsHover = true;
        }


        public virtual void MouseExit(Vector2 mousePosition, Event currentEvent) 
        {
            IsHover = false;
        }
        public virtual void DoubleClick(Vector2 mousePosition, Event currentEvent)
        {
            
        }

        public virtual void MouseMove(Vector2 mousePosition, Event currentEvent)
        {
            
        }

        public virtual void Draw() { }

        public virtual bool IsInBox(Rect rect)
        {
            return false;
        }

        public void Hide()
        {
            IsHidden = true;
        }

        public void Show()
        {
            IsHidden = false;
        }

        public abstract void Delete();
    }
}