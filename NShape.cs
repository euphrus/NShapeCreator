using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Design;

namespace NShapeCreator
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

    public enum OutputPointTypeID
    {
        GDI,
        NShape,
    }

    #endregion

    [Serializable]
    public class NShape : ICloneable
    {

        #region Constructor

        public NShape()
        {
            Paths = new NPathCollection();
            Properties = new NPropertyCollection();
            XAxisGridMultiple = 1;
            YAxisGridMultiple = 1;
        }

        #endregion

        #region Category: Paths

        [Category("Paths")]
        [Description("Collection of paths that define this shape")]
        [Editor(typeof(CustomCollectionEditor), typeof(UITypeEditor))]
        public NPathCollection Paths { get; set; }

        #endregion

        #region Category: Shape Options

        private string _name;
        [Category("\t\tShape Options")]
        [Description("The name that will be assigned this shape")]
        [DisplayName("\tShape Name")]
        public string Name
        {
            get
            {
                if (String.IsNullOrEmpty(_name))
                {
                    _name = "NewShape";
                }
                return _name;
            }
            set
            {
                _name = value.Replace(" ", "");
            }
        }

        #endregion

        #region Category: Display Output Options

        private float _XAxisGridMultiple;
        [Category("\tDisplay Output Options")]
        [Description("This sets how often the X-Axis grid line is painted")]
        [DisplayName("X-Axis Grid Multiple")]
        public float XAxisGridMultiple
        {
            get { return _XAxisGridMultiple; }
            set { _XAxisGridMultiple = value; }
        }

        private float _YAxisGridMultiple;
        [Category("\tDisplay Output Options")]
        [Description("This sets how often the Y-Axis grid line is painted")]
        [DisplayName("Y-Axis Grid Multiple")]
        public float YAxisGridMultiple
        {
            get { return _YAxisGridMultiple; }
            set { _YAxisGridMultiple = value; }
        }

        #endregion

        #region Category: NShape Class Options

        private string _namespace;
        [Category("NShape Class Options")]
        [Description("The namespace to use for the generated Nshape class")]
        [DisplayName("Namespace")]
        public string Namespace
        {
            get
            {
                if (String.IsNullOrEmpty(_namespace))
                {
                    _namespace = "NShapeCreator.NShape";
                }
                return _namespace;
            }
            set
            {
                _namespace = value.Replace(" ", "");
                if (_namespace.EndsWith("."))
                {
                    _namespace = _namespace.Substring(0, _namespace.Length - 1);
                }
            }
        }

        private string _libraryName;
        [Category("NShape Class Options")]
        [Description("The library name used for initialization")]
        [DisplayName("Library Name")]
        public string LibraryName
        {
            get
            {
                if (String.IsNullOrEmpty(_libraryName))
                {
                    _libraryName = "NShapeCreatorLibraryName";
                }
                return _libraryName;
            }
            set
            {
                _libraryName = value.Replace(" ", "");
            }
        }

        [Category("NShape Class Options")]
        [Description("The class name (same as the shape name)")]
        [DisplayName("Class Name")]
        public string ClassName
        {
            get
            {
                return Name;
            }
        }

        [Category("NShape Class Options")]
        [Description("Persistable properties you would like to add to the shape")]
        [DisplayName("Persistable Properties")]
        public NPropertyCollection Properties { get; set; }

        #endregion

        public Object Clone()
        {
            NShape me = new NShape();
            me.LibraryName = LibraryName;
            me.Name = Name;
            me.Namespace = Namespace;
            me.Paths = (NPathCollection)Paths.Clone();
            me.XAxisGridMultiple = XAxisGridMultiple;
            me.YAxisGridMultiple = YAxisGridMultiple;
            me.Properties = Properties;
            return me;
        }

        #region Static Methods

        public static string SerializeColor(Color color)
        {
            return color.Name;
        }

        public static Color DeserializeColor(string colorName)
        {
            return Color.FromName(colorName);
        }

        public static NPath GetPoints(NShape cartesian, NPath npath, OutputPointTypeID returnPointType, Size canvasSize, float multiplier)
        {
            NPath returnValue = null;
            switch (returnPointType)
            {
                case OutputPointTypeID.GDI:
                    returnValue = GetGDIPoints(npath, canvasSize, multiplier);
                    break;
                case OutputPointTypeID.NShape:
                    returnValue = GetNShapePoints(npath, canvasSize, multiplier);
                    break;
            }
            return returnValue;
        }

        public static NPath GetGDIPoints(NPath npath, Size canvaseSize, float multiplier)
        {
            NPath gdiPath = (NPath)npath.Clone();
            float xOffset = canvaseSize.Width / 2f;
            float yOffset = canvaseSize.Height / 2f;
            for (int i2 = 0; i2 < gdiPath.Segments.Count; i2++)
            {
                if (gdiPath.Segments[i2].IsValidPathSegment)
                {
                    switch (gdiPath.Segments[i2].SegmentType)
                    {
                        case GDIPathSegmentTypeName.Arc:
                            Arc arc = (Arc)((Arc)gdiPath.Segments[i2]);
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
                            CubicBezier cub = (CubicBezier)((CubicBezier)gdiPath.Segments[i2]);
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
                            PointF[] gdiLinePoints = (PointF[])((Lines)gdiPath.Segments[i2]).Points;
                            for (int i3 = 0; i3 < gdiLinePoints.Length; i3++)
                            {
                                gdiLinePoints[i3].X *= multiplier;
                                gdiLinePoints[i3].Y *= -1 * multiplier;
                                gdiLinePoints[i3].X += xOffset;
                                gdiLinePoints[i3].Y += yOffset;
                            }
                            break;
                        case GDIPathSegmentTypeName.Polygon:
                            PointF[] gdipolyPoints = (PointF[])((Polygon)gdiPath.Segments[i2]).Points;
                            for (int i3 = 0; i3 < gdipolyPoints.Length; i3++)
                            {
                                gdipolyPoints[i3].X *= multiplier;
                                gdipolyPoints[i3].Y *= -1 * multiplier;
                                gdipolyPoints[i3].X += xOffset;
                                gdipolyPoints[i3].Y += yOffset;
                            }
                            break;
                        case GDIPathSegmentTypeName.Rectangle:
                            Rectangle rec = (Rectangle)((Rectangle)gdiPath.Segments[i2]);
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
            return gdiPath;
        }

        public static NPath GetNShapePoints(NPath npath, Size canvaseSize, float multiplier)
        {
            NPath nshapePath = (NPath)npath.Clone();
            int Width = canvaseSize.Width;
            int Height = canvaseSize.Height;
            for (int i2 = 0; i2 < nshapePath.Segments.Count; i2++)
            {
                if (nshapePath.Segments[i2].IsValidPathSegment)
                {
                    switch (nshapePath.Segments[i2].SegmentType)
                    {
                        case GDIPathSegmentTypeName.Arc:
                            Arc arc = (Arc)((Arc)nshapePath.Segments[i2]);
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
                            CubicBezier cub = (CubicBezier)((CubicBezier)nshapePath.Segments[i2]);
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
                            PointF[] gdiLinePoints = (PointF[])((Lines)nshapePath.Segments[i2]).Points;
                            for (int i3 = 0; i3 < gdiLinePoints.Length; i3++)
                            {
                                gdiLinePoints[i3].X *= multiplier;
                                gdiLinePoints[i3].Y *= -1 * multiplier;
                            }
                            break;
                        case GDIPathSegmentTypeName.Polygon:
                            PointF[] gdipolyPoints = (PointF[])((Polygon)nshapePath.Segments[i2]).Points;
                            for (int i3 = 0; i3 < gdipolyPoints.Length; i3++)
                            {
                                gdipolyPoints[i3].X *= multiplier;
                                gdipolyPoints[i3].Y *= -1 * multiplier;
                            }
                            break;
                        case GDIPathSegmentTypeName.Rectangle:
                            Rectangle rec = (Rectangle)((Rectangle)nshapePath.Segments[i2]);
                            rec.X *= multiplier;
                            rec.Y *= -1 * multiplier;
                            rec.Width *= multiplier;
                            rec.Height *= multiplier;
                            break;
                    }
                }
            }
            return nshapePath;
        }

        #endregion

    }

}