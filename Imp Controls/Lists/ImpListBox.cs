#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Base;
using Base.Commands;
using Base.ListLogic;
using SelectionMode = Base.ListLogic.SelectionMode;

#endregion

namespace ImpControls
{
    public delegate void ListSelectionChangedEventHandler();

    public class ImpListBox<T> : ImpBaseControl
    {
        #region Static Fields and Constants

        protected const int SCROLLBARWIDTH = 10;

        #endregion

        #region Fields

        protected ListController<T> controller;

        /// <summary>
        ///     Indicates where to drag current selection
        /// </summary>
        /// <remarks>
        ///     DragTo equals -1 when the drag is inactive. 0 places selection to the first index in playlist.
        /// </remarks>
        protected int dragTo = -1;

        protected MouseStates mouseoverState = MouseStates.None;
        protected MouseStates pressedState = MouseStates.None;
        protected ToolTip toolTip = new ToolTip();
        internal int RowHeight = 15;
        private int highIndex;
        private int lowIndex;
        //Mouse over index, the list box item that the mouse currently is over.
        private int mouseOverIndex = -1;

        #endregion

        #region Properties

        public string FindText
        {
            get { return controller.FindText; }
            set { controller.FindText = value; }
        }

        
        public bool ItemsDragable { get; set; }
        public bool AcceptsNullSelection { get; set; }

        /// <summary>
        ///     Lowest index number visible to user
        /// </summary>
        public virtual int LowIndex
        {
            get { return lowIndex; }
            set
            {
                var TIndex = lowIndex;
                if (lowIndex == value)
                    return;
                if (controller.VisibleCount <= 0)
                {
                    if (value == 0 & TIndex == -1)
                        InvalidateVisual();
                    lowIndex = 0;
                    highIndex = 0;
                    return;
                }
                lowIndex = value;
                if (lowIndex < 0)
                    lowIndex = 0;
                highIndex = lowIndex + (int) Math.Floor(ActualHeight / RowHeight) - 1;
                if (highIndex > controller.VisibleCount - 1)
                {
                    highIndex = controller.VisibleCount - 1;
                    lowIndex = highIndex - (int) Math.Floor(ActualHeight / RowHeight) + 1;
                }
                if (lowIndex < 0)
                    lowIndex = 0;
                if (TIndex != lowIndex)
                    InvalidateVisual();
            }
        }

        /// <summary>
        ///     Highest index number visible to user
        /// </summary>
        public virtual int HighIndex
        {
            get { return highIndex; }
            set
            {
                var TIndex = highIndex;
                if (highIndex == value)
                    return;
                if (controller.VisibleCount <= 0)
                {
                    lowIndex = 0;
                    highIndex = 0;
                    return;
                }
                highIndex = value;
                if (highIndex > controller.VisibleCount - 1)
                {
                    highIndex = controller.VisibleCount - 1;
                }
                lowIndex = highIndex - (int) Math.Floor(ActualHeight / RowHeight);
                if (lowIndex < 0)
                    lowIndex = 0;
                if (TIndex != highIndex)
                    InvalidateVisual();
            }
        }

        /// <summary>
        ///     Index that mouse is over, -1 if none.
        /// </summary>
        public virtual int MouseoverIndex
        {
            get { return mouseOverIndex; }
            set
            {
                if (mouseOverIndex == value)
                    return;

                mouseOverIndex = value;
                if (mouseOverIndex < lowIndex)
                    mouseOverIndex = -1;
                if (mouseOverIndex > highIndex)
                    mouseOverIndex = -1;

                InvalidateVisual();
            }
        }

        private bool ScrollBarVisible
        {
            get { return HighIndex - LowIndex < controller.VisibleCount - 1; }
        }

        #endregion

