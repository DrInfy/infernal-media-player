using System;
using System.Collections.Generic;
using System.Linq;
using Base.Libraries;

namespace Base.ListLogic
{
    /// <summary>
    /// Implements the basic logical operations for list types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListController<T>
    {
        #region Delegates and Events

        public delegate void ListSelectionChangedEvent();
        public delegate void ListSizeChangedEvent(bool enlargement);

        /// <summary>
        /// Occurs when list size changes.
        /// </summary>
        public event ListSizeChangedEvent ListSizeChanged;
        public event ListSelectionChangedEvent ListSelectionChanged;

        #endregion

        protected readonly bool multiSelectable;
        protected readonly bool searchable;

        private string findText = "";

        protected FindString[] findWords;
        protected int[] findlist = new int[0];

        protected List<Selectable<T>> items = new List<Selectable<T>>();


        /// <summary> 
        /// The selected index of an item in items list.
        /// When search is active, use findList[selectedIndex].
        /// </summary>
        protected int selectedIndex = -1;


        public ListController(bool searchable, bool multiSelectable)
        {
            this.searchable = searchable;
            this.multiSelectable = multiSelectable;
        }


        public bool ItemsDragable { get; set; }

        public string FindText
        {
            get { return findText; }
            set
            {
                value = value.Trim();
                if (string.Compare(findText, value, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    findText = value;
                    findWords = StringHandler.GetFindWords(findText);
                    UpdateItems();
                }
            }
        }

        protected bool SearchActive
        {
            get { return searchable && !string.IsNullOrEmpty(findText); }
        }

        public int VisibleCount
        {
            get
            {
                if (SearchActive)
                    return findlist.Length;
                return items.Count;
            }
        }




        /// <summary>
        /// Gets the content for item.
        /// </summary>
        /// <param name="visibleIndex">Index of the visible.</param>
        /// <returns></returns>
        public T GetContent(int visibleIndex)
        {
            if (SearchActive)
            {
                return items[findlist[visibleIndex]].Content;
            }
            return items[visibleIndex].Content;
        }


        /// <summary>
        /// Gets the content for item.
        /// </summary>
        /// <param name="visibleIndex">Index of the visible.</param>
        /// <returns></returns>
        public string GetText(int visibleIndex)
        {
            if (SearchActive)
            {
                return items[findlist[visibleIndex]].PresentText;
            }
            return items[visibleIndex].PresentText;
        }


        /// <summary>
        /// Updates the items in find list.
        /// </summary>
        public void UpdateItems()
        {
            if (SearchActive)
            {
                Array.Resize(ref findlist, items.Count);

                int added = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    bool found = items[i].NeverFilter || StringHandler.FindFound(items[i].PresentText, findWords);

                    if (found)
                    {
                        findlist[added] = i;
                        added++;
                    }
                }
                Array.Resize(ref findlist, added);
            }
            else
            {
                findlist = null;
            }
            SeekSelectedIndex();
            ListSizeChanged(true);
        }





        public T GetSelected()
        {
            if (SearchActive)
            {
                if (selectedIndex >= 0 && selectedIndex < findlist.Length)
                    return items[findlist[selectedIndex]].Content;
            }
            else
            {
                if (selectedIndex >= 0)
                    return items[selectedIndex].Content;
            }

            return default(T);
        }



        public List<T> GetSelectedList()
        {
            var list = new List<T>();
            if (multiSelectable)
            {
                for (int i = 0; i <= items.Count - 1; i++)
                {
                    if (items[i].Selected)
                    {
                        list.Add(items[i].Content);
                    }
                }
            }
            else if (selectedIndex >= 0)
            {
                list.Add(items[selectedIndex].Content);
            }
            return list;
        }


        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="neverFilter">set true if this item needs to unfilterable. </param>
        public virtual void AddItem(T item, bool neverFilter = false)
        {
            items.Add(new Selectable<T>(item) { NeverFilter = neverFilter });
            OnListSizeChanged(true);
        }


        /// <summary>
        /// Adds the items.
        /// </summary>
        /// <param name="list">The list.</param>
        public virtual void AddItems(ICollection<T> list)
        {
            foreach (T selectable in list)
            {
                items.Add(new Selectable<T>(selectable));
            }
            OnListSizeChanged(true);
        }


        /// <summary>
        /// Adds the items.
        /// </summary>
        /// <param name="array">The array.</param>
        public virtual void AddItems(T[] array)
        {
            foreach (T selectable in array)
            {
                items.Add(new Selectable<T>(selectable));
            }
            OnListSizeChanged(true);
        }


        /// <summary>
        /// Clears the list.
        /// </summary>
        public virtual void Clear()
        {
            items.Clear();
            if (searchable)
                Array.Resize(ref findlist, 0);
            selectedIndex = -1;
            OnListSizeChanged(false);
        }


        /// <summary>
        /// Removes the selected items from the list.
        /// </summary>
        public virtual void RemoveSelected()
        {
            if (multiSelectable)
            {
                bool removed = false;
                for (int i = items.Count - 1; i >= 0; i--)
                {
                    if (items[i].Selected)
                    {
                        items.RemoveAt(i);

                        if (selectedIndex == i)
                            selectedIndex = -1;
                        else if (selectedIndex > i)
                            selectedIndex--;

                        removed = true;
                    }
                }
                if (removed) OnListSizeChanged(false);
            }
            else
            {
                if (selectedIndex >= 0)
                {
                    items.RemoveAt(selectedIndex);
                    selectedIndex = -1;
                    OnListSizeChanged(false);
                }
            }
        }


        /// <summary>
        /// Is specified index selected?
        /// </summary>
        /// <param name="visibleIndex">The index.</param>
        /// <returns></returns>
        public bool IsSelected(int visibleIndex)
        {
            if (visibleIndex < 0)
                return false;

            if (SearchActive)
            {
                if (!multiSelectable)
                    return items[findlist[visibleIndex]].SelectedIndex;
                return items[findlist[visibleIndex]].Selected;
            }

            if (!multiSelectable)
                return items[visibleIndex].SelectedIndex;
            return items[visibleIndex].Selected;
        }


        /// <summary>
        /// Determines whether the specified index is selected index.
        /// </summary>
        /// <param name="visibleIndex"> The visible index.</param>
        public bool IsSelectedIndex(int visibleIndex)
        {
            if (SearchActive)
            {
                if (visibleIndex >= findlist.Length || visibleIndex < 0)
                    return false;
                return items[findlist[visibleIndex]].SelectedIndex;
            }
            if (visibleIndex >= items.Count || visibleIndex < 0)
                return false;
            return items[visibleIndex].SelectedIndex;
        }


        /// <summary>
        /// Makes the specified selection of items.
        /// </summary>
        /// <param name="selection">The specified selection mode.</param>
        /// <param name="visibleIndex"> visible index in the list that is used to make some selections.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">selection</exception>
        public void Select(SelectionMode selection, int visibleIndex = -1)
        {
            switch (selection)
            {
                case SelectionMode.None:
                case SelectionMode.InverseAll:
                case SelectionMode.All:
                    break;
                default:
                    if (visibleIndex < 0 || visibleIndex >= VisibleCount)
                        return; // invalid index
                    break;
            }

            bool selectionChanged = false;
            switch (selection)
            {
                case SelectionMode.None:
                    foreach (var item in items)
                    {
                        item.Selected = false;
                        item.SelectedIndex = false;
                    }
                    break;
                case SelectionMode.One:
                    selectionChanged = SoloSelect(visibleIndex);
                    break;
                case SelectionMode.Add:
                    selectionChanged = AddSelectOne(visibleIndex);
                    break;
                case SelectionMode.Inverse:
                    selectionChanged = InverseSelectOne(visibleIndex);
                    break;
                case SelectionMode.InverseAll:
                    selectionChanged = InverseSelectAll();
                    break;
                case SelectionMode.All:
                    selectionChanged = SelectAll();
                    break;
                case SelectionMode.AddGroup:
                    selectionChanged = AddGroup(visibleIndex);
                    break;
                case SelectionMode.GroupOnly:
                    selectionChanged = SelectGroup(visibleIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("selection");
            }
            if (selectionChanged)
            {
                if (visibleIndex > -1 && IsSelectedIndex(visibleIndex))
                {
                    selectedIndex = visibleIndex;
                }
                else
                {
                    SeekSelectedIndex();
                }

                OnListSelectionChanged();
            }
        }


        private void SeekSelectedIndex()
        {
            selectedIndex = -1;
            if (SearchActive)
            {
                for (int i = 0; i < findlist.Length; i++)
                {
                    if (items[findlist[i]].SelectedIndex)
                    {
                        selectedIndex = i;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].SelectedIndex)
                    {
                        selectedIndex = i;
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Select by object if object is in this list
        /// </summary>
        /// <param name="objectToSelect"></param>
        public void Select(T objectToSelect)
        {
            if (SearchActive)
            {
                for (int i = 0; i < findlist.Count(); i++)
                {
                    if (items[findlist[i]].Content.Equals(objectToSelect))
                        Select(SelectionMode.One, i);
                }
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Content.Equals(objectToSelect))
                        Select(SelectionMode.One, i);
                }
            }
        }


        /// <summary>
        /// Does the drag by moving items to their correct positions.
        /// </summary>
        /// <param name="toIndex">To index.</param>
        public virtual void DoDrag(int toIndex)
        {
            if (toIndex < 0 || toIndex > VisibleCount)
                return; // illegal drag

            if (SearchActive)
            {
                if (toIndex != 0)
                {
                    if (toIndex == VisibleCount)
                        toIndex = findlist[toIndex - 1] + 1;
                    else
                        toIndex = findlist[toIndex];
                }
            }


            if (multiSelectable)
            {
                int last = toIndex;

                // This is the actual "move" algorithm
                // arranges drag items in the same order they were originally
                for (int i = 0; i <= items.Count - 1; i++)
                {
                    if (items[i].Selected)
                    {
                        Selectable<T> tempItem = items[i];

                        if (i >= toIndex)
                        {
                            for (int j = i; j >= toIndex + 1; j += -1)
                            {
                                items[j] = items[j - 1];
                            }
                            items[toIndex] = tempItem;
                            toIndex += 1;
                        }
                        else if (i <= last)
                        {
                            for (int j = i; j <= toIndex - 1; j++)
                            {
                                items[j] = items[j + 1];
                            }
                            items[toIndex] = tempItem;
                            i = i - 1;
                            last = last - 1;
                        }
                    }
                }

                // scan the new selected index
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].SelectedIndex)
                    {
                        selectedIndex = i;
                        break;
                    }
                }
            }
            else
            {
                // none selected
                if (selectedIndex < 0)
                    return;

                // do a quick swap
                Selectable<T> tempItem = items[toIndex];
                items[toIndex] = items[selectedIndex];
                items[selectedIndex] = tempItem;
                selectedIndex = toIndex;
            }
        }


