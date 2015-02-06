using System;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace NShapeCreator.GDI
{

    #region Enums

    public enum GDIPathSegmentTypeName
    {
        Polygon,
        Arc,
        CubicBezier,
        Lines,
        Rectangle,
    }

    public enum PointTypeID
    {
        GDI,
        Cartesian,
        NShape,
    }

    #endregion

    [Serializable]
    public class GDIShape : ICloneable
    {

        public GDIShape()
        {
            Segments = new GDIPathSegmentCollection();
            PenColor = Color.Black;
            FillColor = Color.SteelBlue;
            PenWidth = 1;
            Name = "NewShape";
            OutPutPointType = PointTypeID.NShape;
            XAxisGridMultiple = 1;
            YAxisGridMultiple = 1;
            DisplaySizeMultiple = 40;
            ImageSizeMultiple = 40;
            PointSizeMultiple = 1;
            Width = 10;
            Height = 10;
        }

        [Category("\t\tShape Options")]
        [Description("The name that will be assigned this shape")]
        [DisplayName("\tShape Name")]
        public string Name { get; set; }

        [Browsable(false)]
        [Category("\tPoint Output Options")]
        [Description("This determines the X and Y point value interpretation")]
        [DisplayName("Output Point Type")]
        public PointTypeID OutPutPointType { get; set; }

        private float _XAxisGridMultiple;
        [Category("\tDisplay Output Options")]
        [DisplayName("X-Axis Grid Multiple")]
        [Description("This sets how often the X-Axis grid line is painted")]
        public float XAxisGridMultiple
        {
            get { return _XAxisGridMultiple; }
            set { _XAxisGridMultiple = value; }
        }

        private float _YAxisGridMultiple;
        [Category("\tDisplay Output Options")]
        [DisplayName("Y-Axis Grid Multiple")]
        [Description("This sets how often the Y-Axis grid line is painted")]
        public float YAxisGridMultiple
        {
            get { return _YAxisGridMultiple; }
            set { _YAxisGridMultiple = value; }
        }

        private float _displaySizeMultiple;
        [Category("\tDisplay Output Options")]
        [Description("Multiplies the display size (X and Y) of the image")]
        [DisplayName("Display Size Multiple")]
        public float DisplaySizeMultiple
        {
            get { return _displaySizeMultiple; }
            set { _displaySizeMultiple = value; }
        }

        private float _pointSizeMultiple;
        [Category("\tPoint Output Options")]
        [Description("Multiplies the display size (X and Y) of the points")]
        [DisplayName("Point Size Multiple")]
        public float PointSizeMultiple
        {
            get { return _pointSizeMultiple; }
            set { _pointSizeMultiple = value; }
        }

        private float _imageSizeMultiple;
        [Category("\tImage Output Options")]
        [Description("Multiplies the image size (X and Y) of the image")]
        [DisplayName("Image Size Multiple")]
        public float ImageSizeMultiple
        {
            get { return _imageSizeMultiple; }
            set { _imageSizeMultiple = value; }
        }

        [Category("\t\tShape Options")]
        [DisplayName("\tWidth")]
        public float Width
        {
            get;
            set;
        }

        [Category("\t\tShape Options")]
        public float Height
        {
            get;
            set;
        }

        [XmlIgnore()]
        [Category("Path Options")]
        [Description("Color of the brush to use to fill this path")]
        [DisplayName("Fill Color")]
        public Color FillColor { get; set; }

        [XmlElement("FillColorStringName")]
        [Browsable(false)]
        public string FillColorStringName
        {
            get
            {
                return SerializeColor(FillColor);
            }
            set
            {
                FillColor = DeserializeColor(value);
            }
        }

        [XmlElement("PenColorStringName")]
        [Browsable(false)]
        public string PenColorStringName
        {
            get
            {
                return SerializeColor(PenColor);
            }
            set
            {
                PenColor = DeserializeColor(value);
            }
        }

        private string SerializeColor(Color color)
        {
            return color.Name;
        }

        private Color DeserializeColor(string colorName)
        {
            return Color.FromName(colorName);
        }

        [Category("Path Options")]
        [Description("Width of the pen to use to paint this path")]
        [DisplayName("Pen Width")]
        public float PenWidth { get; set; }

        [XmlIgnore()]
        [Category("Path Options")]
        [Description("Color of the pen to use to paint this path")]
        [DisplayName("Pen Color")]
        public Color PenColor { get; set; }

        public static GDIShape GetPoints(GDIShape cartesian, PointTypeID pointType, Size canvasSize, float multiplier)
        {
            GDIShape returnValue = null;
            switch (pointType)
            {
                case PointTypeID.GDI:
                    returnValue = GetGDIPoints(cartesian, canvasSize, multiplier);
                    break;
                case PointTypeID.Cartesian:
                    returnValue = GetCartesianPoints(cartesian, canvasSize, multiplier);
                    break;
                case PointTypeID.NShape:
                    returnValue = GetNShapePoints(cartesian, canvasSize, multiplier);
                    break;
            }
            return returnValue;
        }

        private static GDIShape GetGDIPoints(GDIShape cartesian, Size canvaseSize, float multiplier)
        {
            GDIShape returnValue = (GDIShape)cartesian.Clone();
            float xOffset = canvaseSize.Width / 2f;
            float yOffset = canvaseSize.Height / 2f;
            for (int i2 = 0; i2 < returnValue.Segments.Count; i2++)
            {
                if (returnValue.Segments[i2].IsValidPathSegment)
                {
                    switch (returnValue.Segments[i2].SegmentType)
                    {
                        case GDIPathSegmentTypeName.Arc:
                            Arc arc = (Arc)((Arc)returnValue.Segments[i2]);
                            arc.X = arc.X * multiplier + xOffset;
                            arc.Y = arc.Y * -1 * multiplier + yOffset;
                            arc.Width = arc.Width * multiplier;
                            arc.Height = arc.Height * multiplier;
                            if (arc.StartAngle > 180)
                            {
                                arc.StartAngle -= 180;
                            }
                            else
                            {
                                arc.StartAngle += 180;
                            }
                            break;
                        case GDIPathSegmentTypeName.CubicBezier:
                            CubicBezier cub = (CubicBezier)((CubicBezier)returnValue.Segments[i2]);
                            cub.StartingPointX = cub.StartingPointX * multiplier + xOffset;
                            cub.StartingPointY = cub.StartingPointY * -1 * multiplier + yOffset;
                            cub.ControlPointOneX = cub.ControlPointOneX * multiplier + xOffset;
                            cub.ControlPointOneY = cub.ControlPointOneY * -1 * multiplier + yOffset;
                            cub.ControlPointTwoX = cub.ControlPointTwoX * multiplier + xOffset;
                            cub.ControlPointTwoY = cub.ControlPointTwoY * -1 * multiplier + yOffset;
                            cub.EndingPointX = cub.EndingPointX * multiplier + xOffset;
                            cub.EndingPointY = cub.EndingPointY * -1 * multiplier + yOffset;
                            break;
                        case GDIPathSegmentTypeName.Lines:
                            PointF[] gdiLinePoints = (PointF[])((Lines)returnValue.Segments[i2]).Points;
                            for (int i3 = 0; i3 < gdiLinePoints.Length; i3++)
                            {
                                gdiLinePoints[i3].X *= multiplier;
                                gdiLinePoints[i3].Y *= -1 * multiplier;
                                gdiLinePoints[i3].X += xOffset;
                                gdiLinePoints[i3].Y += yOffset;
                            }
                            break;
                        case GDIPathSegmentTypeName.Polygon:
                            PointF[] gdipolyPoints = (PointF[])((Polygon)returnValue.Segments[i2]).Points;
                            for (int i3 = 0; i3 < gdipolyPoints.Length; i3++)
                            {
                                gdipolyPoints[i3].X *= multiplier;
                                gdipolyPoints[i3].Y *= -1 * multiplier;
                                gdipolyPoints[i3].X += xOffset;
                                gdipolyPoints[i3].Y += yOffset;
                            }
                            break;
                        case GDIPathSegmentTypeName.Rectangle:
                            Rectangle rec = (Rectangle)((Rectangle)returnValue.Segments[i2]);
                            rec.X *= multiplier;
                            rec.Y *= -1 * multiplier;
                            rec.X += xOffset;
                            rec.Y += yOffset;
                            rec.Width *= multiplier;
                            rec.Height *= multiplier;
                            break;
                    }
                }
            }
            return returnValue;
        }

        private static GDIShape GetNShapePoints(GDIShape cartesian, Size canvaseSize, float multiplier)
        {
            GDIShape c = (GDIShape)cartesian.Clone();
            int Width = canvaseSize.Width;
            int Height = canvaseSize.Height;
            for (int i2 = 0; i2 < c.Segments.Count; i2++)
            {
                if (c.Segments[i2].IsValidPathSegment)
                {
                    switch (c.Segments[i2].SegmentType)
                    {
                        case GDIPathSegmentTypeName.Arc:
                            Arc arc = (Arc)((Arc)c.Segments[i2]);
                            arc.X = arc.X * multiplier;
                            arc.Y = arc.Y * -1 * multiplier;
                            arc.Width = arc.Width * multiplier;
                            arc.Height = arc.Height * multiplier;
                            if (arc.StartAngle > 180)
                            {
                                arc.StartAngle -= 180;
                            }
                            else
                            {
                                arc.StartAngle += 180;
                            }
                            break;
                        case GDIPathSegmentTypeName.CubicBezier:
                            CubicBezier cub = (CubicBezier)((CubicBezier)c.Segments[i2]);
                            cub.StartingPointX = cub.StartingPointX * multiplier;
                            cub.StartingPointY = cub.StartingPointY * -1 * multiplier;
                            cub.ControlPointOneX = cub.ControlPointOneX * multiplier;
                            cub.ControlPointOneY = cub.ControlPointOneY * -1 * multiplier;
                            cub.ControlPointTwoX = cub.ControlPointTwoX * multiplier;
                            cub.ControlPointTwoY = cub.ControlPointTwoY * -1 * multiplier;
                            cub.EndingPointX = cub.EndingPointX * multiplier;
                            cub.EndingPointY = cub.EndingPointY * -1 * multiplier;
                            break;
                        case GDIPathSegmentTypeName.Lines:
                            PointF[] gdiLinePoints = (PointF[])((Lines)c.Segments[i2]).Points;
                            for (int i3 = 0; i3 < gdiLinePoints.Length; i3++)
                            {
                                gdiLinePoints[i3].X *= multiplier;
                                gdiLinePoints[i3].Y *= -1 * multiplier;
                            }
                            break;
                        case GDIPathSegmentTypeName.Polygon:
                            PointF[] gdipolyPoints = (PointF[])((Polygon)c.Segments[i2]).Points;
                            for (int i3 = 0; i3 < gdipolyPoints.Length; i3++)
                            {
                                gdipolyPoints[i3].X *= multiplier;
                                gdipolyPoints[i3].Y *= -1 * multiplier;
                            }
                            break;
                        case GDIPathSegmentTypeName.Rectangle:
                            Rectangle rec = (Rectangle)((Rectangle)c.Segments[i2]);
                            rec.X *= multiplier;
                            rec.Y *= -1 * multiplier;
                            rec.Width *= multiplier;
                            rec.Height *= multiplier;
                            break;
                    }
                }
            }
            return c;
        }

        private static GDIShape GetCartesianPoints(GDIShape cartesian, Size canvaseSize, float multiplier)
        {
            GDIShape c = (GDIShape)cartesian.Clone();
            int Width = canvaseSize.Width;
            int Height = canvaseSize.Height;
            for (int i2 = 0; i2 < c.Segments.Count; i2++)
            {
                if (c.Segments[i2].IsValidPathSegment)
                {
                    switch (c.Segments[i2].SegmentType)
                    {
                        case GDIPathSegmentTypeName.Arc:
                            Arc arc = (Arc)((Arc)c.Segments[i2]);
                            arc.X = arc.X * multiplier;
                            arc.Y = arc.Y * multiplier;
                            arc.Width = arc.Width * multiplier;
                            arc.Height = arc.Height * multiplier;
                            break;
                        case GDIPathSegmentTypeName.CubicBezier:
                            CubicBezier cub = (CubicBezier)((CubicBezier)c.Segments[i2]);
                            cub.StartingPointX = cub.StartingPointX * multiplier;
                            cub.StartingPointY = cub.StartingPointY * multiplier;
                            cub.ControlPointOneX = cub.ControlPointOneX * multiplier;
                            cub.ControlPointOneY = cub.ControlPointOneY * multiplier;
                            cub.ControlPointTwoX = cub.ControlPointTwoX * multiplier;
                            cub.ControlPointTwoY = cub.ControlPointTwoY * multiplier;
                            cub.EndingPointX = cub.EndingPointX * multiplier;
                            cub.EndingPointY = cub.EndingPointY * multiplier;
                            break;
                        case GDIPathSegmentTypeName.Lines:
                            PointF[] gdiLinePoints = (PointF[])((Lines)c.Segments[i2]).Points;
                            for (int i3 = 0; i3 < gdiLinePoints.Length; i3++)
                            {
                                gdiLinePoints[i3].X *= multiplier;
                                gdiLinePoints[i3].Y *= multiplier;
                            }
                            break;
                        case GDIPathSegmentTypeName.Polygon:
                            PointF[] gdipolyPoints = (PointF[])((Polygon)c.Segments[i2]).Points;
                            for (int i3 = 0; i3 < gdipolyPoints.Length; i3++)
                            {
                                gdipolyPoints[i3].X *= multiplier;
                                gdipolyPoints[i3].Y *= multiplier;
                            }
                            break;
                        case GDIPathSegmentTypeName.Rectangle:
                            Rectangle rec = (Rectangle)((Rectangle)c.Segments[i2]);
                            rec.X *= multiplier;
                            rec.Y *= multiplier;
                            rec.Width *= multiplier;
                            rec.Height *= multiplier;
                            break;
                    }
                }
            }
            return c;
        }

        public Object Clone()
        {
            GDIShape me = new GDIShape();
            me.XAxisGridMultiple = XAxisGridMultiple;
            me.YAxisGridMultiple = YAxisGridMultiple;
            me.OutPutPointType = OutPutPointType;
            me.ImageSizeMultiple = ImageSizeMultiple;
            me.DisplaySizeMultiple = DisplaySizeMultiple;
            me.PointSizeMultiple = PointSizeMultiple;
            me.Name = Name;
            me.Width = Width;
            me.Height = Height;
            me.PenColor = PenColor;
            me.PenWidth = PenWidth;
            me.FillColor = FillColor;
            me.Segments = (GDIPathSegmentCollection)Segments.Clone();
            return (Object)me;
        }

        [Category("Path")]
        [Description("Collection of segments that define this path")]
        [Editor(typeof(CustomCollectionEditor), typeof(UITypeEditor))]
        public GDIPathSegmentCollection Segments { get; set; }

    }

}
