using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;

namespace NShapeCreator.GDI
{
    [XmlInclude(typeof(Arc)), XmlInclude(typeof(CubicBezier)), XmlInclude(typeof(Polygon)), XmlInclude(typeof(Lines)), XmlInclude(typeof(Rectangle))]
    public class GDIPathSegment : ICloneable
    {

        [Category("\tSegment")]
        [DisplayName("Is A Valid Path Segment")]
        [Description("Displays whether this segment is valid or not - given the provided information.\nNOTE: Only valid segments will be drawn to the display.")]
        public virtual bool IsValidPathSegment
        {
            get { return false; }
        }

        [Category("\tSegment")]
        [DisplayName("Path Segment Type")]
        [Description("Type of segment to draw.")]
        public GDIPathSegmentTypeName SegmentType
        {
            get;
            set;
        }

        [Category("\tSegment")]
        [DisplayName("Path Segment Name")]
        [Description("The name for this path segment.")]
        public string Name
        {
            get;
            set;
        }

        [Category("\tSegment Continuation")]
        [DisplayName("\tStarts New Figure")]
        [Description("Start a new figure or continue with the previous segment")]
        public bool StartFigure
        {
            get;
            set;
        }

        [Category("\tSegment Continuation")]
        [DisplayName("Close Figure")]
        [Description("Close this figure (connect to the starting point)")]
        public bool CloseFigure
        {
            get;
            set;
        }

        public virtual Object Clone()
        {
            GDIPathSegment me = new GDIPathSegment();
            me.CloseFigure = CloseFigure;
            me.StartFigure = StartFigure;
            me.Name = Name;
            return (Object)me;
        }

        public GDIPathSegment()
        {
        }

    }

    public class Arc : GDIPathSegment
    {

        public override bool IsValidPathSegment
        {
            get
            {
                bool returnValue = false;
                try
                {
                    using (GraphicsPath p = new GraphicsPath())
                    {
                        p.AddArc(X, Y, Width, Height, StartAngle, SweepAngle);
                        returnValue = true;
                    }
                }
                catch
                {
                }
                return returnValue;
            }
        }

        private float _x;
        [Category("Layout")]
        [DisplayName("\t\tX")]
        public float X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        private float _y;
        [Category("Layout")]
        [DisplayName("\t\tY")]
        public float Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }

        private float _width;
        [Category("Layout")]
        [DisplayName("\tWidth")]
        public float Width
        {
            get { return _width; }
            set
            {
                _width = value;
            }
        }

        private float _height;
        [Category("Layout")]
        [DisplayName("Height")]
        public float Height
        {
            get { return _height; }
            set
            {
                _height = value;
            }
        }

        private float _startAngle;
        [Category("Layout")]
        [DisplayName("Start Angle")]
        public float StartAngle
        {
            get { return _startAngle; }
            set
            {
                _startAngle = value;
            }
        }

        private float _sweepAngle;
        [Category("Layout")]
        [DisplayName("Sweep Angle")]
        public float SweepAngle
        {
            get { return _sweepAngle; }
            set
            {
                _sweepAngle = value;
            }
        }

        public override Object Clone()
        {
            Arc me = new Arc();
            me.SegmentType = SegmentType;
            me.CloseFigure = CloseFigure;
            me.StartFigure = StartFigure;
            me.X = X;
            me.Y = Y;
            me.Width = Width;
            me.Height = Height;
            me.SweepAngle = SweepAngle;
            me.StartAngle = StartAngle;
            me.Name = Name;
            return me;
        }