        public ImpListBox(bool searchable, bool multiSelectable)
        {
            CreateController(searchable, multiSelectable);

            SizeChanged += ImpListBoxBase_SizeChanged;
            MouseWheel += ImpListBoxBase_MouseWheel;
            MouseMove += ImpListBoxBase_MouseMove;
            MouseLeave += ImpListBoxBase_MouseLeave;
            MouseEnter += ImpListBoxBase_MouseEnter;
            MouseDown += ImpListBoxBase_MouseDown;
            MouseDoubleClick += ImpListBoxBase_MouseDoubleClick;

            controller.ListSizeChanged += ListSizeChanged;
            controller.ListSelectionChanged += OnSelectionChanged; // need redrawing when list changes
            //selected = New List(Of Boolean)
            //For i As Integer = 1 To 20
            //    selected.Add(False)
            //Next

            ToolTip = toolTip;
            //toolTip.StaysOpen = true;
        }

        #region Delegates & Events

        /// <summary>
        ///     This event is called when selectedindex changes
        /// </summary>
        public event ListSelectionChangedEventHandler SelectionChanged;

        #endregion

        protected virtual void CreateController(bool searchable, bool multiSelectable)
        {
            controller = new ListController<T>(searchable, multiSelectable);
        }

        private void OnSelectionChanged()
        {
            if (SelectionChanged != null)
            {
                SelectionChanged();
            }
            InvalidateVisual();
        }

        private void ListSizeChanged(bool enlargement)
        {
            InvalidateVisual();
            UpdateItems();
        }

        protected override void ControlGetsHidden()
        {
            base.ControlGetsHidden();
            pressedState = MouseStates.None;
        }

        public void Select(int index)
        {
            controller.Select(SelectionMode.One, index);
        }

        public void SelectNone()
        {
            controller.Select(SelectionMode.None);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (MouseoverIndex > -1 && MouseoverIndex < controller.VisibleCount)
            {
                var p = GetDpiSafeLocation(PointToScreen(e.GetPosition(this)));

                toolTip.HorizontalOffset = p.X + 10;
                toolTip.VerticalOffset = p.Y + 10;
                toolTip.Placement = PlacementMode.AbsolutePoint;

                GetTooltip();
                if (!string.IsNullOrEmpty(toolTip.Content as string))
                {
                    toolTip.IsOpen = true;
                    toolTip.Visibility = Visibility.Visible;
                }

                //toolTip.IsReallyOpen = true;
            }
            else
            {
                toolTip.Visibility = Visibility.Hidden;
                //toolTip.HorizontalOffset = e.GetPosition(null).X + 10;
                //toolTip.VerticalOffset = e.GetPosition(null).Y + 10;
                toolTip.Placement = PlacementMode.RelativePoint;
                //toolTip.StaysOpen = false;
                toolTip.Content = string.Empty;
                toolTip.IsOpen = false;
            }
        }

        protected virtual void GetTooltip() {}

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            //toolTip.StaysOpen = false;
            toolTip.IsOpen = false;
        }

