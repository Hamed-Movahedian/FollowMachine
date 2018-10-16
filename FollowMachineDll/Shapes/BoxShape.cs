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
            GUILayout.BeginVertical((GUIStyle)"box");
            {
                GUILayout.Label("Info :", (GUIStyle) "BoldLabel");

                var s = GUILayout.TextField(Info);
                if (s != Info)
                {
                    EditorTools.Instance.Undo_RecordObject(this,"Change Info");
                    Info = s;
                    name = Info + " (" + GetType().Name + ")";
                }
            }
            GUILayout.EndVertical();
            GUILayout.Space(5);
        }
    }
}