        public void OnListSizeChanged(bool enlargement)
        {
            if (ListSizeChanged != null) ListSizeChanged(enlargement);
        }


        public void OnListSelectionChanged()
        {
            if (ListSelectionChanged != null) ListSelectionChanged();
        }

        #region Selection private methods

        /// <summary>
        /// Adds the group ranging from selectedIndex to specified index to selection.
        /// </summary>
        /// <param name="visibleIndexTo">The visible index to select up to.</param>
        private bool AddGroup(int visibleIndexTo)
        {
            var changed = false;
            if (multiSelectable && selectedIndex >= 0)
            {
                if (SearchActive)
                {
                    int visibleSelectedIndex = -1;
                    for (int i = 0; i < findlist.Length; i++)
                    {
                        if (findlist[i] == selectedIndex)
                        {
                            visibleSelectedIndex = findlist[i];
                            break;
                        }
                    }

                    if (visibleSelectedIndex < 0)
                        return false; // no valid selection possible

                    int lowBound = Math.Min(visibleSelectedIndex, visibleIndexTo);
                    int highBound = Math.Max(visibleSelectedIndex, visibleIndexTo);


                    for (int i = lowBound; i < highBound; i++)
                    {
                        if (items[findlist[i]].Selected == false)
                            changed = true;
                        items[findlist[i]].Selected = true;
                    }
                }
                else
                {
                    int lowBound = Math.Min(selectedIndex, visibleIndexTo);
                    int highBound = Math.Max(selectedIndex, visibleIndexTo);

                    for (int i = lowBound; i < highBound; i++)
                    {
                        if (items[i].Selected == false)
                            changed = true;
                        items[i].Selected = true;
                    }
                }
            }
            return changed;
        }


