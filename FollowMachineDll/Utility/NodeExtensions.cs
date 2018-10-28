using System.Collections.Generic;
using FMachine.Shapes.Nodes;
using UnityEngine;

namespace FollowMachineDll.Utility
{
    public static class NodeExtensions
    {
        public static Rect BoundigRect(this List<Node> list)
        {
            Rect rect = Rect.zero;

            foreach (var node in list)
            {
                if (rect == Rect.zero)
                    rect = node.Rect;
                else
                    rect=rect.Expand(node.Rect);
            }

            return rect;
        }
    }
}