        protected override void DrawContent(DrawingContext drawingContext)
        {
            //MyBase.OnRender(drawingContext)
            if (ActualWidth <= 0 || ActualHeight <= 0)
            {
                return;
            }

            Brush brush;
            Brush borderbrush;


            if (!IsEnabled)
            {
                brush = sStyle.BackDisabledBrush;
                borderbrush = sStyle.BorderDisabledBrush;
            }
            else if (MouseOver | pressedState == MouseStates.WindowPressed)
            {
                brush = sStyle.BackMouseoverBrush;
                borderbrush = sStyle.BorderMouseoverBrush;
            }
            else
            {
                brush = sStyle.BackNormalBrush;
                borderbrush = sStyle.BorderBrush;
            }


            drawingContext.DrawRectangle(brush, null, new Rect(0, 0, ActualWidth, ActualHeight));


            drawingContext.DrawLine(new Pen(borderbrush, sStyle.BaseBorderThickness.Left), new Point(0.5, 0.5),
                new Point(0.5, ActualHeight - 0.5));
            drawingContext.DrawLine(new Pen(borderbrush, sStyle.BaseBorderThickness.Top), new Point(0.5, 0.5),
                new Point(GetSW(false), 0.5));
            drawingContext.DrawLine(new Pen(borderbrush, sStyle.BaseBorderThickness.Right), new Point(GetSW(false), 0.5),
                new Point(GetSW(false), ActualHeight - 0.5));
            drawingContext.DrawLine(new Pen(borderbrush, sStyle.BaseBorderThickness.Bottom),
                new Point(0.5, ActualHeight - 0.5),
                new Point(GetSW(false), ActualHeight - 0.5));

            DrawScrollbar(drawingContext);

            if (controller.VisibleCount < 1)
                return;

            // Actual loop for drawing list items
            for (var i = LowIndex; i <= Math.Min(HighIndex, controller.VisibleCount - 1); i++)
            {
                brush = getBrush(i, drawingContext, brush);
                DrawText(i, drawingContext, brush);
            }

            //' Draws to line where DragTo points
            if (dragTo >= lowIndex && dragTo <= highIndex)
            {
                if (dragTo < controller.GetSelectedIndex())
                    drawingContext.DrawLine(new Pen(sStyle.PressedBrush, 1),
                        new Point(0, (dragTo - LowIndex) * RowHeight + 3),
                        new Point(GetSW(), (dragTo - LowIndex) * RowHeight + 3));
                else
                    drawingContext.DrawLine(new Pen(sStyle.PressedBrush, 1),
                        new Point(0, (dragTo - LowIndex + 1) * RowHeight + 3),
                        new Point(GetSW(), (dragTo - LowIndex + 1) * RowHeight + 3));
            }
        }

        private void DrawScrollbar(DrawingContext drawingContext)
        {
            if (!ScrollBarVisible)
            {
                return; // scrollbar is not required, everything is visible
            }

            Brush scrollerbrush;
            Brush brush;
            Brush borderbrush;


            if (pressedState == MouseStates.PanRightPressed)
            {
                scrollerbrush = sStyle.PressedBrush;
                brush = sStyle.BackPressedBrush;
                borderbrush = sStyle.BorderPressedBrush;
            }
            else if (mouseoverState == MouseStates.PanRightPressed)
            {
                scrollerbrush = sStyle.MouseoverBrush;
                brush = sStyle.BackMouseoverBrush;
                borderbrush = sStyle.BorderMouseoverBrush;
            }
            else
            {
                scrollerbrush = sStyle.NormalBrush;
                brush = sStyle.BackNormalBrush;
                borderbrush = sStyle.BorderBrush;
            }


            drawingContext.DrawRectangle(brush, null, SafeRect(GetSW() + 0.5, 1, SCROLLBARWIDTH - 1, GetSH(true, false) - 1.5));

            drawingContext.DrawLine(new Pen(borderbrush, sStyle.BaseBorderThickness.Left), new Point(GetSW(), GetSH()),
                new Point(GetSW(), GetSH(true, false)));
            drawingContext.DrawLine(new Pen(borderbrush, sStyle.BaseBorderThickness.Top), new Point(GetSW(), GetSH()),
                new Point(GetSW(false), GetSH()));
            drawingContext.DrawLine(new Pen(borderbrush, sStyle.BaseBorderThickness.Right),
                new Point(GetSW(false), GetSH()), new Point(GetSW(false), GetSH(true, false)));
            drawingContext.DrawLine(new Pen(borderbrush, sStyle.BaseBorderThickness.Bottom),
                new Point(GetSW(), GetSH(true, false)), new Point(GetSW(false), GetSH(true, false)));


            drawingContext.DrawLine(new Pen(borderbrush, 1), new Point(GetSW(), GetSH(false)),
                new Point(GetSW(false), GetSH(false)));

            drawingContext.DrawLine(new Pen(borderbrush, 1), new Point(GetSW(), GetSH(false, false)),
                new Point(GetSW(false), GetSH(false, false)));


            if (GetSH(false) - GetSH(false, false) + 1 < 0)
                drawingContext.DrawRectangle(scrollerbrush, null,
                    new Rect(GetSW() + 1, GetSH(false) + 0.5, SCROLLBARWIDTH - 2,
                        GetSH(false, false) - GetSH(false) - 1));
        }