        /// <summary>
        /// Selects the group ranging from selectedIndex to specified index.
        /// </summary>
        /// <param name="visibleIndexTo">The visible index to select up to.</param>
        private bool SelectGroup(int visibleIndexTo)
        {
            var changed = false;

            if (multiSelectable && selectedIndex >= 0)
            {
                if (SearchActive)
                {
                    int visibleSelectedIndex = -1;
                    for (int i = 0; i < findlist.Length; i++)
                    {
                        if (findlist[i] == selectedIndex)
                        {
                            visibleSelectedIndex = findlist[i];
                            break;
                        }
                    }
                    if (visibleSelectedIndex < 0)
                        return false; // no valid selection possible

                    int lowBound = Math.Min(visibleSelectedIndex, visibleIndexTo);
                    int highBound = Math.Max(visibleSelectedIndex, visibleIndexTo);

                    for (int i = 0; i < VisibleCount; i++)
                    {
                        // selected if and only if inside bounds
                        var selected = !(i < lowBound || i > highBound);
                        if (items[findlist[i]].Selected != selected)
                            changed = true;

                        items[findlist[i]].Selected = selected;
                    }
                }
                else
                {
                    int lowBound = Math.Min(selectedIndex, visibleIndexTo);
                    int highBound = Math.Max(selectedIndex, visibleIndexTo);

                    for (int i = 0; i < VisibleCount; i++)
                    {
                        // selected if and only if inside bounds
                        var selected = !(i < lowBound || i > highBound);
                        if (items[i].Selected != selected)
                            changed = true;
                        items[i].Selected = !(i < lowBound || i > highBound);
                    }
                }
            }
            return changed;
        }


