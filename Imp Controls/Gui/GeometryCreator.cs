#region Usings

using System;
using System.Windows;
using System.Windows.Media;

#endregion

namespace Imp.Controls.Gui
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
        Close,
        ClearList,

        Loop,

        AddFolder,
        AddSubFolder,

        Refresh,

        Remove,
        Save,
        AddFile,
        EnlargeUp,
        EnlargeDown,
        Next,
        
        SortByName,
        Sort,
        Movie,
        Music,
        Picture,

        RememberPath,
        ForgetPath,
        Previous
    }


    internal static class GeometryCreator
    {
        public static Geometry GetGeometry(BtnNumber btn, int state)
        {
            var pFc = new PathFigureCollection();
            var pG = new PathGeometry();

            switch (btn)
            {
                case BtnNumber.Play:
                    if (state == 0)
                    {
                        return PlayGeo(pFc, pG);
                    }
                    return PauseGeo(pFc, pG);
                case BtnNumber.Loop:
                    if (state == 0)
                    {
                        return NoLoop(pFc, pG);
                    }
                    if (state == 1)
                    {
                        return LoopAll(pFc, pG);
                    }
                    return LoopOne(pFc, pG);
                case BtnNumber.Mute:
                    if (state == 1)
                    {
                        return MuteGeo(pFc, pG);
                    }

                    return UnmuteGeo(pFc, pG);

                case BtnNumber.Open:
                    return OpenGeo(pFc, pG);
                case BtnNumber.Help:
                    return HelpGeo(pFc, pG);
                case BtnNumber.Settings:
                    return SettingsGeo(pFc, pG);
                case BtnNumber.Playlist:
                    return PlaylistGeo(pFc, pG);
                case BtnNumber.Save:
                    return SavesGeo(pFc, pG);
                case BtnNumber.Minimize:
                    return MinimizeGeo(pFc, pG);
                case BtnNumber.Maximize:
                    if (state == 0)
                    {
                        return MaximizeGeo(pFc, pG);
                    }
                    if (state == 1)
                    {
                        return NormalizeGeo(pFc, pG);
                    }

                    break;
                case BtnNumber.Close:
                    return ExitGeo(pFc, pG);
                case BtnNumber.ClearList:
                    return ClearGeo(pFc, pG);
                case BtnNumber.AddFile:
                    return AddFile(pFc, pG);
                case BtnNumber.AddFolder:
                    return AddFolder(pFc, pG);
                case BtnNumber.AddSubFolder:
                    return AddSubFolder(pFc, pG);
                case BtnNumber.Refresh:
                    return Refresh(pFc, pG);
                case BtnNumber.Remove:
                    return Remove(pFc, pG);
                case BtnNumber.EnlargeUp:
                    return EnlargeUp(pFc, pG);
                case BtnNumber.EnlargeDown:
                    return EnlargeDown(pFc, pG);
                case BtnNumber.Next:
                    return Next(pFc, pG);
                case BtnNumber.Previous:
                    return Prev(pFc, pG);
                case BtnNumber.SortByName:
                    return SortByName(pFc, pG);
                case BtnNumber.Sort:
                    if (state == 0) { return SortByName(pFc, pG); }
                    if (state == 1) { return SortByTime(pFc, pG); }
                    return SortByView(pFc, pG);
                case BtnNumber.Movie:
                    return Movie(pFc, pG);
                case BtnNumber.Music:
                    return Music(pFc, pG);
                case BtnNumber.Picture:
                    return Pictures(pFc, pG);
                case BtnNumber.RememberPath:
                    return RememberPath(pFc, pG);
                case BtnNumber.ForgetPath:
                    return ForgetPath(pFc, pG);
                default:

                    throw new Exception("Unidentified button");
            }
            return null;
        }

        private static Geometry ForgetPath(PathFigureCollection pFc, PathGeometry pG)
        {
            //AddLine(new Point(0, 0), new Point(40, 40), pFc);
            //AddLine(new Point(0, 40), new Point(40, 0), pFc);
            AddLine(new Point(0, 10), new Point(40, 10), pFc);

            AddLine(new Point(35, 35), new Point(35, 85), pFc);
            AddLine(new Point(35, 85), new Point(70, 100), pFc);

            AddLine(new Point(35, 35), new Point(100, 35), pFc);
            AddLine(new Point(100, 30), new Point(100, 85), pFc);

            AddLine(new Point(70, 45), new Point(70, 100), pFc);
            AddLine(new Point(35, 35), new Point(70, 45), pFc);

            AddLine(new Point(100, 85), new Point(70, 85), pFc);
            pG.Figures = pFc;
            return pG;

        }

        private static Geometry RememberPath(PathFigureCollection pFc, PathGeometry pG)
        {
            AddLine(new Point(20, 0), new Point(20, 40), pFc);
            AddLine(new Point(0, 20), new Point(40, 20), pFc);

            AddLine(new Point(35, 35), new Point(35, 85), pFc);
            AddLine(new Point(35, 85), new Point(70, 100), pFc);

            AddLine(new Point(35, 35), new Point(100, 35), pFc);
            AddLine(new Point(100, 30), new Point(100, 85), pFc);

            AddLine(new Point(70, 45), new Point(70, 100), pFc);
            AddLine(new Point(35, 35), new Point(70, 45), pFc);

            AddLine(new Point(100, 85), new Point(70, 85), pFc);
            pG.Figures = pFc;
            return pG;
        }

        private static Geometry SortByTime(PathFigureCollection pFc, PathGeometry pG)
        {
            AddLine(new Point(10, 100), new Point(20, 100), pFc);
            AddLine(new Point(10, 100), new Point(10, 30), pFc);
            AddLine(new Point(20, 100), new Point(20, 30), pFc);
            AddLine(new Point(10, 30), new Point(0, 30), pFc);
            AddLine(new Point(20, 30), new Point(30, 30), pFc);
            AddLine(new Point(0, 30), new Point(15, 0), pFc);
            AddLine(new Point(30, 30), new Point(15, 0), pFc);

            var center = new Point(70, 60);
            AddLine(center, new Point(65, 40), pFc);
            AddLine(center, new Point(100, 70), pFc);

            pG.Figures = pFc;
            pG.AddGeometry(new EllipseGeometry(center, 30, 30));

            return pG;
        }

        private static Geometry SortByName(PathFigureCollection pFc, PathGeometry pG)
        {
            AddLine(new Point(10, 0), new Point(20, 0), pFc);
            AddLine(new Point(10, 0), new Point(10, 70), pFc);
            AddLine(new Point(20, 0), new Point(20, 70), pFc);
            AddLine(new Point(10, 70), new Point(0, 70), pFc);
            AddLine(new Point(20, 70), new Point(30, 70), pFc);
            AddLine(new Point(0, 70), new Point(15, 100), pFc);
            AddLine(new Point(30, 70), new Point(15, 100), pFc);

            AddLine(new Point(40, 10), new Point(60, 10), pFc);
            AddLine(new Point(40, 30), new Point(70, 30), pFc);
            AddLine(new Point(40, 50), new Point(80, 50), pFc);
            AddLine(new Point(40, 70), new Point(90, 70), pFc);
            AddLine(new Point(40, 90), new Point(100, 90), pFc);

            pG.Figures = pFc;
            return pG;
        }

        private static Geometry SortByView(PathFigureCollection pFc, PathGeometry pG)
        {
            AddLine(new Point(10, 0), new Point(20, 0), pFc);
            AddLine(new Point(10, 0), new Point(10, 70), pFc);
            AddLine(new Point(20, 0), new Point(20, 70), pFc);
            AddLine(new Point(10, 70), new Point(0, 70), pFc);
            AddLine(new Point(20, 70), new Point(30, 70), pFc);
            AddLine(new Point(0, 70), new Point(15, 100), pFc);
            AddLine(new Point(30, 70), new Point(15, 100), pFc);

            var center = new Point(65, 40);
            AddLine(new Point(30, 40), new Point(65, 20), pFc);
            AddLine(new Point(30, 40), new Point(65, 60), pFc);
            AddLine(new Point(100, 40), new Point(65, 20), pFc);
            AddLine(new Point(100, 40), new Point(65, 60), pFc);

            pG.Figures = pFc;
            pG.AddGeometry(new EllipseGeometry(center, 15, 15));

            return pG;
        }

        private static Geometry Pictures(PathFigureCollection pFc, PathGeometry pG)
        {

            AddLine(pFc,
                new Point(0, 90),
                new Point(20, 30),
                new Point(35, 60),
                new Point(60, 65),
                new Point(70, 85),
                new Point(100, 90));
            

            pG.Figures = pFc;
            AddRect(new Rect(0, 10, 100, 80), pG);
            pG.AddGeometry(new EllipseGeometry(new Point(80, 30), 10, 10));

            return pG;
        }

        private static Geometry Music(PathFigureCollection pFc, PathGeometry pG)
        {
            
            AddVLine(new Point(30, 65), 65, pFc );
            AddVLine(new Point(100, 85), 65, pFc);
            AddLine(new Point(30, 0), new Point(100, 20),pFc  );
            AddLine(new Point(30, 20), new Point(100, 40),pFc  );

            pG.Figures = pFc;
            const int r = 15;
            pG.AddGeometry(new EllipseGeometry(new Point(15, 65), r, r));
            pG.AddGeometry(new EllipseGeometry(new Point(85, 85), r, r));

            return pG;
        }

        private static Geometry Movie(PathFigureCollection pFc, PathGeometry pG)
        {
            var d = 20d;
            AddLine(new Point(0, 0), new Point(0, 100), pFc);
            AddLine(new Point(d, 0), new Point(d, 100), pFc);

            AddLine(new Point(100, 0), new Point(100, 100), pFc);
            AddLine(new Point(100 - d, 0), new Point(100-d, 100), pFc);

            for (int i = 0; i < 6; i++)
            {
                AddHLine(new Point(0, i * d), d, pFc);
                AddHLine(new Point(100, i * d), -d, pFc);
            }

            AddHLine(new Point(d, 0), 100-d*2, pFc);
            AddHLine(new Point(d, 50), 100 - d * 2, pFc);
            AddHLine(new Point(d, 100), 100 - d * 2, pFc);


            pG.Figures = pFc;
            return pG;
        }

        private static Geometry Prev(PathFigureCollection pFc, PathGeometry pG)
        {
            AddLine(new Point(100 - 80, 100 - 0), new Point(100 - 80, 100 - 100), pFc);
            AddLine(new Point(100 - 80, 100 - 0), new Point(100 - 100, 100 - 0), pFc);
            AddLine(new Point(100 - 80, 100 - 100), new Point(100 - 100, 100 - 100), pFc);
            AddLine(new Point(100 - 100, 100 - 0), new Point(100 - 100, 100 - 100), pFc);

            AddLine(new Point(100 - 0, 100 - 0), new Point(100 - 80, 100 - 40), pFc);
            AddLine(new Point(100 - 0, 100 - 0), new Point(100 - 0, 100 - 100), pFc);
            AddLine(new Point(100 - 0, 100 - 100), new Point(100 - 80, 100 - 60), pFc);

            pG.Figures = pFc;
            return pG;
        }

        private static Geometry Next(PathFigureCollection pFc, PathGeometry pG)
        {
            AddLine(new Point(80, 0), new Point(80, 100), pFc);
            AddLine(new Point(80, 0), new Point(100, 0), pFc);
            AddLine(new Point(80, 100), new Point(100, 100), pFc);
            AddLine(new Point(100, 0), new Point(100, 100), pFc);

            AddLine(new Point(0, 0), new Point(80, 40), pFc);
            AddLine(new Point(0, 0), new Point(0, 100), pFc);
            AddLine(new Point(0, 100), new Point(80, 60), pFc);

            pG.Figures = pFc;
            return pG;
        }

        private static Geometry EnlargeUp(PathFigureCollection pFc, PathGeometry pG)
        {
            AddLine(new Point(20, 50), new Point(50, 5), pFc);
            AddLine(new Point(80, 50), new Point(50, 5), pFc);

            AddLine(new Point(20, 95), new Point(50, 50), pFc);
            AddLine(new Point(80, 95), new Point(50, 50), pFc);
            ;

            pG.Figures = pFc;
            return pG;
        }

        private static Geometry EnlargeDown(PathFigureCollection pFc, PathGeometry pG)
        {
            AddLine(new Point(20, 50), new Point(50, 95), pFc);
            AddLine(new Point(80, 50), new Point(50, 95), pFc);

            AddLine(new Point(20, 5), new Point(50, 50), pFc);
            AddLine(new Point(80, 5), new Point(50, 50), pFc);
            ;

            pG.Figures = pFc;
            return pG;
        }

        private static Geometry AddFile(PathFigureCollection pFc, PathGeometry pG)
        {
            // sets the arrow position
            var y = -5;

            // file left top triangle
            AddLine(new Point(15, 15), new Point(0, 30), pFc);
            AddLine(new Point(15, 15), new Point(15, 30), pFc);
            AddLine(new Point(0, 30), new Point(15, 30), pFc);

            // file basic rectangle shaper
            AddLine(new Point(15, 15), new Point(60, 15), pFc);
            AddLine(new Point(60, 15), new Point(60, 50 + y), pFc);
            AddLine(new Point(60, 65 + y), new Point(60, 85), pFc);
            AddLine(new Point(60, 85), new Point(0, 85), pFc);
            AddLine(new Point(0, 85), new Point(0, 30), pFc);

            // a couple text lines representing content
            AddLine(new Point(10, 35), new Point(50, 35), pFc);
            AddLine(new Point(10, 50), new Point(50, 50), pFc);
            AddLine(new Point(10, 65), new Point(50, 65), pFc);


            // arrow
            AddLine(new Point(55, 50 + y), new Point(85, 50 + y), pFc);
            AddLine(new Point(85, 50 + y), new Point(85, 40 + y), pFc);
            AddLine(new Point(85, 40 + y), new Point(100, 57.5f + y), pFc);
            AddLine(new Point(100, 57.5f + y), new Point(85, 75 + y), pFc);
            AddLine(new Point(85, 75 + y), new Point(85, 65 + y), pFc);
            AddLine(new Point(85, 65 + y), new Point(55, 65 + y), pFc);
            AddLine(new Point(55, 65 + y), new Point(55, 50 + y), pFc);
            pG.Figures = pFc;
            return pG;
        }

        private static void AddHLine(Point p1, double d, PathFigureCollection pathF)
        {
            AddLine(p1, new Point(p1.X + d, p1.Y), pathF);
        }

        private static void AddVLine(Point p1, double d, PathFigureCollection pathF)
        {
            AddLine(p1, new Point(p1.X, p1.Y - d), pathF);
        }

        private static void AddLine(PathFigureCollection pathF, params Point[] points)
        {
            for (int i = 0; i < points.Length-1; i++)
            {
                AddLine(points[i], points[i+1], pathF);
            }
        }

        private static void AddLine(Point p1, Point p2, PathFigureCollection pathF)
        {
            var polkuFigure = new PathFigure();
            var myLineSegment = new LineSegment();
            var pathS = new PathSegmentCollection();
            polkuFigure.StartPoint = p1;
            myLineSegment.Point = p2;
            pathS.Add(myLineSegment);

            polkuFigure.Segments = pathS;
            pathF.Add(polkuFigure);
        }

        private static void AddRect(Rect box, PathGeometry geom)
        {
            var loota = new RectangleGeometry();
            loota.Rect = box;
            geom.AddGeometry(loota);
        }

        private static void AddCircle(Rect box, PathGeometry geom)
        {
            var pallo = new EllipseGeometry(box);
            geom.AddGeometry(pallo);
        }

        private static Geometry SavesGeo(PathFigureCollection pFc, PathGeometry pG)
        {
            AddLine(new Point(0, 2), new Point(2, 0), pFc);
            AddLine(new Point(2, 0), new Point(95, 0), pFc);

            AddLine(new Point(95, 0), new Point(100, 5), pFc);
            AddLine(new Point(100, 5), new Point(100, 98), pFc);

            AddLine(new Point(100, 98), new Point(98, 100), pFc);
            AddLine(new Point(98, 100), new Point(2, 100), pFc);

            AddLine(new Point(2, 100), new Point(0, 98), pFc);
            AddLine(new Point(0, 98), new Point(0, 2), pFc);

            AddLine(new Point(20, 0), new Point(20, 35), pFc);
            AddLine(new Point(20, 35), new Point(80, 35), pFc);
            AddLine(new Point(80, 35), new Point(80, 0), pFc);

            pG.Figures = pFc;

            AddLine(new Point(60, 0), new Point(60, 25), pFc);
            AddLine(new Point(60, 25), new Point(70, 25), pFc);
            AddLine(new Point(70, 25), new Point(70, 0), pFc);

            //AddRect(new Rect(55,5,10,20), pG);
            return pG;
        }

        private static PathGeometry PlayGeo(PathFigureCollection pFc, PathGeometry pG)
        {
            AddLine(new Point(0, 0), new Point(100, 50), pFc);
            AddLine(new Point(0, 0), new Point(0, 100), pFc);
            AddLine(new Point(0, 100), new Point(100, 50), pFc);

            pG.Figures = pFc;
            return pG;
        }

        private static PathGeometry PauseGeo(PathFigureCollection pFc, PathGeometry pG)
        {
            AddRect(new Rect(0, 0, 35, 100), pG);
            AddRect(new Rect(65, 0, 35, 100), pG);
            return pG;
        }

        private static PathGeometry MuteGeo(PathFigureCollection pFc, PathGeometry pG)
        {
            MuteLines(pFc);

            AddLine(new Point(65, 25), new Point(90, 0), pFc);
            AddLine(new Point(70, 50), new Point(100, 50), pFc);
            AddLine(new Point(65, 75), new Point(90, 100), pFc);

            pG.Figures = pFc;
            return pG;
        }

        private static PathGeometry UnmuteGeo(PathFigureCollection pFc, PathGeometry pG)
        {
            MuteLines(pFc);

            pG.Figures = pFc;
            return pG;
        }

        private static PathFigureCollection MuteLines(PathFigureCollection pFc)
        {
            AddLine(new Point(0, 50), new Point(40, 0), pFc);
            AddLine(new Point(0, 50), new Point(40, 100), pFc);

            AddLine(new Point(40, 0), new Point(30, 50), pFc);
            AddLine(new Point(40, 0), new Point(50, 50), pFc);

            AddLine(new Point(40, 100), new Point(30, 50), pFc);
            AddLine(new Point(40, 100), new Point(50, 50), pFc);
            return pFc;
        }

        private static PathGeometry OpenGeo(PathFigureCollection pFc, PathGeometry pG)
        {
            AddLine(new Point(50, 0), new Point(0, 60), pFc);
            AddLine(new Point(50, 0), new Point(100, 60), pFc);
            AddLine(new Point(0, 60), new Point(100, 60), pFc);

            AddLine(new Point(0, 100), new Point(100, 100), pFc);
            AddLine(new Point(0, 80), new Point(100, 80), pFc);
            AddLine(new Point(0, 80), new Point(0, 100), pFc);
            AddLine(new Point(100, 80), new Point(100, 100), pFc);


            pG.Figures = pFc;
            return pG;
        }

        private static PathGeometry HelpGeo(PathFigureCollection pFc, PathGeometry pG)
        {
            AddLine(new Point(20, 40), new Point(10, 20), pFc);
            AddLine(new Point(10, 20), new Point(20, 10), pFc);

            AddLine(new Point(20, 10), new Point(50, 0), pFc);
            AddLine(new Point(50, 0), new Point(75, 5), pFc);
            AddLine(new Point(75, 5), new Point(85, 25), pFc);
            AddLine(new Point(85, 25), new Point(55, 40), pFc);

            AddLine(new Point(55, 40), new Point(50, 60), pFc);

            AddLine(new Point(50, 70), new Point(35, 85), pFc);
            AddLine(new Point(50, 70), new Point(65, 85), pFc);
            AddLine(new Point(50, 100), new Point(35, 85), pFc);

            AddLine(new Point(50, 100), new Point(65, 85), pFc);


            pG.Figures = pFc;
            return pG;
        }

        private static PathGeometry SettingsGeo(PathFigureCollection pFc, PathGeometry pG)
        {
            AddLine(new Point(0, 40), new Point(40, 0), pFc);
            AddLine(new Point(0, 40), new Point(15, 55), pFc);
            AddLine(new Point(40, 0), new Point(55, 15), pFc);

            AddLine(new Point(15, 55), new Point(30, 40), pFc);
            AddLine(new Point(55, 15), new Point(40, 30), pFc);

            AddLine(new Point(30, 40), new Point(80, 100), pFc);
            AddLine(new Point(40, 30), new Point(100, 80), pFc);
            AddLine(new Point(80, 100), new Point(100, 80), pFc);

            pG.Figures = pFc;
            return pG;
        }

        private static PathGeometry PlaylistGeo(PathFigureCollection pFc, PathGeometry pG)
        {
            //AddLine(new Point(0, 0), new Point(100, 0), pFc);
            //AddLine(new Point(0, 33), new Point(100, 33), pFc);
            //AddLine(new Point(0, 67), new Point(100, 67), pFc);

            //AddLine(new Point(0, 100), new Point(100, 100), pFc);


            //pG.Figures = pFc;

            AddRect(new Rect(0, 0, 100, 10), pG);
            AddRect(new Rect(0, 30, 100, 10), pG);
            AddRect(new Rect(0, 60, 100, 10), pG);

            AddRect(new Rect(0, 90, 100, 10), pG);

            return pG;
        }

        private static PathGeometry MinimizeGeo(PathFigureCollection pFc, PathGeometry pG)
        {
            AddRect(new Rect(0, 80, 100, 20), pG);
            return pG;
        }

        private static PathGeometry NormalizeGeo(PathFigureCollection pFc, PathGeometry pG)
        {
            AddLine(new Point(20, 0), new Point(20, 20), pFc);
            AddLine(new Point(20, 0), new Point(100, 0), pFc);
            AddLine(new Point(100, 0), new Point(100, 80), pFc);
            AddLine(new Point(100, 80), new Point(80, 80), pFc);
            pG.Figures = pFc;
            AddRect(new Rect(0, 20, 80, 80), pG);
            return pG;
        }

        private static PathGeometry MaximizeGeo(PathFigureCollection pFc, PathGeometry pG)
        {
            AddRect(new Rect(0, 0, 100, 20), pG);
            AddRect(new Rect(0, 0, 100, 100), pG);
            return pG;
        }

        private static PathGeometry ExitGeo(PathFigureCollection pFc, PathGeometry pG)
        {
            //AddLine(new Point(0, 0), new Point(100, 100), pFc);
            //AddLine(new Point(100, 0), new Point(0, 100), pFc);

            AddLine(new Point(10, 0), new Point(50, 40), pFc);
            AddLine(new Point(10, 0), new Point(0, 10), pFc);
            AddLine(new Point(0, 10), new Point(40, 50), pFc);

            AddLine(new Point(90, 0), new Point(50, 40), pFc);
            AddLine(new Point(90, 0), new Point(100, 10), pFc);
            AddLine(new Point(100, 10), new Point(60, 50), pFc);

            AddLine(new Point(10, 100), new Point(50, 60), pFc);
            AddLine(new Point(10, 100), new Point(0, 90), pFc);
            AddLine(new Point(0, 90), new Point(40, 50), pFc);

            AddLine(new Point(90, 100), new Point(50, 60), pFc);
            AddLine(new Point(90, 100), new Point(100, 90), pFc);
            AddLine(new Point(100, 90), new Point(60, 50), pFc);

            pG.Figures = pFc;
            return pG;
        }

        private static PathGeometry ClearGeo(PathFigureCollection pFc, PathGeometry pG)
        {
            AddLine(new Point(0, 10), new Point(100, 10), pFc);
            AddLine(new Point(0, 35), new Point(100, 35), pFc);
            AddLine(new Point(0, 60), new Point(100, 60), pFc);
            AddLine(new Point(0, 85), new Point(100, 85), pFc);

            AddLine(new Point(10, 0), new Point(90, 100), pFc);
            AddLine(new Point(90, 0), new Point(10, 100), pFc);

            pG.Figures = pFc;
            return pG;
        }

        private static Geometry NoLoop(PathFigureCollection pFc, PathGeometry pG)
        {
            AddLine(new Point(0, 50), new Point(100, 50), pFc);

            pG.Figures = pFc;
            return pG;
        }

        private static Geometry LoopAll(PathFigureCollection pFc, PathGeometry pG)
        {
            AddLine(new Point(25, 30), new Point(75, 30), pFc);
            AddLine(new Point(25, 70), new Point(75, 70), pFc);

            AddLine(new Point(0, 50), new Point(25, 30), pFc);
            AddLine(new Point(0, 50), new Point(25, 70), pFc);

            AddLine(new Point(100, 50), new Point(75, 30), pFc);
            AddLine(new Point(100, 50), new Point(75, 70), pFc);

            AddLine(new Point(30, 10), new Point(50, 30), pFc);
            AddLine(new Point(30, 50), new Point(50, 30), pFc);

            AddLine(new Point(70, 50), new Point(50, 70), pFc);
            AddLine(new Point(70, 90), new Point(50, 70), pFc);

            pG.Figures = pFc;
            return pG;
        }

        private static Geometry LoopOne(PathFigureCollection pFc, PathGeometry pG)
        {
            AddLine(new Point(0, 50), new Point(15, 50), pFc);
            AddLine(new Point(40, 50), new Point(60, 50), pFc);
            AddLine(new Point(85, 50), new Point(100, 50), pFc);

            AddLine(new Point(25, 5), new Point(50, 30), pFc);

            AddLine(new Point(75, 95), new Point(50, 70), pFc);
            pG.Figures = pFc;
            AddCircle(new Rect(25, 25, 50, 50), pG);

            pG.Figures = pFc;
            return pG;
        }

        private static Geometry AddFolder(PathFigureCollection pFc, PathGeometry pG)
        {
            AddLine(new Point(0, 0), new Point(0, 85), pFc);
            AddLine(new Point(0, 85), new Point(30, 100), pFc);

            AddLine(new Point(0, 0), new Point(70, 0), pFc);
            AddLine(new Point(70, 0), new Point(70, 40), pFc);
            AddLine(new Point(70, 60), new Point(70, 85), pFc);

            AddLine(new Point(30, 15), new Point(30, 100), pFc);
            AddLine(new Point(0, 0), new Point(30, 15), pFc);

            AddLine(new Point(70, 85), new Point(30, 85), pFc);

            // arrow
            AddLine(new Point(45, 60), new Point(45, 40), pFc);
            AddLine(new Point(45, 60), new Point(85, 60), pFc);
            AddLine(new Point(45, 40), new Point(85, 40), pFc);
            AddLine(new Point(85, 60), new Point(85, 70), pFc);
            AddLine(new Point(85, 40), new Point(85, 30), pFc);
            // arrow head
            AddLine(new Point(85, 70), new Point(100, 50), pFc);
            AddLine(new Point(85, 30), new Point(100, 50), pFc);
            pG.Figures = pFc;
            return pG;
        }

        private static Geometry AddSubFolder(PathFigureCollection pFc, PathGeometry pG)
        {
            // front bigger
            AddLine(new Point(0, 10), new Point(0, 70), pFc);
            AddLine(new Point(0, 70), new Point(25, 75), pFc);
            AddLine(new Point(25, 75), new Point(30, 70), pFc);
            AddLine(new Point(30, 70), new Point(30, 25), pFc);
            AddLine(new Point(30, 25), new Point(10, 10), pFc);

            // back bigger
            AddLine(new Point(0, 10), new Point(40, 10), pFc);
            AddLine(new Point(40, 10), new Point(45, 15), pFc);
            AddLine(new Point(45, 15), new Point(45, 35), pFc);

            // front smaller
            AddLine(new Point(25, 75), new Point(25, 85), pFc);
            AddLine(new Point(25, 85), new Point(50, 90), pFc);
            AddLine(new Point(50, 90), new Point(50, 45), pFc);
            AddLine(new Point(50, 45), new Point(30, 40), pFc);

            // back smaller
            AddLine(new Point(30, 35), new Point(55, 35), pFc);
            AddLine(new Point(55, 35), new Point(60, 40), pFc);
            AddLine(new Point(60, 40), new Point(60, 50), pFc);
            AddLine(new Point(60, 65), new Point(60, 80), pFc);
            AddLine(new Point(60, 80), new Point(50, 80), pFc);

            // arrow
            AddLine(new Point(55, 50), new Point(85, 50), pFc);
            AddLine(new Point(85, 50), new Point(85, 40), pFc);
            AddLine(new Point(85, 40), new Point(100, 57.5f), pFc);
            AddLine(new Point(100, 57.5f), new Point(85, 75), pFc);
            AddLine(new Point(85, 75), new Point(85, 65), pFc);
            AddLine(new Point(85, 65), new Point(55, 65), pFc);
            AddLine(new Point(55, 65), new Point(55, 50), pFc);
            pG.Figures = pFc;
            return pG;
        }

        private static Geometry Refresh(PathFigureCollection pFc, PathGeometry pG)
        {
            AddLine(pFc, 
                new Point(50, 15),
                new Point(50, -5),
                new Point(75, 20),
                new Point(50, 45),
                new Point(50, 25));
            AddLine(new Point(90, 50), new Point(70, 50), pFc);
            pG.Figures = pFc;
            var figure1 = new PathFigure();
            figure1.StartPoint = new Point(50,15);
            figure1.Segments.Add(new ArcSegment(new Point(85,50), new Size(35, 35), 45, true, SweepDirection.Counterclockwise, true) );
            pG.Figures.Add(figure1);

            var figure2 = new PathFigure();
            figure2.StartPoint = new Point(50, 25);
            figure2.Segments.Add(new ArcSegment(new Point(75, 50), new Size(25, 25), 45, true, SweepDirection.Counterclockwise, true));
            pG.Figures.Add(figure2);
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(new TranslateTransform(-50,-50));
            transformGroup.Children.Add(new RotateTransform(45));
            transformGroup.Children.Add(new TranslateTransform(50, 50));
            pG.Transform = transformGroup;
            return pG.GetFlattenedPathGeometry();
        }

        private static Geometry Remove(PathFigureCollection pFc, PathGeometry pG)
        {
            AddLine(pFc, new Point(20, 0),
                 new Point(60, 0),
                 new Point(60, 70),
                 new Point(0, 70),
                 new Point(0, 20),
                 new Point(20, 0));

            AddLine(pFc, new Point(20, 0),
                new Point(20, 20),
                new Point(0, 20));

            // a couple text lines representing content
            AddHLine(new Point(10, 20), 40, pFc);
            AddHLine(new Point(10, 35), 40, pFc);
            AddHLine(new Point(10, 50), 40, pFc);

            AddLine(new Point(35, 35), new Point(100, 100), pFc);
            AddLine(new Point(100, 35), new Point(35, 100), pFc);

            pG.Figures = pFc;
            return pG;
        }
    }
}