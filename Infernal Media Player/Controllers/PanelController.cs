using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Base;
using Base.Libraries;
using Ipv;

namespace Imp.Controllers
{
    public class PanelController : IPanelController
    {
        private struct PanelPositions
        {
            public double LeftPercentage;
            public double MidPercentage;
            public double RightPercentage;
            public Point OriginalCursorPosition;
            public Point OriginalCursorPositionOnControl;
        }

        private const int resizerWidth = 5;
        private const int PanelHeight = 40;
        private const int desiredLeftWidth = 350;
        private const int desiredRightWidth = 350;
        private const int MaxDesiredLeftWidth = 600;
        private const int MaxDesiredRightWidth = 600;
        private int minLeftPanelWidth = 200;
        private int minRightPanelWidth = 200;
        
        private readonly MainWindow window;

        private PanelPositions oldPositions = new PanelPositions();

        public PanelController(MainWindow window)
        {
            this.window = window;
        }

        private bool IsLeftOpen { get { return window.grid.ColumnDefinitions[0].ActualWidth > 1; } }

        private bool IsRightOpen { get { return window.grid.ColumnDefinitions[4].ActualWidth > 1; } }

        /// <summary>
        /// hides other left panels and makes panel open visible
        /// </summary>
        private void MakeVisiblePanelOpen()
        {
            window.PanelOpen.Visibility = Visibility.Visible;
        }