        /// <summary>
        /// Selects all visible indexes and deselects hidden objects
        /// </summary>
        private bool SelectAll()
        {
            if (!multiSelectable)
                return false;

            if (SearchActive)
            {
                for (int i = 0; i <= items.Count - 1; i++)
                {
                    items[i].Selected = false;
                }

                for (int i = 0; i <= findlist.Length - 1; i++)
                {
                    items[findlist[i]].Selected = true;
                }
            }
            else
            {
                for (int i = 0; i <= items.Count - 1; i++)
                {
                    items[i].Selected = true;
                }
            }
            return true;
        }


        /// <summary>
        /// inverses selection for a single item, generally done by holding ctrl and mouse clicking item
        /// </summary>
        private bool InverseSelectOne(int visibleIndex)
        {
            if (SearchActive)
            {
                if (multiSelectable)
                {
                    items[findlist[visibleIndex]].Selected = !items[findlist[visibleIndex]].Selected;
                    if (items[findlist[visibleIndex]].Selected)
                    {
                        items[findlist[visibleIndex]].SelectedIndex = true;
                    }
                }
                else
                    items[findlist[visibleIndex]].SelectedIndex = true;

                selectedIndex = visibleIndex;
            }
            else
            {
                if (multiSelectable)
                {
                    items[visibleIndex].Selected = !items[visibleIndex].Selected;
                    if (items[visibleIndex].Selected)
                    {
                        items[visibleIndex].SelectedIndex = true;
                    }
                }
                else
                    items[visibleIndex].SelectedIndex = true;

                selectedIndex = visibleIndex;
            }
            return true;
        }


        private bool InverseSelectAll()
        {
            if (!multiSelectable || VisibleCount < 1)
                return false;

            var changed = false;

            if (SearchActive)
            {
                int searchIndex = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    if (searchIndex < findlist.Length && findlist[searchIndex] == i)
                    {
                        items[i].Selected = !items[i].Selected;
                        changed = true;
                    }
                    else
                    {
                        items[i].Selected = false; // no changed selection event because change is hidden
                    }

                }
            }
            else
            {
                foreach (var item in items)
                {
                    item.Selected = !item.Selected;
                }
                changed = true;
            }