        public Arc()
        {
            SegmentType = GDIPathSegmentTypeName.Arc;
        }

    }

    public class CubicBezier : GDIPathSegment
    {

        private float _startingPointX;
        [Category("Layout")]
        [DisplayName("\t\tStarting X")]
        public float StartingPointX
        {
            get { return _startingPointX; }
            set
            {
                _startingPointX = value;
            }
        }

        private float _startingPointY;
        [Category("Layout")]
        [DisplayName("\t\tStarting Y")]
        public float StartingPointY
        {
            get { return _startingPointY; }
            set
            {
                _startingPointY = value;
            }
        }

        private float _controlPointOneX;
        [Category("Layout")]
        [DisplayName("Control Point One X")]
        public float ControlPointOneX
        {
            get { return _controlPointOneX; }
            set
            {
                _controlPointOneX = value;
            }
        }

        private float _controlPointOneY;
        [Category("Layout")]
        [DisplayName("Control Point One Y")]
        public float ControlPointOneY
        {
            get { return _controlPointOneY; }
            set
            {
                _controlPointOneY = value;
            }
        }

        private float _controlPointTwoX;
        [Category("Layout")]
        [DisplayName("Control Point Two X")]
        public float ControlPointTwoX
        {
            get { return _controlPointTwoX; }
            set
            {
                _controlPointTwoX = value;
            }
        }

        private float _controlPointTwoY;
        [Category("Layout")]
        [DisplayName("Control Point Two Y")]
        public float ControlPointTwoY
        {
            get { return _controlPointTwoY; }
            set
            {
                _controlPointTwoY = value;
            }
        }

        private float _endingPointX;
        [Category("Layout")]
        [DisplayName("\tEnding X")]
        public float EndingPointX
        {
            get { return _endingPointX; }
            set
            {
                _endingPointX = value;
            }
        }

        private float _endingPointY;
        [Category("Layout")]
        [DisplayName("\tEnding Y")]
        public float EndingPointY
        {
            get { return _endingPointY; }
            set
            {
                _endingPointY = value;
            }
        }

        public override bool IsValidPathSegment
        {
            get
            {
                bool returnValue = false;
                try
                {
                    using (GraphicsPath p = new GraphicsPath())
                    {
                        p.AddBezier(StartingPointX, StartingPointY, ControlPointOneX, ControlPointOneY, ControlPointTwoX, ControlPointTwoY, EndingPointX, EndingPointY);
                        returnValue = true;
                    }
                }
                catch
                {
                }
                return returnValue;
            }
        }

        public override Object Clone()
        {
            CubicBezier me = new CubicBezier();
            me.SegmentType = SegmentType;
            me.CloseFigure = CloseFigure;
            me.StartFigure = StartFigure;
            me.StartingPointX = StartingPointX;
            me.ControlPointOneX = ControlPointOneX;
            me.ControlPointTwoX = ControlPointTwoX;
            me.EndingPointX = EndingPointX;
            me.StartingPointY = StartingPointY;
            me.ControlPointOneY = ControlPointOneY;
            me.ControlPointTwoY = ControlPointTwoY;
            me.EndingPointY = EndingPointY;
            me.Name = Name;
            return me;
        }

        public CubicBezier()
        {
            SegmentType = GDIPathSegmentTypeName.CubicBezier;
        }

    }

    public class Polygon : GDIPathSegment
    {

        private PointF[] _points;
        [Category("Layout")]
        [DisplayName("\tPoints")]
        public PointF[] Points
        {
            get
            {
                return _points;
            }
            set
            {
                _points = value;
            }
        }

        [Category("Layout")]
        [DisplayName("Point Index")]
        public int PointIndex
        {
            get;
            set;
        }

        [Category("Layout")]
        public float X
        {
            get
            {
                return Points[PointIndex].X;
            }
            set
            {
                Points[PointIndex].X = value;
            }
        }

        [Category("Layout")]
        public float Y
        {
            get
            {
                return Points[PointIndex].Y;
            }
            set
            {
                Points[PointIndex].Y = value;
            }
        }

        public override bool IsValidPathSegment
        {
            get
            {
                bool returnValue = false;
                try
                {
                    using (GraphicsPath p = new GraphicsPath())
                    {
                        p.AddPolygon(Points);
                        returnValue = true;
                    }
                }
                catch
                {
                }
                return returnValue;
            }
        }

        public override Object Clone()
        {
            Polygon me = new Polygon();
            me.CloseFigure = CloseFigure;
            me.StartFigure = StartFigure;
            me.SegmentType = SegmentType;
            me.Name = Name;
            if (Points != null)
            {
                me.Points = (PointF[])Points.Clone();
                me.PointIndex = PointIndex;
            }
            return me;
        }

        public Polygon()
        {
            SegmentType = GDIPathSegmentTypeName.Polygon;
            _points = new PointF[3];
        }

    }

    public class Lines : GDIPathSegment
    {

        private PointF[] _points;
        [Category("Layout")]
        [DisplayName("\tPoints")]
        public PointF[] Points
        {
            get
            {
                return _points;
            }
            set
            {
                _points = value;
            }
        }

        [Category("Layout")]
        [DisplayName("Point Index")]
        public int PointIndex
        {
            get;
            set;
        }

        [Category("Layout")]
        public float X
        {
            get
            {
                return Points[PointIndex].X;
            }
            set
            {
                Points[PointIndex].X = value;
            }
        }

        [Category("Layout")]
        public float Y
        {
            get
            {
                return Points[PointIndex].Y;
            }
            set
            {
                Points[PointIndex].Y = value;
            }
        }

        public override bool IsValidPathSegment
        {
            get
            {
                bool returnValue = false;
                try
                {
                    using (GraphicsPath p = new GraphicsPath())
                    {
                        p.AddLines(Points);
                        returnValue = true;
                    }
                }
                catch
                {
                }
                return returnValue;
            }
        }

        public override Object Clone()
        {
            Lines me = new Lines();
            me.SegmentType = SegmentType;
            me.CloseFigure = CloseFigure;
            me.StartFigure = StartFigure;
            me.Name = Name;
            if (Points != null)
            {
                me.Points = (PointF[])Points.Clone();
                me.PointIndex = PointIndex;
            }
            return me;
        }

        public Lines()
        {
            SegmentType = GDIPathSegmentTypeName.Lines;
            _points = new PointF[2]
            {
                PointF.Empty,
                PointF.Empty
            };
        }

    }

    public class Rectangle : GDIPathSegment
    {

        private System.Drawing.RectangleF _rectangle;
        [Category("Layout")]
        [DisplayName("\tRectangle")]
        public System.Drawing.RectangleF GDIRectangleF
        {
            get
            {
                return _rectangle;
            }
            set
            {
                _rectangle = value;
            }
        }

        [Category("Layout")]
        public float X
        {
            get
            {
                return _rectangle.X;
            }
            set
            {
                _rectangle.X = value;
            }
        }

        [Category("Layout")]
        public float Y
        {
            get
            {
                return _rectangle.Y;
            }
            set
            {
                _rectangle.Y = value;
            }
        }

        [Category("Layout")]
        [DisplayName("\tWidth")]
        public float Width
        {
            get 
            {
                return _rectangle.Width; 
            }
            set
            {
                _rectangle.Width = value;
            }
        }

        [Category("Layout")]
        [DisplayName("Height")]
        public float Height
        {
            get 
            {
                return _rectangle.Height; 
            }
            set
            {
                _rectangle.Height = value;
            }
        }

        public override bool IsValidPathSegment
        {
            get
            {
                bool returnValue = false;
                try
                {
                    using (GraphicsPath p = new GraphicsPath())
                    {
                        p.AddRectangle(GDIRectangleF);
                        returnValue = true;
                    }
                }
                catch
                {
                }
                return returnValue;
            }
        }

        public override Object Clone()
        {
            Rectangle me = new Rectangle();
            me.SegmentType = SegmentType;
            me.CloseFigure = CloseFigure;
            me.StartFigure = StartFigure;
            me.Name = Name;
            me.GDIRectangleF = new System.Drawing.RectangleF(GDIRectangleF.Location, GDIRectangleF.Size);
            return me;
        }

        public Rectangle()
        {
            _rectangle = new System.Drawing.RectangleF();
            SegmentType = GDIPathSegmentTypeName.Rectangle;
        }

    }

}