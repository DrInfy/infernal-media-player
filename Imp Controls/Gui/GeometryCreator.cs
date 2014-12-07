using System;
using System.Windows;
using System.Windows.Media;

namespace ImpControls.Gui
{
    public enum BtnNumber
    {
        Play,
        Mute,
        Open,
        Help,
        Settings,
        Playlist,
        Minimize,
        Maximize,
        Exit_,
        ClearList,

        Loop,

        AddFolder,
        AddSubFolder,

        Refresh,

        Remove,
        Save,
        AddFile
    }


    static class GeometryCreator
    {
        public static Geometry GetGeometry(BtnNumber btn, int state)
        {
            PathFigureCollection pFC = new PathFigureCollection();
            PathGeometry pG = new PathGeometry();

            switch (btn)
            {
                case BtnNumber.Play:
                    if (state == 0)
                    {
                        return PlayGeo(pFC, pG);
                    }
                    return PauseGeo(pFC, pG);
                case BtnNumber.Loop:
                    if (state == 0)
                    {
                        return NoLoop(pFC, pG);
                    }
                    if (state == 1)
                    {
                        return LoopAll(pFC, pG);
                    }
                    return LoopOne(pFC, pG);
                case BtnNumber.Mute:
                    if (state == 1)
                    {
                        return MuteGeo(pFC, pG);
                    }

                    return UnmuteGeo(pFC, pG);
                    
                case BtnNumber.Open:
                    return OpenGeo(pFC, pG);
                case BtnNumber.Help:
                    return HelpGeo(pFC, pG);
                case BtnNumber.Settings:
                    return SettingsGeo(pFC, pG);
                case BtnNumber.Playlist:
                    return PlaylistGeo(pFC, pG);
                case BtnNumber.Save:
                    return SavesGeo(pFC, pG);
                case BtnNumber.Minimize:
                    return MinimizeGeo(pFC, pG);
                case BtnNumber.Maximize:
                    if (state == 0)
                    {
                        return MaximizeGeo(pFC, pG);
                    }
                    if (state == 1)
                    {
                        return NormalizeGeo(pFC, pG);
                    }

                    break;
                case BtnNumber.Exit_:
                    return ExitGeo(pFC, pG);
                case BtnNumber.ClearList:
                    return ClearGeo(pFC, pG);
                case BtnNumber.AddFile:
                    return AddFile(pFC, pG);
                case BtnNumber.AddFolder:
                    return AddFolder(pFC, pG);
                case BtnNumber.AddSubFolder:
                    return AddSubFolder(pFC, pG);
                case BtnNumber.Refresh:
                    return Refresh(pFC, pG);
                case BtnNumber.Remove:
                    return Remove(pFC, pG);
                default:

                    throw new Exception("Unidentified button");
            }
            return null;
        }


        private static Geometry AddFile(PathFigureCollection pFC, PathGeometry pG)
        {
            // sets the arrow position
            var y = -5;

            // file left top triangle
            AddLine(new Point(15, 15), new Point(0, 30), pFC);
            AddLine(new Point(15, 15), new Point(15, 30), pFC);
            AddLine(new Point(0, 30), new Point(15, 30), pFC);

            // file basic rectangle shaper
            AddLine(new Point(15, 15), new Point(60, 15), pFC);
            AddLine(new Point(60, 15), new Point(60, 50 + y), pFC);
            AddLine(new Point(60, 65 + y), new Point(60, 85), pFC);
            AddLine(new Point(60, 85), new Point(0, 85), pFC);
            AddLine(new Point(0, 85), new Point(0, 30), pFC);

            // a couple text lines representing content
            AddLine(new Point(10, 35), new Point(50, 35), pFC);
            AddLine(new Point(10, 50), new Point(50, 50), pFC);
            AddLine(new Point(10, 65), new Point(50, 65), pFC);


            
            

            // arrow
            AddLine(new Point(55, 50 + y), new Point(85, 50 + y), pFC);
            AddLine(new Point(85, 50 + y), new Point(85, 40 + y), pFC);
            AddLine(new Point(85, 40 + y), new Point(100, 57.5f + y), pFC);
            AddLine(new Point(100, 57.5f + y), new Point(85, 75 + y), pFC);
            AddLine(new Point(85, 75 + y), new Point(85, 65 + y), pFC);
            AddLine(new Point(85, 65 + y), new Point(55, 65 + y), pFC);
            AddLine(new Point(55, 65 + y), new Point(55, 50 + y), pFC);
            pG.Figures = pFC;
            return pG;
        }