            return changed;
        }


        /// <summary>
        /// adds selection for a single item, generally done by holding shift and mouse clicking item
        /// </summary>
        private bool AddSelectOne(int visibleIndex)
        {
            var changed = false;

            if (SearchActive)
            {
                if (multiSelectable)
                {
                    int searchIndex = 0;
                    for (int i = 0; i < items.Count; i++)
                    {
                        if (searchIndex < findlist.Length && findlist[searchIndex] == i)
                        {


                            if (findlist[searchIndex] == findlist[visibleIndex])
                            {
                                if (!items[i].Selected || !items[i].SelectedIndex)
                                    changed = true;

                                items[findlist[searchIndex]].Selected = true;
                                items[findlist[searchIndex]].SelectedIndex = true;
                            }
                            else
                            {
                                // not the selected item
                                if (items[i].SelectedIndex)
                                    changed = true;

                                items[i].SelectedIndex = false;
                            }
                            searchIndex++;
                        }
                        else
                        {
                            items[i].Selected = false; // no changed selection event because change is hidden
                            items[i].SelectedIndex = false; // no changed selection event because change is hidden
                        }
                    }
                }
                else if (selectedIndex >= 0)
                {
                    if (items[selectedIndex].SelectedIndex || !items[findlist[visibleIndex]].SelectedIndex)
                        changed = true;

                    items[selectedIndex].SelectedIndex = false;
                    items[findlist[visibleIndex]].SelectedIndex = true;
                }
                selectedIndex = visibleIndex;
            }
            else
            {
                if (multiSelectable)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        var result = (i == visibleIndex);
                        if ((!items[i].Selected && result) || items[i].SelectedIndex != result)
                            changed = true;

                        if (result) items[i].Selected = true;

                        items[i].SelectedIndex = result;
                    }
                }
                else
                {
                    if (selectedIndex >= 0)
                    {
                        if (items[selectedIndex].SelectedIndex)
                            changed = true;
                        items[selectedIndex].SelectedIndex = false;
                    }
                    if (!items[visibleIndex].SelectedIndex)
                        changed = true;

                    items[visibleIndex].SelectedIndex = true;
                }

                selectedIndex = visibleIndex;
            }
            return changed;
        }


        /// <summary>
        /// Selects a single item from the listbox.
        /// </summary>
        /// <param name="visibleIndex">The index.</param>
        private bool SoloSelect(int visibleIndex)
        {
            var changed = false;
            if (SearchActive)
            {
                if (multiSelectable)
                {
                    int searchIndex = 0;
                    for (int i = 0; i < items.Count; i++)
                    {
                        if (searchIndex < findlist.Length && findlist[searchIndex] == i)
                        {
                            if (findlist[searchIndex] == findlist[visibleIndex])
                            {
                                if (!items[i].Selected || !items[i].SelectedIndex)
                                    changed = true;

                                items[findlist[searchIndex]].Selected = true;
                                items[findlist[searchIndex]].SelectedIndex = true;
                            }
                            else
                            {
                                // not the selected item
                                if (items[i].Selected)
                                    changed = true;

                                items[i].Selected = false;
                                items[i].SelectedIndex = false;
                            }
                            searchIndex++;
                        }
                        else
                        {
                            items[i].Selected = false; // no changed selection event because change is hidden
                            items[i].SelectedIndex = false; // no changed selection event because change is hidden
                        }
                    }
                }
                else
                {
                    if (items[findlist[visibleIndex]].SelectedIndex) return false;

                    changed = true;
                    if (selectedIndex >= 0)
                    { 
                        foreach (var item in items)
                        {
                            item.Selected = false;
                            item.SelectedIndex = false;
                        }
                    }
                    items[findlist[visibleIndex]].SelectedIndex = true;
                }
                selectedIndex = visibleIndex;
            }
            else
            {
                if (multiSelectable)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        var result = (i == visibleIndex);
                        if (items[i].Selected != result || items[i].SelectedIndex != result)
                            changed = true;

                        items[i].Selected = result;
                        items[i].SelectedIndex = result;
                    }
                }
                else
                {
                    if (selectedIndex >= 0)
                    {
                        if (items[selectedIndex].SelectedIndex)
                            changed = true;
                        items[selectedIndex].SelectedIndex = false;
                    }
                    if (!items[visibleIndex].SelectedIndex)
                        changed = true;

                    items[visibleIndex].SelectedIndex = true;
                }

                selectedIndex = visibleIndex;
            }
            return changed;
        }

        #endregion

        public void Sort(IComparer<Selectable<T>> comparer)
        {
            var lastSelected = default(T);
            if (selectedIndex >= 0)
            {
                // store selection
                lastSelected = SearchActive ? items[findlist[selectedIndex]].Content : items[selectedIndex].Content;
            }

            items.Sort(comparer);
            UpdateItems();

            if (lastSelected != null && !lastSelected.Equals(default(T)))
            {
                // restore selection
                SeekSelectedIndex();
            }
        }


        public int GetSelectedIndex()
        {
            return selectedIndex;
        }
    }
}