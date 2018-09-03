using System;
using System.Collections.Generic;
using System.Linq;
using FMachine;
using FMachine.Editor;
using FollowMachineEditor.Windows.FollowMachineExplorer;
using MgsCommonLib.UI;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FollowMachineEditor.Windows.WindowExplorer
{
    public class WindowTreeView : TreeView
    {
        public WindowTreeView(TreeViewState state) : base(state)
        {
            Reload();
            showBorder = true;
        }

        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);

            MgsUIWindow window = (MgsUIWindow)EditorUtility.InstanceIDToObject(id);

            if (window)
                Selection.activeGameObject = window.gameObject;
        }

        protected override void DoubleClickedItem(int id)
        {
            MgsUIWindow window = (MgsUIWindow)EditorUtility.InstanceIDToObject(id);

            if (window)
                Selection.activeGameObject = window.gameObject;
            

        }

        #region Rename

        protected override bool CanRename(TreeViewItem item)
        {
            return true;
        }

        protected override void RenameEnded(RenameEndedArgs args)
        {
            var window = (MgsUIWindow)EditorUtility.InstanceIDToObject(args.itemID);
            if (window)
            {
                window.name = args.newName;
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
            var windows = new List<MgsUIWindow>(draggedObjects.Length);
            foreach (var obj in draggedObjects)
            {
                var window = obj as MgsUIWindow;
                if (window == null)
                {
                    return DragAndDropVisualMode.None;
                }

                windows.Add(window);
            }

            if (args.performDrop)
            {
                switch (args.dragAndDropPosition)
                {
                    case DragAndDropPosition.UponItem:
                    case DragAndDropPosition.BetweenItems:
                        MgsUIWindow parent = args.parentItem != null ? GetWindow(args.parentItem.id) : null;


                        foreach (var window in windows)
                            window.transform.SetParent(parent.transform);

                        if (args.dragAndDropPosition == DragAndDropPosition.BetweenItems)
                        {
                            int insertIndex = args.insertAtIndex;
                            for (int i = windows.Count - 1; i >= 0; i--)
                            {
                                var window = windows[i];
                                insertIndex = GetAdjustedInsertIndex(parent, window, insertIndex);
                                window.transform.SetSiblingIndex(insertIndex);
                            }
                        }
                        break;

                    case DragAndDropPosition.OutsideItems:
                        var canvases = Object.FindObjectsOfType<Canvas>().ToList();
                        if(canvases.Count>0)
                        foreach (var window in windows)
                        {
                            window.transform.SetParent(canvases[0].transform); // make root when dragged to empty space in treeview
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                Reload();
                SetSelection(windows.Select(t => t.GetInstanceID()).ToList(), TreeViewSelectionOptions.RevealAndFrame);
            }

            return DragAndDropVisualMode.Move;
        }

        private int GetAdjustedInsertIndex(MgsUIWindow parent, MgsUIWindow machine, int insertIndex)
        {
            if (machine.transform.parent == parent.transform && machine.transform.GetSiblingIndex() < insertIndex)
                return --insertIndex;
            return insertIndex;
        }

        protected override bool CanBeParent(TreeViewItem item)
        {
            return true;
        }
        

        #endregion

        private MgsUIWindow GetWindow(int id)
        {
            return (MgsUIWindow)EditorUtility.InstanceIDToObject(id);
        }

        #region BuildRoot

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem(0, -1, "Root");

            var canvases = Object.FindObjectsOfType<Canvas>().ToList();

            if (canvases.Count==0)
            {
                return root;
            }

            var windows = canvases[0].GetComponentsInChildren<MgsUIWindow>();

            var items = new List<TreeViewItem>();

            foreach (var window in windows)
            {
                items.Add(new TreeViewItem { id = window.GetInstanceID(), displayName = window.name, });
            }

            for (int i = 0; i < windows.Length; i++)
            {
                MgsUIWindow parentWindow = null;
                if (windows[i].transform.parent)
                    parentWindow = windows[i].transform.parent.GetComponentInParent<MgsUIWindow>();

                if (parentWindow == null)
                    root.AddChild(items[i]);
                else
                {
                    items.Find(item => item.id == parentWindow.GetInstanceID()).AddChild(items[i]);
                }
            }

            SetupDepthsFromParentsAndChildren(root);
            return root;
        }
        

        #endregion
    }
}