        private Rect SafeRect(double x, double y, double height, double width)
        {
            if (width < 0) width = 0;
            if (height < 0) height = 0;
            var rect = new Rect(x, y, height, width);

            return rect;
        }

        /// <summary>
        ///     Get Scroller height, as in position where to draw
        /// </summary>
        /// <param name="bounds">outer bounds of the scroller</param>
        /// <param name="upper">upper as in lower value of Y</param>
        /// <returns></returns>
        protected double GetSH(bool bounds = true, bool upper = true)
        {
            double value = 0;
            if (bounds && upper)
                value = 0.5;

            else if (bounds)
                value = Math.Floor(ActualHeight) - 0.5;

            else if (upper)
                value = Math.Floor((ActualHeight - 1) * LowIndex / (controller.VisibleCount - 1)) + 0.5;
            else
                value = GetSH(false) +
                        Math.Ceiling((ActualHeight - 1) * (HighIndex - LowIndex) / (controller.VisibleCount - 1));


            return value;
        }

        /// <summary>
        ///     Get Scroller width, as in position where to draw
        /// </summary>
        /// <param name="left">left as in lower value of x</param>
        /// <returns></returns>
        protected double GetSW(bool left = true)
        {
            double value = 0;
            if (left)
                value = ActualWidth - 0.5 - SCROLLBARWIDTH;
            else
                value = ActualWidth - 0.5;

            return value;
        }

        protected virtual Brush getBrush(int i, DrawingContext drawingContext, Brush brush)
        {
            if (controller.IsSelected(i))
            {
                //' Draws box if the item is selected
                if (HighIndex - LowIndex < controller.VisibleCount - 1)
                {
                    if (ActualWidth > SCROLLBARWIDTH)
                        drawingContext.DrawRectangle(sStyle.BackPressedBrush, null,
                            new Rect(0, (i - LowIndex) * RowHeight + 3,
                                ActualWidth - SCROLLBARWIDTH, RowHeight));
                }
                else
                {
                    drawingContext.DrawRectangle(sStyle.BackPressedBrush, null,
                        new Rect(0, (i - LowIndex) * RowHeight + 3, ActualWidth, RowHeight));
                }
            }


            if (!IsEnabled)
            {
                brush = sStyle.DisabledBrush;
            }
            else if (controller.IsSelectedIndex(i))
            {
                var penl = new Pen(sStyle.PressedBrush, 1);
                penl.DashStyle = DashStyles.Dash;
                //' Draws lines for selectedindex
                if (HighIndex - LowIndex < controller.VisibleCount - 1)
                {
                    drawingContext.DrawLine(penl, new Point(0, (i - LowIndex) * RowHeight + 3),
                        new Point(ActualWidth - SCROLLBARWIDTH, (i - LowIndex) * RowHeight + 3));
                    drawingContext.DrawLine(penl, new Point(0, (i - LowIndex + 1) * RowHeight + 3 - 1f),
                        new Point(ActualWidth - SCROLLBARWIDTH,
                            (i - LowIndex + 1) * RowHeight + 3 - 1f));
                }
                else
                {
                    drawingContext.DrawLine(penl, new Point(0, (i - LowIndex) * RowHeight + 3),
                        new Point(ActualWidth, (i - LowIndex) * RowHeight + 3));
                    drawingContext.DrawLine(penl, new Point(0, (i - LowIndex + 1) * RowHeight + 3 - 1f),
                        new Point(ActualWidth, (i - LowIndex + 1) * RowHeight + 3 - 1f));
                }

                brush = sStyle.PressedBrush;
            }
            else if (i == MouseoverIndex)
            {
                brush = sStyle.MouseoverBrush;
            }
            else
            {
                brush = sStyle.NormalBrush;
            }
            return brush;
        }