        private static void AddLine(Point p1, Point p2, PathFigureCollection pathF)
        {
            PathFigure polkuFigure = new PathFigure();
            LineSegment myLineSegment = new LineSegment();
            PathSegmentCollection pathS = new PathSegmentCollection();
            polkuFigure.StartPoint = p1;
            myLineSegment.Point = p2;
            pathS.Add(myLineSegment);

            polkuFigure.Segments = pathS;
            pathF.Add(polkuFigure);
        }

        private static void AddRect(Rect box, PathGeometry geom)
        {
            RectangleGeometry loota = new RectangleGeometry();
            loota.Rect = box;
            geom.AddGeometry(loota);
        }


        private static void AddCircle(Rect box, PathGeometry geom)
        {
            EllipseGeometry pallo = new EllipseGeometry(box);
            geom.AddGeometry(pallo);
        }

        private static Geometry SavesGeo(PathFigureCollection pFC, PathGeometry pG)
        {
            AddLine(new Point(0, 2), new Point(2, 0), pFC);
            AddLine(new Point(2, 0), new Point(95, 0), pFC);

            AddLine(new Point(95, 0), new Point(100, 5), pFC);
            AddLine(new Point(100, 5), new Point(100, 98), pFC);

            AddLine(new Point(100, 98), new Point(98, 100), pFC);
            AddLine(new Point(98, 100), new Point(2, 100), pFC);

            AddLine(new Point(2, 100), new Point(0, 98), pFC);
            AddLine(new Point(0, 98), new Point(0, 2), pFC);

            AddLine(new Point(20, 0), new Point(20, 35), pFC);
            AddLine(new Point(20, 35), new Point(80, 35), pFC);
            AddLine(new Point(80, 35), new Point(80, 0), pFC);

            pG.Figures = pFC;

            AddLine(new Point(60, 0), new Point(60, 25), pFC);
            AddLine(new Point(60, 25), new Point(70, 25), pFC);
            AddLine(new Point(70, 25), new Point(70, 0), pFC);

            //AddRect(new Rect(55,5,10,20), pG);
            return pG;
        }

        private static PathGeometry PlayGeo(PathFigureCollection pFC, PathGeometry pG)
        {
            AddLine(new Point(0, 0), new Point(100, 50), pFC);
            AddLine(new Point(0, 0), new Point(0, 100), pFC);
            AddLine(new Point(0, 100), new Point(100, 50), pFC);

            pG.Figures = pFC;
            return pG;
        }

        private static PathGeometry PauseGeo(PathFigureCollection pFC, PathGeometry pG)
        {
            pG = new PathGeometry();
            AddRect(new Rect(0, 0, 35, 100), pG);
            AddRect(new Rect(65, 0, 35, 100), pG);
            return pG;
        }

        private static PathGeometry MuteGeo(PathFigureCollection pFC, PathGeometry pG)
        {
            MuteLines(pFC);

            AddLine(new Point(65, 25), new Point(90, 0), pFC);
            AddLine(new Point(70, 50), new Point(100, 50), pFC);
            AddLine(new Point(65, 75), new Point(90, 100), pFC);

            pG.Figures = pFC;
            return pG;
        }

        private static PathGeometry UnmuteGeo(PathFigureCollection pFC, PathGeometry pG)
        {
            MuteLines(pFC);

            pG.Figures = pFC;
            return pG;
        }
        private static PathFigureCollection MuteLines(PathFigureCollection pFC)
        {
            AddLine(new Point(0, 50), new Point(40, 0), pFC);
            AddLine(new Point(0, 50), new Point(40, 100), pFC);

            AddLine(new Point(40, 0), new Point(30, 50), pFC);
            AddLine(new Point(40, 0), new Point(50, 50), pFC);

            AddLine(new Point(40, 100), new Point(30, 50), pFC);
            AddLine(new Point(40, 100), new Point(50, 50), pFC);
            return pFC;
        }

        