        public void CommandPanelOpen(PanelCommand? panelCommand)
        {
            PanelCommand command;
            if (panelCommand == null)
                command = PanelCommand.Toggle;
            else
                command = (PanelCommand) panelCommand;


            if (command == PanelCommand.Toggle &&
                (!IsLeftOpen || window.PanelOpen.IsVisible == false))
                command = PanelCommand.Show;
            else if (command == PanelCommand.Toggle)
                command = PanelCommand.Hide;


            if (command == PanelCommand.MaxToggle &&
                (window.grid.ColumnDefinitions[2].ActualWidth > 1 || window.grid.ColumnDefinitions[3].ActualWidth > 1 ))
                command = PanelCommand.Maximize;
            else if (command == PanelCommand.MaxToggle)
                command = PanelCommand.Normalize;

            switch (command)
            {
                case PanelCommand.Toggle:
                    break;
                case PanelCommand.MaxToggle:
                    break;
                case PanelCommand.Maximize:
                    MaximizeLeft();
                    MaximizePanelButtons();
                    break;
                case PanelCommand.Normalize:
                    if (NormalizeLeft())
                        NormalizePanelButtons();
                    break;
                case PanelCommand.Hide:
                    HideLeftPanel();
                    break;
                case PanelCommand.Show:
                    MakeVisiblePanelOpen();
                    ShowLeftPanel();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            CheckMainGrid();
        }


        public void CommandPanelPlaylist(PanelCommand? panelCommand)
        {
            PanelCommand command;
            if (panelCommand == null)
                command = PanelCommand.Toggle;
            else
                command = (PanelCommand)panelCommand;


            if (command == PanelCommand.Toggle &&
                (!IsRightOpen || window.PanelPlaylist.IsVisible == false))
                command = PanelCommand.Show;
            else if (command == PanelCommand.Toggle)
                command = PanelCommand.Hide;


            if (command == PanelCommand.MaxToggle &&
                (window.grid.ColumnDefinitions[2].ActualWidth > 1 || window.grid.ColumnDefinitions[3].ActualWidth > 1))
                command = PanelCommand.Maximize;
            else if (command == PanelCommand.MaxToggle)
                command = PanelCommand.Normalize;

            switch (command)
            {
                case PanelCommand.Toggle:
                    break;
                case PanelCommand.MaxToggle:
                    break;
                case PanelCommand.Maximize:
                    MaximizeRight();
                    MaximizePanelButtons();
                    break;
                case PanelCommand.Normalize:
                    if (NormalizeRight())
                        NormalizePanelButtons();
                    break;
                case PanelCommand.Hide:
                    HideRightPanel();
                    break;
                case PanelCommand.Show:
                    MakeVisiblePanelPlaylist();
                    ShowRightPanel();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            CheckMainGrid();
        }


        public void RememberThisPanelPosition(Point cursorPosition, Point cursorOnControl)
        {
            oldPositions.OriginalCursorPosition = cursorPosition;
            oldPositions.OriginalCursorPosition = cursorPosition;
            oldPositions.LeftPercentage = GetLeftPercentage();
            oldPositions.MidPercentage = GetMiddlePercentage();
            oldPositions.RightPercentage = GetRightPercentage();
        }


        public void PanelPanLeft(Point cursorPosition)
        {
            var leftWidth = cursorPosition.X - oldPositions.OriginalCursorPositionOnControl.X;

            if (oldPositions.RightPercentage > 0)
            {
                if (leftWidth < minLeftPanelWidth)
                {
                    window.grid.ColumnDefinitions[1].Width = new GridLength(0);
                    window.grid.ColumnDefinitions[0].Width = new GridLength(0, GridUnitType.Star);
                    window.grid.ColumnDefinitions[4].Width = new GridLength(oldPositions.RightPercentage * window.ActualWidth, GridUnitType.Star);
                    if (oldPositions.MidPercentage > 0)
                    {
                        window.grid.ColumnDefinitions[3].Width = new GridLength(resizerWidth);
                        window.grid.ColumnDefinitions[2].Width = new GridLength(window.ActualWidth - oldPositions.RightPercentage * window.ActualWidth, GridUnitType.Star);
                        NormalizePanelButtons();
                    }
                        
                }
                else if (window.ActualWidth - leftWidth < minRightPanelWidth)
                {
                    MaximizePanelButtons();
                    ShowOnlyLeft();
                }
                else if (window.ActualWidth - leftWidth <= oldPositions.RightPercentage * window.ActualWidth + resizerWidth * 2)
                {
                    window.grid.ColumnDefinitions[0].Width = new GridLength(leftWidth, GridUnitType.Star);
                    window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth);

                    window.grid.ColumnDefinitions[2].Width = new GridLength(0);
                    window.grid.ColumnDefinitions[3].Width = new GridLength(0);
                    window.grid.ColumnDefinitions[4].Width = new GridLength(window.ActualWidth - leftWidth - resizerWidth, GridUnitType.Star);
                    MaximizePanelButtons();
                }
                else
                {
                    if (oldPositions.MidPercentage > 0)
                    {
                        window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth);
                        window.grid.ColumnDefinitions[0].Width = new GridLength(leftWidth, GridUnitType.Star);

                        window.grid.ColumnDefinitions[2].Width = new GridLength(window.ActualWidth - leftWidth - window.ActualWidth * oldPositions.RightPercentage - resizerWidth * 2, GridUnitType.Star);

                        window.grid.ColumnDefinitions[3].Width = new GridLength(resizerWidth);
                        window.grid.ColumnDefinitions[4].Width = new GridLength(window.ActualWidth * oldPositions.RightPercentage, GridUnitType.Star);

                        NormalizePanelButtons();
                    }
                    else
                    {
                        window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth);
                        window.grid.ColumnDefinitions[0].Width = new GridLength(leftWidth, GridUnitType.Star);

                        window.grid.ColumnDefinitions[2].Width = new GridLength(0);

                        window.grid.ColumnDefinitions[3].Width = new GridLength(0);
                        window.grid.ColumnDefinitions[4].Width = new GridLength(window.ActualWidth - leftWidth - resizerWidth, GridUnitType.Star);
                    }
                }
                
            }
            else
            {
                if (leftWidth <= minLeftPanelWidth)
                {
                    ShowOnlyMiddle();
                }
                else if (leftWidth >= window.ActualWidth - 100)
                {
                    ShowOnlyLeft();
                }
                else
                {
                    window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth);
                    window.grid.ColumnDefinitions[0].Width = new GridLength(leftWidth, GridUnitType.Star);

                    window.grid.ColumnDefinitions[4].Width = new GridLength(0);

                    window.grid.ColumnDefinitions[3].Width = new GridLength(0);
                    window.grid.ColumnDefinitions[2].Width = new GridLength(window.ActualWidth - leftWidth - resizerWidth, GridUnitType.Star);
                }
            }
        }


        public void PanelPanRight(Point cursorPosition)
        {
            var rightWidth = window.ActualWidth - (cursorPosition.X + resizerWidth - oldPositions.OriginalCursorPositionOnControl.X);

            if (oldPositions.LeftPercentage > 0)
            {
                if (rightWidth < minRightPanelWidth)
                {
                    window.grid.ColumnDefinitions[3].Width = new GridLength(0);
                    window.grid.ColumnDefinitions[4].Width = new GridLength(0, GridUnitType.Star);
                    window.grid.ColumnDefinitions[0].Width = new GridLength(oldPositions.LeftPercentage * window.ActualWidth, GridUnitType.Star);
                    if (oldPositions.MidPercentage > 0)
                    {
                        window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth);
                        window.grid.ColumnDefinitions[2].Width = new GridLength(window.ActualWidth - oldPositions.RightPercentage * window.ActualWidth, GridUnitType.Star);
                        NormalizePanelButtons();
                    }

                }
                else if (window.ActualWidth - rightWidth < minLeftPanelWidth)
                {
                    MaximizePanelButtons();
                    ShowOnlyRight();
                }
                else if (window.ActualWidth - rightWidth <= oldPositions.LeftPercentage * window.ActualWidth + resizerWidth * 2)
                {
                    window.grid.ColumnDefinitions[4].Width = new GridLength(rightWidth, GridUnitType.Star);
                    window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth);

                    window.grid.ColumnDefinitions[2].Width = new GridLength(0);
                    window.grid.ColumnDefinitions[3].Width = new GridLength(0);
                    window.grid.ColumnDefinitions[0].Width = new GridLength(window.ActualWidth - rightWidth - resizerWidth, GridUnitType.Star);
                    MaximizePanelButtons();
                }
                else
                {
                    if (oldPositions.MidPercentage > 0)
                    {
                        window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth);
                        window.grid.ColumnDefinitions[4].Width = new GridLength(rightWidth, GridUnitType.Star);

                        window.grid.ColumnDefinitions[2].Width = new GridLength(window.ActualWidth - rightWidth - window.ActualWidth * oldPositions.LeftPercentage - resizerWidth * 2, GridUnitType.Star);

                        window.grid.ColumnDefinitions[3].Width = new GridLength(resizerWidth);
                        window.grid.ColumnDefinitions[0].Width = new GridLength(window.ActualWidth * oldPositions.LeftPercentage, GridUnitType.Star);

                        NormalizePanelButtons();
                    }
                    else
                    {
                        window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth);
                        window.grid.ColumnDefinitions[4].Width = new GridLength(rightWidth, GridUnitType.Star);

                        window.grid.ColumnDefinitions[2].Width = new GridLength(0);

                        window.grid.ColumnDefinitions[3].Width = new GridLength(0);
                        window.grid.ColumnDefinitions[0].Width = new GridLength(window.ActualWidth - rightWidth - resizerWidth, GridUnitType.Star);
                    }
                }

            }
            else
            {
                if (rightWidth <= minRightPanelWidth)
                {
                    ShowOnlyMiddle();
                }
                else if (rightWidth >= window.ActualWidth - 100)
                {
                    ShowOnlyRight();
                }
                else
                {
                    window.grid.ColumnDefinitions[3].Width = new GridLength(resizerWidth);
                    window.grid.ColumnDefinitions[4].Width = new GridLength(rightWidth, GridUnitType.Star);

                    window.grid.ColumnDefinitions[0].Width = new GridLength(0);

                    window.grid.ColumnDefinitions[1].Width = new GridLength(0);
                    window.grid.ColumnDefinitions[2].Width = new GridLength(window.ActualWidth - rightWidth - resizerWidth, GridUnitType.Star);
                }
            }
        }


        private void MakeVisiblePanelPlaylist()
        {
            window.PanelPlaylist.Visibility = Visibility.Visible;
        }


        public void HideRightPanel()
        {
            if (GetMiddlePercentage() <= 0)
            {
                var left = GetLeftPercentage();
                if (left > 0)
                {
                    if (window.ActualWidth > MaxDesiredLeftWidth)
                    {
                        var leftWidth = Math.Min(MaxDesiredLeftWidth, window.ActualWidth * 0.5);
                        window.grid.ColumnDefinitions[0].Width = new GridLength(leftWidth, GridUnitType.Star);
                        window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth, GridUnitType.Pixel);
                        window.grid.ColumnDefinitions[2].Width = new GridLength(window.ActualWidth - resizerWidth - leftWidth, GridUnitType.Star);
                        window.grid.ColumnDefinitions[3].Width = new GridLength(0, GridUnitType.Pixel);
                        window.grid.ColumnDefinitions[4].Width = new GridLength(0, GridUnitType.Star);    
                        NormalizePanelButtons();
                    }
                    else
                    {
                        ShowOnlyLeft();
                    }
                }
                else
                {
                    window.grid.ColumnDefinitions[3].Width = new GridLength(0, GridUnitType.Pixel);
                    window.grid.ColumnDefinitions[4].Width = new GridLength(0, GridUnitType.Star);  
                }
            }
            else
            {
                var left = GetLeftPercentage();
                if (left > 0)
                {
                    window.grid.ColumnDefinitions[0].Width = new GridLength(left * window.ActualWidth, GridUnitType.Star);
                    window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth, GridUnitType.Pixel); 
                }

                window.grid.ColumnDefinitions[2].Width = new GridLength((1 - left) * window.ActualWidth - resizerWidth, GridUnitType.Star);
                window.grid.ColumnDefinitions[3].Width = new GridLength(0, GridUnitType.Pixel);
                window.grid.ColumnDefinitions[4].Width = new GridLength(0, GridUnitType.Star);    
            }
            window.PanelPlaylist.Visibility = Visibility.Hidden;
        }


        private bool NormalizeRight()
        {
            if (IsLeftOpen)
            {
                if (window.ActualWidth <= desiredRightWidth + resizerWidth * 2 + desiredLeftWidth)
                {
                    return false;
                }
                else if (window.ActualWidth <= desiredRightWidth * 1.5 + resizerWidth * 2 + desiredLeftWidth * 1.5)
                {
                    window.grid.ColumnDefinitions[4].Width = new GridLength(desiredRightWidth, GridUnitType.Star);
                    window.grid.ColumnDefinitions[3].Width = new GridLength(resizerWidth);
                    window.grid.ColumnDefinitions[2].Width =
                        new GridLength(window.ActualWidth - desiredRightWidth - desiredLeftWidth - resizerWidth * 2,
                            GridUnitType.Star);
                    window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth);
                    window.grid.ColumnDefinitions[0].Width = new GridLength(desiredLeftWidth, GridUnitType.Star);
                }
                else
                {
                    SetNormalLayoutPanelsVisible();
                }
            }
            else
            {
                if (window.ActualWidth <= desiredRightWidth + resizerWidth)
                {
                    return false;
                }
                else if (window.ActualWidth <= minLeftPanelWidth + desiredRightWidth + resizerWidth)
                {
                    window.grid.ColumnDefinitions[4].Width = new GridLength(minLeftPanelWidth + (window.ActualWidth - minLeftPanelWidth - resizerWidth) * 0.3, GridUnitType.Star);
                    window.grid.ColumnDefinitions[3].Width = new GridLength(resizerWidth);
                    window.grid.ColumnDefinitions[2].Width = new GridLength((window.ActualWidth - minLeftPanelWidth - resizerWidth) * 0.7, GridUnitType.Star);
                }
                else if (window.ActualWidth <= desiredRightWidth * 5 / 2 + resizerWidth)
                {
                    window.grid.ColumnDefinitions[4].Width = new GridLength(2, GridUnitType.Star);
                    window.grid.ColumnDefinitions[3].Width = new GridLength(resizerWidth);
                    window.grid.ColumnDefinitions[2].Width = new GridLength(2, GridUnitType.Star);
                }
                else if (window.ActualWidth <= MaxDesiredRightWidth * 5 / 2 + resizerWidth)
                {
                    window.grid.ColumnDefinitions[4].Width = new GridLength(2, GridUnitType.Star);
                    window.grid.ColumnDefinitions[3].Width = new GridLength(resizerWidth);
                    window.grid.ColumnDefinitions[2].Width = new GridLength(3, GridUnitType.Star);
                }
                else
                {
                    window.grid.ColumnDefinitions[4].Width = new GridLength(MaxDesiredRightWidth, GridUnitType.Star);
                    window.grid.ColumnDefinitions[3].Width = new GridLength(resizerWidth);
                    window.grid.ColumnDefinitions[2].Width = new GridLength(window.ActualWidth - MaxDesiredRightWidth - resizerWidth, GridUnitType.Star);
                }
            }
            return true;
        }


        private void MaximizeRight()
        {
            if (IsLeftOpen)
            {
                double leftPercentage;

                leftPercentage = GetLeftPercentage();

                window.grid.ColumnDefinitions[4].Width = new GridLength((1 - leftPercentage) * window.ActualWidth - resizerWidth, GridUnitType.Star);
                window.grid.ColumnDefinitions[3].Width = new GridLength(0);
                window.grid.ColumnDefinitions[2].Width = new GridLength(0);
                window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth);
                window.grid.ColumnDefinitions[0].Width = new GridLength(leftPercentage * window.ActualWidth, GridUnitType.Star);
            }
            else
            {
                ShowOnlyRight();
            }
        }

        private void ShowLeftPanel()
        {
            if (IsRightOpen)
            {
                if (window.ActualWidth < desiredLeftWidth)
                {
                    ShowOnlyLeft();
                }
                else if (window.ActualWidth < 600)
                {
                    ShowOnlyBothPanelsMaxed();
                }
                else
                {
                    var right = GetRightPercentage();
                    var mid = GetMiddlePercentage();

                    if (mid <= 0)
                    {
                        ShowOnlyBothPanelsMaxed();
                    }
                    else
                    {
                        window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth, GridUnitType.Pixel);
                        window.grid.ColumnDefinitions[3].Width = new GridLength(resizerWidth, GridUnitType.Pixel);

                        if (window.ActualWidth * (1 - right) < desiredLeftWidth * 1.5 + resizerWidth * 2)
                        {
                            window.grid.ColumnDefinitions[4].Width = new GridLength(window.ActualWidth - resizerWidth - desiredLeftWidth, GridUnitType.Star);
                            window.grid.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Star);
                            window.grid.ColumnDefinitions[3].Width = new GridLength(0, GridUnitType.Pixel);
                            window.grid.ColumnDefinitions[0].Width = new GridLength(desiredLeftWidth, GridUnitType.Star);

                            MaximizePanelButtons();
                        }
                        else if (window.ActualWidth * (1 - right) > desiredLeftWidth * 2 + resizerWidth * 2)
                        {
                            window.grid.ColumnDefinitions[4].Width = new GridLength(right * window.ActualWidth, GridUnitType.Star);
                            window.grid.ColumnDefinitions[2].Width = new GridLength((1 - right) * window.ActualWidth * 0.6 - resizerWidth * 2, GridUnitType.Star);
                            window.grid.ColumnDefinitions[0].Width = new GridLength((1 - right) * window.ActualWidth * 0.4, GridUnitType.Star);
                        }
                        else
                        {
                            window.grid.ColumnDefinitions[4].Width = new GridLength(right * window.ActualWidth, GridUnitType.Star);
                            window.grid.ColumnDefinitions[2].Width = new GridLength((1 - right) * window.ActualWidth - desiredLeftWidth - resizerWidth * 2, GridUnitType.Star);
                            window.grid.ColumnDefinitions[0].Width = new GridLength(desiredLeftWidth, GridUnitType.Star);
                        }
                    }

                    
                }
            }
            else
            {
                if (window.ActualWidth < desiredLeftWidth)
                {
                    ShowOnlyLeft();
                }
                else if (window.ActualWidth < 600)
                {
                    ShowLeftAndMiddleEven();
                }
                else
                {
                    window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth, GridUnitType.Pixel);

                    var w = window.grid.ColumnDefinitions[2].Width.Value + window.grid.ColumnDefinitions[0].Width.Value;
                    window.grid.ColumnDefinitions[0].Width = new GridLength(w * 3 / 7, GridUnitType.Star);
                }
            }
        }

        private void ShowRightPanel()
        {
            if (IsLeftOpen)
            {
                if (window.ActualWidth < desiredRightWidth)
                {
                    ShowOnlyRight();
                }
                else if (window.ActualWidth < 600)
                {
                    ShowOnlyBothPanelsMaxed();
                }
                else
                {
                    var left = GetLeftPercentage();
                    var mid = GetMiddlePercentage();

                    if (mid <= 0)
                    {
                        ShowOnlyBothPanelsMaxed();
                    }
                    else
                    {
                        window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth, GridUnitType.Pixel);
                        window.grid.ColumnDefinitions[3].Width = new GridLength(resizerWidth, GridUnitType.Pixel);

                        if (window.ActualWidth * (1 - left) < desiredRightWidth * 1.5 + resizerWidth * 2)
                        {
                            window.grid.ColumnDefinitions[0].Width = new GridLength(window.ActualWidth - resizerWidth - desiredRightWidth, GridUnitType.Star);
                            window.grid.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Star);
                            window.grid.ColumnDefinitions[3].Width = new GridLength(0, GridUnitType.Pixel);
                            window.grid.ColumnDefinitions[4].Width = new GridLength(desiredRightWidth, GridUnitType.Star);

                            MaximizePanelButtons();
                        }
                        else if (window.ActualWidth * (1 - left) > desiredRightWidth * 2 + resizerWidth * 2)
                        {
                            window.grid.ColumnDefinitions[0].Width = new GridLength(left * window.ActualWidth, GridUnitType.Star);
                            window.grid.ColumnDefinitions[2].Width = new GridLength((1 - left) * window.ActualWidth * 0.6 - resizerWidth * 2, GridUnitType.Star);
                            window.grid.ColumnDefinitions[4].Width = new GridLength((1 - left) * window.ActualWidth * 0.4, GridUnitType.Star);
                        }
                        else
                        {
                            window.grid.ColumnDefinitions[0].Width = new GridLength(left * window.ActualWidth, GridUnitType.Star);
                            window.grid.ColumnDefinitions[2].Width = new GridLength((1 - left) * window.ActualWidth - desiredLeftWidth - resizerWidth * 2, GridUnitType.Star);
                            window.grid.ColumnDefinitions[4].Width = new GridLength(desiredLeftWidth, GridUnitType.Star);
                        }
                    }
                }
            }
            else
            {
                if (window.ActualWidth < desiredRightWidth)
                {
                    ShowOnlyRight();
                }
                else if (window.ActualWidth < 600)
                {
                    ShowRightAndMiddleEven();
                }
                else
                {
                    window.grid.ColumnDefinitions[3].Width = new GridLength(resizerWidth, GridUnitType.Pixel);

                    var w = window.grid.ColumnDefinitions[2].Width.Value + window.grid.ColumnDefinitions[0].Width.Value;
                    window.grid.ColumnDefinitions[4].Width = new GridLength(w * 3 / 7, GridUnitType.Star);
                }
            }
        }


        private void ShowLeftAndMiddleEven()
        {
            window.grid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
            window.grid.ColumnDefinitions[3].Width = new GridLength(0);
            window.grid.ColumnDefinitions[4].Width = new GridLength(0);

            window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth);
            window.grid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
        }

        private void ShowRightAndMiddleEven()
        {
            window.grid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
            window.grid.ColumnDefinitions[3].Width = new GridLength(0);
            window.grid.ColumnDefinitions[0].Width = new GridLength(0);

            window.grid.ColumnDefinitions[3].Width = new GridLength(resizerWidth);
            window.grid.ColumnDefinitions[4].Width = new GridLength(1, GridUnitType.Star);
        }


        private void ShowOnlyBothPanelsMaxed()
        {
            window.grid.ColumnDefinitions[2].Width = new GridLength(0);
            window.grid.ColumnDefinitions[3].Width = new GridLength(0);
            window.grid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);

            window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth);
            window.grid.ColumnDefinitions[4].Width = new GridLength(1, GridUnitType.Star);
            MaximizePanelButtons();
        }


        private void ShowOnlyRight()
        {
            window.grid.ColumnDefinitions[2].Width = new GridLength(0);
            window.grid.ColumnDefinitions[1].Width = new GridLength(0);
            window.grid.ColumnDefinitions[0].Width = new GridLength(0);

            window.grid.ColumnDefinitions[3].Width = new GridLength(0);
            window.grid.ColumnDefinitions[4].Width = new GridLength(1, GridUnitType.Star);
        }

        private void ShowOnlyMiddle()
        {
            window.grid.ColumnDefinitions[4].Width = new GridLength(0);
            window.grid.ColumnDefinitions[1].Width = new GridLength(0);
            window.grid.ColumnDefinitions[0].Width = new GridLength(0);

            window.grid.ColumnDefinitions[3].Width = new GridLength(0);
            window.grid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
        }

        private void ShowOnlyLeft()
        {
            window.grid.ColumnDefinitions[2].Width = new GridLength(0);
            window.grid.ColumnDefinitions[3].Width = new GridLength(0);
            window.grid.ColumnDefinitions[4].Width = new GridLength(0);

            window.grid.ColumnDefinitions[1].Width = new GridLength(0);
            window.grid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
        }

        private void NormalizePanelButtons()
        {
            window.PanelOpen.ButtonMaximizePanel.CurrentState = 0;
            window.PanelPlaylist.ButtonMaximizePanel.CurrentState = 0;
        }


        private void MaximizePanelButtons()
        {
            window.PanelOpen.ButtonMaximizePanel.CurrentState = 1;
            window.PanelPlaylist.ButtonMaximizePanel.CurrentState = 1;
        }

        private bool NormalizeLeft()
        {
            if (IsRightOpen)
            {
                if (window.ActualWidth <= desiredLeftWidth + resizerWidth * 2 + desiredRightWidth)
                {
                    return false;
                }
                else if (window.ActualWidth <= desiredLeftWidth * 1.5 + resizerWidth * 2 + desiredRightWidth * 1.5)
                {
                    window.grid.ColumnDefinitions[0].Width = new GridLength(desiredLeftWidth, GridUnitType.Star);
                    window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth);
                    window.grid.ColumnDefinitions[2].Width =
                        new GridLength(window.ActualWidth - desiredLeftWidth - desiredRightWidth - resizerWidth * 2,
                            GridUnitType.Star);
                    window.grid.ColumnDefinitions[3].Width = new GridLength(resizerWidth);
                    window.grid.ColumnDefinitions[4].Width = new GridLength(desiredRightWidth, GridUnitType.Star);
                }
                else
                {
                    SetNormalLayoutPanelsVisible();
                }
                

                
            }
            else
            {
                if (window.ActualWidth <= desiredLeftWidth + resizerWidth)
                {
                    return false;
                }
                else if (window.ActualWidth <= minLeftPanelWidth + desiredLeftWidth + resizerWidth)
                {
                    window.grid.ColumnDefinitions[0].Width = new GridLength(minLeftPanelWidth + (window.ActualWidth - minLeftPanelWidth - resizerWidth) * 0.3, GridUnitType.Star);
                    window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth);
                    window.grid.ColumnDefinitions[2].Width = new GridLength((window.ActualWidth - minLeftPanelWidth - resizerWidth) * 0.7, GridUnitType.Star);
                }
                else if (window.ActualWidth <= desiredLeftWidth * 5 / 2 + resizerWidth)
                {
                    window.grid.ColumnDefinitions[0].Width = new GridLength(2, GridUnitType.Star);
                    window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth);
                    window.grid.ColumnDefinitions[2].Width = new GridLength(2, GridUnitType.Star);
                }
                else if (window.ActualWidth <= MaxDesiredLeftWidth * 5 / 2 + resizerWidth)
                {
                    window.grid.ColumnDefinitions[0].Width = new GridLength(2, GridUnitType.Star);
                    window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth);
                    window.grid.ColumnDefinitions[2].Width = new GridLength(3, GridUnitType.Star);
                }
                else
                {
                    window.grid.ColumnDefinitions[0].Width = new GridLength(MaxDesiredLeftWidth, GridUnitType.Star);
                    window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth);
                    window.grid.ColumnDefinitions[2].Width = new GridLength(window.ActualWidth - MaxDesiredLeftWidth - resizerWidth, GridUnitType.Star);
                }
            }
            return true;
        }


        private void SetNormalLayoutPanelsVisible()
        {
            window.grid.ColumnDefinitions[0].Width = new GridLength(2, GridUnitType.Star);
            window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth);
            window.grid.ColumnDefinitions[2].Width = new GridLength(3, GridUnitType.Star);
            window.grid.ColumnDefinitions[3].Width = new GridLength(resizerWidth);
            window.grid.ColumnDefinitions[4].Width = new GridLength(2, GridUnitType.Star);
        }


        private void MaximizeLeft()
        {
            if (IsRightOpen)
            {
                double rightPercentage;

                rightPercentage = GetRightPercentage();

                window.grid.ColumnDefinitions[0].Width = new GridLength((1 - rightPercentage) * window.ActualWidth - resizerWidth, GridUnitType.Star);
                window.grid.ColumnDefinitions[1].Width = new GridLength(resizerWidth);
                window.grid.ColumnDefinitions[2].Width = new GridLength(0);
                window.grid.ColumnDefinitions[3].Width = new GridLength(0);    
                window.grid.ColumnDefinitions[4].Width = new GridLength(rightPercentage * window.ActualWidth, GridUnitType.Star);
            }
            else
            {
                ShowOnlyLeft();
            }

            
        }

        private double GetPercentage(ColumnDefinition current, double actualWidth, ColumnDefinitionCollection columnDefinitions)
        {
            double percentage;
            if (current.Width.IsAbsolute)
                percentage = current.Width.Value / actualWidth;
            else
            {
                double percentageWidthPixels;
                double totalPercentage = current.Width.Value;
                percentageWidthPixels = actualWidth;

                foreach (var column in columnDefinitions)
                {
                    if (column == current) continue;

                    if (column.Width.IsAbsolute)
                        percentageWidthPixels -= column.Width.Value;
                    else
                        totalPercentage += column.Width.Value;
                }

                percentage = percentageWidthPixels / actualWidth *
                                  current.Width.Value / totalPercentage;
            }
            return percentage;
        }

        private double GetRightPercentage()
        {
            return GetPercentage(window.grid.ColumnDefinitions[4], window.ActualWidth, window.grid.ColumnDefinitions);
        }

        

        private double GetLeftPercentage()
        {
            return GetPercentage(window.grid.ColumnDefinitions[0], window.ActualWidth, window.grid.ColumnDefinitions);
        }


        private double GetMiddlePercentage()
        {
            return GetPercentage(window.grid.ColumnDefinitions[2], window.ActualWidth, window.grid.ColumnDefinitions);
        }


        public void HideLeftPanel()
        {
            if (GetMiddlePercentage() <= 0)
            {
                var right = GetRightPercentage();
                if (right > 0)
                {
                    if (window.ActualWidth > MaxDesiredLeftWidth)
                    {
                        var rightWidth = Math.Min(MaxDesiredRightWidth, window.ActualWidth * 0.5);
                        window.grid.ColumnDefinitions[4].Width = new GridLength(rightWidth, GridUnitType.Star);
                        window.grid.ColumnDefinitions[3].Width = new GridLength(resizerWidth, GridUnitType.Pixel);
                        window.grid.ColumnDefinitions[2].Width = new GridLength(window.ActualWidth - resizerWidth - rightWidth, GridUnitType.Star);
                        window.grid.ColumnDefinitions[1].Width = new GridLength(0);
                        window.grid.ColumnDefinitions[0].Width = new GridLength(0);
                        NormalizePanelButtons();
                    }
                    else
                    {
                        ShowOnlyRight();
                    }
                }
                else
                {
                    window.grid.ColumnDefinitions[0].Width = new GridLength(0);
                    window.grid.ColumnDefinitions[1].Width = new GridLength(0);
                }
            }
            else
            {
                var right = GetRightPercentage();
                if (right > 0)
                {
                    window.grid.ColumnDefinitions[4].Width = new GridLength(right * window.ActualWidth, GridUnitType.Star);
                    window.grid.ColumnDefinitions[3].Width = new GridLength(resizerWidth, GridUnitType.Pixel);
                }

                window.grid.ColumnDefinitions[2].Width = new GridLength((1 - right) * window.ActualWidth - resizerWidth, GridUnitType.Star);
                window.grid.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Pixel);
                window.grid.ColumnDefinitions[0].Width = new GridLength(0, GridUnitType.Star);
            }
            window.PanelOpen.Visibility = Visibility.Hidden;
        }


        


        /// <summary>
        /// Ensures that grid doesn't break down with all columns being 0
        /// </summary>
        public void CheckMainGrid()
        {
            if (window.grid.ColumnDefinitions[0].Width.Value <= 0 && window.grid.ColumnDefinitions[4].Width.Value <= 0)
            {
                window.grid.ColumnDefinitions[2].Width = new GridLength(3, GridUnitType.Star);
            }

            if (!IsLeftOpen && !IsRightOpen)
                NormalizePanelButtons();

            //if (window.grid.ColumnDefinitions[0].ActualWidth < minLeftPanelWidth && window.grid.ColumnDefinitions[0].ActualWidth > 0 && (window.grid.ColumnDefinitions[0].Width.Value > 0))
            //{
            //    window.grid.ColumnDefinitions[0].Width = new GridLength(0, GridUnitType.Star);
            //    //window.grid.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Star);
            //}
            //if (window.grid.ColumnDefinitions[4].ActualWidth < minRightPanelWidth && window.grid.ColumnDefinitions[4].ActualWidth > 0 && (window.grid.ColumnDefinitions[4].Width.Value > 0))
            //{
            //    window.grid.ColumnDefinitions[3].Width = new GridLength(0, GridUnitType.Star);
            //    //window.grid.ColumnDefinitions[4].Width = new GridLength(0, GridUnitType.Star);
            //}
        }


        public void Update()
        {
            
        }


        public void CheckPanelHide(Point cursorPosition)
        {
            if (window.panelLeft.Width.Value <= 0 && window.panelRight.Width.Value <= 0 &&
                (window.ExtWindowState == ExtWindowState.Maximized || window.ExtWindowState == ExtWindowState.Fullscreen) &&
                 ((cursorPosition.Y > window.PanelHigh.Height.Value &&
                  cursorPosition.Y < window.ActualHeight - window.PanelHigh.Height.Value - 1 &&
                  (cursorPosition.X < 2 || cursorPosition.X > window.ActualWidth - 2)) ||
                  (cursorPosition.Y < 0 ||
                  cursorPosition.Y > window.ActualHeight)))
            {
                HideBottomAndTop();
            }
            else
            {
                ShowBottomAndTop();
            }
        }


        public void HideBottomAndTop()
        {
            window.PanelHigh.Height = new GridLength(0, GridUnitType.Pixel);
            window.PanelLow.Height = new GridLength(0, GridUnitType.Pixel);
            Mouse.OverrideCursor = Cursors.None;
            window.LabelEvent.Visibility = Visibility.Visible;
        }


        public void ShowBottomAndTop()
        {
            window.PanelHigh.Height = new GridLength(PanelHeight, GridUnitType.Pixel);
            window.PanelLow.Height = new GridLength(PanelHeight, GridUnitType.Pixel);
            Mouse.OverrideCursor = null;
            window.LabelEvent.Visibility = Visibility.Hidden;
        }


        public void CheckResize()
        {
            ResizeTitleBar();
            CheckPanelsOnResize();
        }


        private void CheckPanelsOnResize()
        {
            if (window.ActualWidth <= minLeftPanelWidth + minRightPanelWidth + resizerWidth)
            {
                if (IsRightOpen && IsLeftOpen)
                {
                    ShowOnlyRight();
                }
                else if (IsLeftOpen)
                {
                    ShowOnlyLeft();
                }
            }
            else if (window.ActualWidth <= desiredLeftWidth + desiredRightWidth + resizerWidth && IsRightOpen &&
                     IsLeftOpen)
            {
                ShowOnlyBothPanelsMaxed();
            }
            else if (IsLeftOpen && GetLeftPercentage() * window.ActualWidth < minLeftPanelWidth)
            {
                NormalizeLeft();
            }
            else if (IsRightOpen && GetRightPercentage() * window.ActualWidth < minLeftPanelWidth)
            {
                NormalizeRight();
            }
            
        }


        private void ResizeTitleBar()
        {
            if (window.ActualWidth < 640)
            {
                window.LabelTopic.Margin = new Thickness(0, 15, 0, -5);
                window.LabelTopic.FontSize = 14;
                window.ButtonExit.Margin = new Thickness(0, 0, 0, 20);
                window.ButtonMax.Margin = new Thickness(0, 0, 40, 20);
                window.ButtonMin.Margin = new Thickness(0, 0, 80, 20);

                window.ButtonPlayList.Margin = new Thickness(0, 0, 120, 20);
                window.ButtonOpen.Margin = new Thickness(0, 0, 160, 20);

                window.BarTop.Margin = new Thickness(0, 0, 200, 20);
                window.BarTop2.Margin = new Thickness(0, 20, 0, 0);
                window.BarTop2.Visibility = Visibility.Visible;
            }
            else
            {
                window.LabelTopic.Margin = new Thickness(0, 0, 240, 5);
                window.LabelTopic.FontSize = 18;

                window.ButtonExit.Margin = new Thickness(0, 0, 0, 0);
                window.ButtonMax.Margin = new Thickness(0, 0, 40, 0);
                window.ButtonMin.Margin = new Thickness(0, 0, 80, 0);

                window.ButtonPlayList.Margin = new Thickness(0, 0, 120, 0);
                window.ButtonOpen.Margin = new Thickness(0, 0, 160, 0);

                window.BarTop.Margin = new Thickness(0, 0, 200, 0);

                window.BarTop2.Visibility = Visibility.Hidden;
            }
        }
    }
}