        protected virtual void DrawText(int index, DrawingContext drawingContext, Brush brush)
        {
            if (ActualWidth - SCROLLBARWIDTH < 0)
                return;

            var text = FormatText(controller.GetText(index), ref brush);
            if (text.MaxTextWidth > 0)
                drawingContext.DrawText(text, new Point(3, (index - LowIndex) * RowHeight + 3));
        }

        protected virtual FormattedText FormatText(string text, ref Brush brush)
        {
            var fText = new FormattedText(text, CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, new Typeface("Arial"), 12, brush);


            var width = ActualWidth - 6;
            if (ScrollBarVisible)
            {
                width -= SCROLLBARWIDTH;
            }
            fText.MaxTextWidth = Math.Max(width, 0);
            fText.MaxTextHeight = RowHeight;
            fText.Trimming = TextTrimming.CharacterEllipsis;
            return fText;
        }

        public void SetList(ref T[] contentList)
        {
            controller.Clear();
            controller.AddItems(contentList);
        }

        public void SetList(ICollection<T> contentList)
        {
            controller.Clear();
            controller.AddItems(contentList);
        }

        public void AddToList(T content)
        {
            controller.AddItem(content);
        }

        public void ClearList()
        {
            controller.Clear();
        }

        public void RemoveSelected()
        {
            controller.RemoveSelected();
        }

        public List<T> GetSelectedList()
        {
            return controller.GetSelectedList();
        }

        public virtual T GetSelected()
        {
            return controller.GetSelected();
        }

        protected virtual string getText(int index, bool indexNumber)
        {
            if (indexNumber)
                return index + ". " + controller.GetText(index);

            return controller.GetText(index);
        }