        private static PathGeometry OpenGeo(PathFigureCollection pFC, PathGeometry pG)
        {
            AddLine(new Point(50, 0), new Point(0, 50), pFC);
            AddLine(new Point(50, 0), new Point(100, 50), pFC);
            AddLine(new Point(0, 50), new Point(100, 50), pFC);

            AddLine(new Point(0, 100), new Point(100, 100), pFC);

            pG.Figures = pFC;
            return pG;
        }

        private static PathGeometry HelpGeo(PathFigureCollection pFC, PathGeometry pG)
        {
            AddLine(new Point(20, 40), new Point(10, 20), pFC);
            AddLine(new Point(10, 20), new Point(20, 10), pFC);

            AddLine(new Point(20, 10), new Point(50, 0), pFC);
            AddLine(new Point(50, 0), new Point(75, 5), pFC);
            AddLine(new Point(75, 5), new Point(85, 25), pFC);
            AddLine(new Point(85, 25), new Point(55, 40), pFC);

            AddLine(new Point(55, 40), new Point(50, 60), pFC);

            AddLine(new Point(50, 70), new Point(35, 85), pFC);
            AddLine(new Point(50, 70), new Point(65, 85), pFC);
            AddLine(new Point(50, 100), new Point(35, 85), pFC);

            AddLine(new Point(50, 100), new Point(65, 85), pFC);


            pG.Figures = pFC;
            return pG;
        }

        private static PathGeometry SettingsGeo(PathFigureCollection pFC, PathGeometry pG)
        {

            AddLine(new Point(0, 40), new Point(40, 0), pFC);
            AddLine(new Point(0, 40), new Point(15, 55), pFC);
            AddLine(new Point(40, 0), new Point(55, 15), pFC);

            AddLine(new Point(15, 55), new Point(30, 40), pFC);
            AddLine(new Point(55, 15), new Point(40, 30), pFC);

            AddLine(new Point(30, 40), new Point(80, 100), pFC);
            AddLine(new Point(40, 30), new Point(100, 80), pFC);
            AddLine(new Point(80, 100), new Point(100, 80), pFC);

            pG.Figures = pFC;
            return pG;
        }

        private static PathGeometry PlaylistGeo(PathFigureCollection pFC, PathGeometry pG)
        {
            AddLine(new Point(0, 0), new Point(100, 0), pFC);
            AddLine(new Point(0, 33), new Point(100, 33), pFC);
            AddLine(new Point(0, 67), new Point(100, 67), pFC);

            AddLine(new Point(0, 100), new Point(100, 100), pFC);


            pG.Figures = pFC;
            return pG;
        }

        private static PathGeometry MinimizeGeo(PathFigureCollection pFC, PathGeometry pG)
        {
            AddLine(new Point(0, 100), new Point(100, 100), pFC);

            pG.Figures = pFC;
            return pG;
        }
        private static PathGeometry NormalizeGeo(PathFigureCollection pFC, PathGeometry pG)
        {
            AddLine(new Point(20, 0), new Point(20, 20), pFC);
            AddLine(new Point(20, 0), new Point(100, 0), pFC);
            AddLine(new Point(100, 0), new Point(100, 80), pFC);
            AddLine(new Point(100, 80), new Point(80, 80), pFC);
            pG.Figures = pFC;
            AddRect(new Rect(0, 20, 80, 80), pG);
            return pG;
        }
        private static PathGeometry MaximizeGeo(PathFigureCollection pFC, PathGeometry pG)
        {
            AddRect(new Rect(0, 0, 100, 20), pG);
            AddRect(new Rect(0, 0, 100, 100), pG);
            return pG;
        }

        private static PathGeometry ExitGeo(PathFigureCollection pFC, PathGeometry pG)
        {

            AddLine(new Point(0, 0), new Point(100, 100), pFC);
            AddLine(new Point(100, 0), new Point(0, 100), pFC);

            pG.Figures = pFC;
            return pG;
        }

