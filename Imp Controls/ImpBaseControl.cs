using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Base;

namespace ImpControls
{
    public abstract class ImpBaseControl : Control
    {
        protected ControlRenderData renderData;

        protected bool sPressed;
        protected bool sMouseover;
        protected bool sSolid = false;

        protected bool sGluedFocus;

        protected StyleClass sStyle = new StyleClass();


        /// <summary>
        /// Proper event for clicking a button
        /// </summary>
        public event SClickedEventHandler Clicked;
        public delegate void SClickedEventHandler(object sender);


        /// <summary>
        /// Call out for opening menu for right clicking if available.
        /// </summary>
        public event OpenRightClickMenuEventHandler OpenRightClick_Menu;
        public delegate void OpenRightClickMenuEventHandler(object sender, System.Windows.Input.MouseButtonEventArgs e);


        public ImpBaseControl()
        {

            this.IsEnabledChanged += OnEnabledChange;
            IsVisibleChanged += OnVisibilityChanged;
            IsTabStop = false;
        }


        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Key == Key.Tab || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Down || e.Key == Key.Up)
            {
                e.Handled = true;
            }
        }


        protected virtual void OnVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue && (bool)e.OldValue)
            {
                ControlGetsHidden();
            }
            
        }


        protected virtual void ControlGetsHidden()
        {
            MouseOver = false;
            Pressed = false;
            InvalidateVisual();
        }


        private void OnEnabledChange(object sender, DependencyPropertyChangedEventArgs e)
        {
            MouseOver = false;
            Pressed = false;
            InvalidateVisual();
        }


        /// <summary>
        /// Is the left mouse button being pressed over this control?
        /// </summary>
        protected virtual bool Pressed
        {
            get { return sPressed; }
            set
            {
                if (value == Pressed)
                    return;
                sPressed = value;
                this.InvalidateVisual();
            }
        }

        /// <summary>
        /// Is the mouse over this control? 
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks> WPF default, isMouseOver thinks mouse is always over the control when button is held.</remarks>
        protected virtual bool MouseOver
        {
            get { return sMouseover; }
            set
            {
                if (value == sMouseover)
                    return;
                sMouseover = value;
                this.InvalidateVisual();
            }
        }

        /// <summary>
        /// Set style to match the current application theme
        /// </summary>
        /// <param name="value"></param>
        public void SetStyle(StyleClass value)
        {
            sStyle = value;
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            if (IsEnabled & !Pressed)
            {
                MouseOver = true;
            }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            Pressed = false;
            if (this.IsEnabled & !Pressed)
            {
                MouseOver = false;
            }
            base.OnMouseLeave(e);
        }

        

        protected override void OnPreviewMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.IsEnabled && e.LeftButton == MouseButtonState.Pressed && IsMouseDirectlyOver)
            {
                //If e.MouseDevice.DirectlyOver Is e.MouseDevice.Target Then
                sMouseover = true;
                Pressed = true;
                this.CaptureMouse();
                //End If
            }

            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (OpenRightClick_Menu != null)
                {
                    OpenRightClick_Menu(this, e);
                }
            }
            base.OnPreviewMouseDown(e);
        }


        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Focus();
            base.OnMouseDown(e);
        }


        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            this.ReleaseMouseCapture();
            
            if (this.IsEnabled && Pressed)
            {
                // e.MouseDevice.DirectlyOver Is e.MouseDevice.Target And Pressed Then
                if (HitTest(e.GetPosition(this)))
                {
                    OnClicked();
                }
            }
            Pressed = false;
            base.OnMouseUp(e);
        }


        protected void OnClicked()
        {
            if (Clicked != null)
            {
                Clicked(this);
            }
        }

        

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if (Pressed)
            {
                if (HitTest(e.GetPosition(this)))
                {
                    MouseOver = true;
                }
                else
                {
                    MouseOver = false;
                }
            }
            base.OnMouseMove(e);
        }


        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            if (this.ActualHeight <= 0 | this.ActualWidth <= 0)
                return; // not visible

            AdjustRenderColors();

            DrawBackground(drawingContext, renderData.BackBrush);
            DrawContent(drawingContext);
            Drawborders(drawingContext, renderData.BorderBrush);
        }


        

        private void DrawBackground(DrawingContext drawingContext, Brush brush)
        {
            drawingContext.DrawRectangle(brush, null, new Rect(0, 0, this.ActualWidth, this.ActualHeight));

        }

        protected abstract void DrawContent(DrawingContext drawingContext);


        private void Drawborders(DrawingContext drawingContext, Brush borderbrush)
        {
            drawingContext.DrawLine(new Pen(borderbrush, sStyle.BaseBorderThickness.Left), new Point(0.5, GetSH()), new Point(0.5, GetSH(false)));
            drawingContext.DrawLine(new Pen(borderbrush, sStyle.BaseBorderThickness.Top), new Point(0.5, GetSH()), new Point(this.ActualWidth - 0.5, GetSH()));
            drawingContext.DrawLine(new Pen(borderbrush, sStyle.BaseBorderThickness.Right), new Point(this.ActualWidth - 0.5, GetSH()), new Point(this.ActualWidth - 0.5, GetSH(false)));
            drawingContext.DrawLine(new Pen(borderbrush, sStyle.BaseBorderThickness.Bottom), new Point(0.5, GetSH(false)), new Point(this.ActualWidth - 0.5, GetSH(false)));
        }


        /// <summary>
        /// Get Scroller height, as in position where to draw
        /// </summary>
        /// <param name="upper">upper as in lower value of Y</param>
        /// <returns></returns>
        protected double GetSH(bool upper = true)
        {
            double value = 0;
            if (upper)
                value = 0.5;
            else
                value = Math.Floor(this.ActualHeight) - 0.5;

            return value;
        }

        /// <summary>
        /// Adjusts renderData to be correct for current control state
        /// </summary>
        protected virtual void AdjustRenderColors()
        {
            if( !IsEnabled)
            {
                renderData.BackBrush = sStyle.BackDisabledBrush;
                renderData.BorderBrush = sStyle.BorderDisabledBrush;
                renderData.FrontBrush = sStyle.DisabledBrush;
                renderData.ResetTranslate();
            }
            else if (Pressed & MouseOver)
            {
                renderData.BackBrush = sStyle.BackPressedBrush;
                renderData.BorderBrush = sStyle.BorderPressedBrush;
                renderData.FrontBrush = sStyle.PressedBrush;
                renderData.SetTranslate(sStyle.PressedTranslation);
            }
            else if (MouseOver | Pressed)
            {
                renderData.BackBrush = sStyle.BackMouseoverBrush;
                renderData.BorderBrush = sStyle.BorderMouseoverBrush;
                renderData.FrontBrush = sStyle.MouseoverBrush;
                renderData.ResetTranslate();
            }
            else
            {
                renderData.BackBrush = sStyle.BackNormalBrush;
                renderData.BorderBrush = sStyle.BorderBrush;
                renderData.FrontBrush = sStyle.NormalBrush;
                renderData.ResetTranslate();
            }
        }

        public virtual bool HitTest(Point cursorPos)
        {
            if (cursorPos.X < 0 | cursorPos.X > this.ActualWidth | cursorPos.Y < 0 | cursorPos.Y > this.ActualHeight)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// This should be used when determining tooltips with absolute position to ensure proper
        /// tooltip placement on the screen regardless of Windows dpi settings
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static Point GetDpiSafeLocation(Point location)
        {
            PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);
            if (source != null)
            {
                double dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                double dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
                return new Point(location.X * 96.0 / dpiX, location.Y * 96.0 / dpiY);
            }
            return location;
        }
    }
}