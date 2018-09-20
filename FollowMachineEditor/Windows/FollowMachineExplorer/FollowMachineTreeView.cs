using System;
using System.Collections.Generic;
using System.Linq;
using FMachine;
using FMachine.Editor;
using FollowMachineDll;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FollowMachineEditor.Windows.FollowMachineExplorer
{
    public class FollowMachineTreeView : TreeView
    {
        public FollowMachineTreeView(TreeViewState state) : base(state)
        {
            Reload();
            showBorder = true;
        }

        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            FollowMachine machine = (FollowMachine)EditorUtility.InstanceIDToObject(id);
            if (machine)
                Selection.activeGameObject = machine.gameObject;
        }

        protected override void DoubleClickedItem(int id)
        {
            FollowMachine machine = (FollowMachine)EditorUtility.InstanceIDToObject(id);

            if (machine)
                Selection.activeGameObject = machine.gameObject;

            var fmWindow = EditorWindow.GetWindow<FMWindow>();
            if(fmWindow!=null)
                fmWindow.Canvas.CordinationSystem.Focus(false, true);
        }

        #region Rename

        protected override bool CanRename(TreeViewItem item)
        {
            return true;
        }

        protected override void RenameEnded(RenameEndedArgs args)
        {
            var machine = (FollowMachine)EditorUtility.InstanceIDToObject(args.itemID);
            if (machine)
            {
                machine.name = args.newName;
                FindItem(args.itemID, rootItem).displayName = args.newName;
                args.acceptedRename = true;
            }
        }

        #endregion

        #region Drag and drop

        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            return true;
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {

            DragAndDrop.PrepareStartDrag();

            var sortedDraggedIDs = SortItemIDsInRowOrder(args.draggedItemIDs);

            List<Object> objList = new List<Object>(sortedDraggedIDs.Count);
            foreach (var id in sortedDraggedIDs)
            {
                Object obj = EditorUtility.InstanceIDToObject(id);
                if (obj != null)
                    objList.Add(obj);
            }

            DragAndDrop.objectReferences = objList.ToArray();

            string title = objList.Count > 1 ? "<Multiple>" : objList[0].name;
            DragAndDrop.StartDrag(title);


        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            // First check if the dragged objects are GameObjects
            var draggedObjects = DragAndDrop.objectReferences;
            var followMachines = new List<FollowMachine>(draggedObjects.Length);
            foreach (var obj in draggedObjects)
            {
                var followMachine = obj as FollowMachine;
                if (followMachine == null)
                {
                    return DragAndDropVisualMode.None;
                }

                followMachines.Add(followMachine);
            }

            if (args.performDrop)
            {
                switch (args.dragAndDropPosition)
                {
                    case DragAndDropPosition.UponItem:
                    case DragAndDropPosition.BetweenItems:
                        var parent = 
                            args.parentItem != null && args.parentItem.id!=0 ? 
                                    GetFollowMachine(args.parentItem.id).transform : FindCommonRoot();


                        foreach (var machine in followMachines)
                            machine.transform.SetParent(parent);

                        if (args.dragAndDropPosition == DragAndDropPosition.BetweenItems)
                        {
                            int insertIndex = args.insertAtIndex;
                            for (int i = followMachines.Count - 1; i >= 0; i--)
                            {
                                var machine = followMachines[i];
                                insertIndex = GetAdjustedInsertIndex(parent, machine, insertIndex);
                                machine.transform.SetSiblingIndex(insertIndex);
                            }
                        }
                        break;

                    case DragAndDropPosition.OutsideItems:
                        foreach (var machine in followMachines)
                        {
                            machine.transform.SetParent(FindCommonRoot()); // make root when dragged to empty space in treeview
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                Reload();
                SetSelection(followMachines.Select(t => t.GetInstanceID()).ToList(), TreeViewSelectionOptions.RevealAndFrame);
            }

            return DragAndDropVisualMode.Move;
        }

        private Transform FindCommonRoot()
        {
            var followMachines = Object.FindObjectsOfType<FollowMachine>();

            if (followMachines.Length == 0)
                return null;

            var commonRoot = followMachines[0].transform.parent;

            while (commonRoot!=null)
            {
                var find = false;

                foreach (var followMachine in followMachines)
                    if (!HasParent(followMachine.transform, commonRoot))
                    {
                        find = true;
                        break;
                    }

                if (find)
                    commonRoot = commonRoot.parent;
                else
                    return commonRoot;
            }

            return null;
        }

        private bool HasParent(Transform transform, Transform parent)
        {
            while (transform.parent!=null)
            {
                if (transform.parent == parent)
                    return true;
                transform = transform.parent;
            }

            return false;
        }

        private int GetAdjustedInsertIndex(Transform parent, FollowMachine machine, int insertIndex)
        {
            if (machine.transform.parent == parent && machine.transform.GetSiblingIndex() < insertIndex)
                return --insertIndex;
            return insertIndex;
        }

        protected override bool CanBeParent(TreeViewItem item)
        {
            return true;
        }
        

        #endregion

        private FollowMachine GetFollowMachine(int id)
        {
            return (FollowMachine)EditorUtility.InstanceIDToObject(id);
        }

        #region BuildRoot

        protected override TreeViewItem BuildRoot()
        {
            
            var followMachines = Object.FindObjectsOfType<FollowMachine>()
                .OrderBy(fm=>fm.transform.GetSiblingIndex())
                .ToArray();

            var root = new TreeViewItem(0, -1, "Root");

            if (followMachines.Length == 0)
            {
                root.AddChild(new TreeViewItem{id=1,displayName="Follow machine not found!"});
            }
            else
            {
                var items = new List<TreeViewItem>();

                foreach (var machine in followMachines)
                {
                    items.Add(new TreeViewItem { id = machine.GetInstanceID(), displayName = machine.name, });
                }

                for (int i = 0; i < followMachines.Length; i++)
                {
                    FollowMachine parentFollowMachine = null;
                    if (followMachines[i].transform.parent)
                        parentFollowMachine = followMachines[i].transform.parent.GetComponentInParent<FollowMachine>();

                    if (parentFollowMachine == null)
                        root.AddChild(items[i]);
                    else
                    {
                        items.Find(item => item.id == parentFollowMachine.GetInstanceID()).AddChild(items[i]);
                    }
                }
            }

            SetupDepthsFromParentsAndChildren(root);
            return root;
        }
        

        #endregion
    }
}