        private static PathGeometry ClearGeo(PathFigureCollection pFC, PathGeometry pG)
        {
            AddLine(new Point(0, 10), new Point(100, 10), pFC);
            AddLine(new Point(0, 35), new Point(100, 35), pFC);
            AddLine(new Point(0, 60), new Point(100, 60), pFC);
            AddLine(new Point(0, 85), new Point(100, 85), pFC);

            AddLine(new Point(10, 0), new Point(90, 100), pFC);
            AddLine(new Point(90, 0), new Point(10, 100), pFC);

            pG.Figures = pFC;
            return pG;
        }


        private static Geometry NoLoop(PathFigureCollection pFC, PathGeometry pG)
        {
            //AddLine(New Point(0, 50), New Point(25, 50), pFC)
            //'AddLine(New Point(40, 50), New Point(60, 50), pFC)

            //AddLine(New Point(75, 50), New Point(100, 50), pFC)

            //AddLine(New Point(35, 30), New Point(55, 50), pFC)
            //AddLine(New Point(35, 70), New Point(55, 50), pFC)

            //AddLine(New Point(100, 25), New Point(100, 75), pFC)
            //'AddLine(New Point(65, 70), New Point(85, 50), pFC)
            //AddLine(New Point(0, 45), New Point(100, 45), pFC)
            AddLine(new Point(0, 50), new Point(100, 50), pFC);
            //AddLine(New Point(0, 55), New Point(100, 55), pFC)

            pG.Figures = pFC;
            return pG;
        }

        private static Geometry LoopAll(PathFigureCollection pFC, PathGeometry pG)
        {
            AddLine(new Point(25, 30), new Point(75, 30), pFC);
            AddLine(new Point(25, 70), new Point(75, 70), pFC);

            AddLine(new Point(0, 50), new Point(25, 30), pFC);
            AddLine(new Point(0, 50), new Point(25, 70), pFC);

            AddLine(new Point(100, 50), new Point(75, 30), pFC);
            AddLine(new Point(100, 50), new Point(75, 70), pFC);

            AddLine(new Point(30, 10), new Point(50, 30), pFC);
            AddLine(new Point(30, 50), new Point(50, 30), pFC);

            AddLine(new Point(70, 50), new Point(50, 70), pFC);
            AddLine(new Point(70, 90), new Point(50, 70), pFC);

            pG.Figures = pFC;
            return pG;
        }

        private static Geometry LoopOne(PathFigureCollection pFC, PathGeometry pG)
        {
            AddLine(new Point(0, 50), new Point(15, 50), pFC);
            AddLine(new Point(40, 50), new Point(60, 50), pFC);
            AddLine(new Point(85, 50), new Point(100, 50), pFC);

            AddLine(new Point(25, 5), new Point(50, 30), pFC);
            //AddLine(New Point(40, 60), New Point(55, 30), pFC)

            AddLine(new Point(75, 95), new Point(50, 70), pFC);
            pG.Figures = pFC;
            AddCircle(new Rect(25, 25, 50, 50), pG);

            pG.Figures = pFC;
            return pG;
        }

        private static Geometry AddFolder(PathFigureCollection pFC, PathGeometry pG)
        {

            AddLine(new Point(0, 0), new Point(0, 85), pFC);
            AddLine(new Point(0, 85), new Point(30, 100), pFC);

            AddLine(new Point(0, 0), new Point(70, 0), pFC);
            AddLine(new Point(70, 0), new Point(70, 40), pFC);
            AddLine(new Point(70, 60), new Point(70, 85), pFC);

            AddLine(new Point(30, 15), new Point(30, 100), pFC);
            AddLine(new Point(0, 0), new Point(30, 15), pFC);

            AddLine(new Point(70, 85), new Point(30, 85), pFC);

            //AddLine(new Point(45, 50), new Point(100, 50), pFC);
            //AddLine(new Point(45, 45), new Point(92, 45), pFC);
            //AddLine(new Point(45, 55), new Point(92, 55), pFC);

            // arrow
            AddLine(new Point(45, 60), new Point(45, 40), pFC);
            AddLine(new Point(45, 60), new Point(85, 60), pFC);
            AddLine(new Point(45, 40), new Point(85, 40), pFC);
            AddLine(new Point(85, 60), new Point(85, 70), pFC);
            AddLine(new Point(85, 40), new Point(85, 30), pFC);
            // arrow head
            AddLine(new Point(85, 70), new Point(100, 50), pFC);
            AddLine(new Point(85, 30), new Point(100, 50), pFC);
            pG.Figures = pFC;
            return pG;
        }