        private void ImpListBoxBase_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Pressed = false;
        }

        private void ImpListBoxBase_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OnMouseDown(e);
            if (Pressed)
            {
                if (e.GetPosition(this).X >= ActualWidth - SCROLLBARWIDTH && ScrollBarVisible)
                {
                    ScrollBarMove(e.GetPosition(this).Y);
                    pressedState = MouseStates.PanRightPressed;
                }
                else
                {
                    pressedState = MouseStates.WindowPressed;
                    if (MouseoverIndex > HighIndex)
                        return;

                    if (ItemsDragable && GlobalKeyboard.ModKeys == ModifierKeys.None)
                    {
                        if (controller.IsSelected(MouseoverIndex))
                        {
                            // makes it selected index
                            controller.Select(SelectionMode.Add, MouseoverIndex);
                            DragShow(MouseoverIndex);
                        }

                        else
                            controller.Select(SelectionMode.One, MouseoverIndex);
                    }
                    else if (GlobalKeyboard.ModKeys == ModifierKeys.None)
                        controller.Select(SelectionMode.One, MouseoverIndex);
                    else if (GlobalKeyboard.ModKeys == ModifierKeys.Shift)
                        controller.Select(SelectionMode.GroupOnly, MouseoverIndex);
                    else if (GlobalKeyboard.ModKeys == ModifierKeys.Control)
                        controller.Select(SelectionMode.Inverse, MouseoverIndex);
                    else if (GlobalKeyboard.ModKeys == (ModifierKeys.Control | ModifierKeys.Shift))
                        controller.Select(SelectionMode.Add, MouseoverIndex);
                }
            }
        }

        private void ImpListBoxBase_MouseEnter(object sender, MouseEventArgs e)
        {
            CaptureMouse();
        }

        private void ImpListBoxBase_MouseLeave(object sender, MouseEventArgs e)
        {
            ClearMouse();
        }

        protected virtual void ImpListBoxBase_MouseMove(object sender, MouseEventArgs e)
        {
            if (pressedState == MouseStates.PanRightPressed)
            {
                ScrollBarMove(e.GetPosition(this).Y);
                return;
            }

            if (HitTest(e.GetPosition(this)))
            {
                var tempIndex = lowIndex + (int) Math.Floor(e.GetPosition(this).Y / RowHeight);
                if (tempIndex == MouseoverIndex)
                    return; // nothing to do here

                MouseoverIndex = tempIndex;

                if (ScrollBarVisible && e.GetPosition(this).X >= ActualWidth - SCROLLBARWIDTH ||
                    MouseoverIndex > HighIndex)
                {
                    MouseoverIndex = -1;
                    return; // no action, not over any item
                }


                if (pressedState == MouseStates.WindowPressed)
                {
                    if (GlobalKeyboard.ModKeys == ModifierKeys.None && ItemsDragable == false)
                        controller.Select(SelectionMode.One, MouseoverIndex);
                    else if (GlobalKeyboard.ModKeys == ModifierKeys.Shift)
                        controller.Select(SelectionMode.AddGroup, MouseoverIndex);
                    else if (GlobalKeyboard.ModKeys == ModifierKeys.Control)
                        controller.Select(SelectionMode.Inverse, MouseoverIndex);
                    else if (GlobalKeyboard.ModKeys == (ModifierKeys.Control | ModifierKeys.Shift))
                        controller.Select(SelectionMode.Add, MouseoverIndex);
                    else if (GlobalKeyboard.ModKeys == ModifierKeys.None & ItemsDragable)
                        DragShow(tempIndex);
                }
            }
            else
            {
                if (!Pressed)
                {
                    ReleaseMouseCapture();
                }
                mouseoverState = MouseStates.None;
            }
        }

        protected virtual void DragShow(int index)
        {
            if (controller.IsSelectedIndex(index))
            {
                dragTo = -1;
            }
            else
            {
                dragTo = Math.Min(Math.Max(index, 0), controller.VisibleCount - 1);
            }
        }

        /// <summary>
        ///     Selects all visible indexes
        /// </summary>
        public virtual void SelectALL()
        {
            controller.Select(SelectionMode.All);
        }

        /// <summary>
        ///     inverses selection on all visible indexes
        /// </summary>
        public virtual void SelectInverse()
        {
            controller.Select(SelectionMode.InverseAll);
        }

        /// <summary>
        ///     Updates the items.
        /// </summary>
        protected virtual void UpdateItems()
        {
            var t = lowIndex;
            lowIndex = -2;
            LowIndex = t;
            //SelectedIndex = SelectedIndex
            InvalidateVisual();
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (GlobalKeyboard.ModKeys == ModifierKeys.None && ItemsDragable && pressedState == MouseStates.WindowPressed)
            {
                if (dragTo > -1)
                    controller.DoDrag(dragTo);
                else if (controller.IsSelectedIndex(MouseoverIndex))
                    controller.Select(SelectionMode.One, MouseoverIndex);
            }

            if (GlobalKeyboard.ModKeys == ModifierKeys.None && e.ChangedButton == MouseButton.Right &&
                !controller.IsSelected(MouseoverIndex))
            {
                // select the one the mouse is over if it is not selected already
                controller.Select(SelectionMode.One, MouseoverIndex);
            }
            pressedState = MouseStates.None;
            base.OnMouseUp(e);
            dragTo = -1;
        }

        private void ImpListBoxBase_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var sign = Math.Sign(e.Delta);
            if (sign == 0) return;

            if (sign > 0)
                LowIndex -= Math.Max((int) (sign * (HighIndex - LowIndex) * 0.2f), 1);
            else
                LowIndex -= Math.Min((int) (sign * (HighIndex - LowIndex) * 0.2f), -1);
        }

        private void ImpListBoxBase_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateItems();
        }

        private void ClearMouse()
        {
            MouseoverIndex = -1;
            mouseoverState = MouseStates.None;
        }

        private void ScrollBarMove(double cursorY)
        {
            double scrollSize = (HighIndex - LowIndex);
            LowIndex = (int) Math.Round(cursorY / ActualHeight * (controller.VisibleCount) - scrollSize / 2);
        }
    }
}