        private static Geometry AddSubFolder(PathFigureCollection pFC, PathGeometry pG)
        {
            // front bigger
            AddLine(new Point(0, 10), new Point(0, 70), pFC);
            AddLine(new Point(0, 70), new Point(25, 75), pFC);
            AddLine(new Point(25, 75), new Point(30, 70), pFC);
            AddLine(new Point(30, 70), new Point(30, 25), pFC);
            AddLine(new Point(30, 25), new Point(10, 10), pFC);

            // back bigger
            AddLine(new Point(0, 10), new Point(40, 10), pFC);
            AddLine(new Point(40, 10), new Point(45, 15), pFC);
            AddLine(new Point(45, 15), new Point(45, 35), pFC);

            // front smaller
            AddLine(new Point(25, 75), new Point(25, 85), pFC);
            AddLine(new Point(25, 85), new Point(50, 90), pFC);
            AddLine(new Point(50, 90), new Point(50, 45), pFC);
            AddLine(new Point(50, 45), new Point(30, 40), pFC);

            // back smaller
            AddLine(new Point(30, 35), new Point(55, 35), pFC);
            AddLine(new Point(55, 35), new Point(60, 40), pFC);
            AddLine(new Point(60, 40), new Point(60, 50), pFC);
            AddLine(new Point(60, 65), new Point(60, 80), pFC);
            AddLine(new Point(60, 80), new Point(50, 80), pFC);

            // arrow
            AddLine(new Point(55, 50), new Point(85, 50), pFC);
            AddLine(new Point(85, 50), new Point(85, 40), pFC);
            AddLine(new Point(85, 40), new Point(100, 57.5f), pFC);
            AddLine(new Point(100, 57.5f), new Point(85, 75), pFC);
            AddLine(new Point(85, 75), new Point(85, 65), pFC);
            AddLine(new Point(85, 65), new Point(55, 65), pFC);
            AddLine(new Point(55, 65), new Point(55, 50), pFC);
            pG.Figures = pFC;
            return pG;
        }

        private static Geometry Refresh(PathFigureCollection pFC, PathGeometry pG)
        {
            AddLine(new Point(0, 60), new Point(45, 60), pFC);
            AddLine(new Point(0, 60), new Point(0, 15), pFC);
            AddLine(new Point(45, 60), new Point(0, 15), pFC);

            AddLine(new Point(22.5f, 37.5f), new Point(40, 20), pFC);
            AddLine(new Point(40, 20), new Point(70, 15), pFC);

            AddLine(new Point(100, 40), new Point(55, 40), pFC);
            AddLine(new Point(100, 40), new Point(100, 85), pFC);
            AddLine(new Point(55, 40), new Point(100, 85), pFC);

            AddLine(new Point(77.5f, 62.5f), new Point(60, 80), pFC);
            AddLine(new Point(60, 75), new Point(30, 85), pFC);
            pG.Figures = pFC;
            return pG;
        }

        private static Geometry Remove(PathFigureCollection pFC, PathGeometry pG)
        {
            AddLine(new Point(0, 0), new Point(60, 0), pFC);
            AddLine(new Point(0, 0), new Point(0, 100), pFC);
            AddLine(new Point(80, 20), new Point(80, 100), pFC);
            AddLine(new Point(0, 100), new Point(80, 100), pFC);

            AddLine(new Point(60, 0), new Point(80, 20), pFC);
            AddLine(new Point(60, 0), new Point(60, 20), pFC);
            AddLine(new Point(60, 20), new Point(80, 20), pFC);

            AddLine(new Point(35, 35), new Point(100, 100), pFC);

            AddLine(new Point(100, 35), new Point(35, 100), pFC);

            pG.Figures = pFC;
            return pG;

        }


    }
}
