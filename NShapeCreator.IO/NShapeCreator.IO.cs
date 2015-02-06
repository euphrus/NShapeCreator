using System;
using System.Drawing.Text;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using NShapeCreator.GDI;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using System.Drawing.Drawing2D;
using NShapeCreator.UI;
using System.Drawing.Imaging;

namespace NShapeCreator.IO
{

    internal static class NFile
    {

        public static void SaveFiles(GDIShape gdiShape, Size canvasSize)
        {
            //string initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string initialDirectory = @"C:\Repo\GitHub\NShape Source Files\ElectricalConnectors";
            string fileName = gdiShape.Name.Replace(" ", "");

            Action<string> save = (s) =>
            {
                string directory = Path.GetDirectoryName(s);
                string baseFileName = Path.GetFileNameWithoutExtension(s);
                SaveGDI(gdiShape, Path.Combine(Path.Combine(directory, "GDI"), baseFileName + ".gdi"));
                float resourceImageMultiple = 1;
                int maxSize = 60;
                if (((gdiShape.Width / maxSize * resourceImageMultiple) < 1
                    || (gdiShape.Height / maxSize * resourceImageMultiple) < 1))
                {
                    while ((gdiShape.Width / maxSize * resourceImageMultiple) < 1
                        || (gdiShape.Height / maxSize * resourceImageMultiple) < 1)
                    {
                        resourceImageMultiple += .01F;
                    }
                    while ((gdiShape.Width / maxSize * resourceImageMultiple) > 1
                        || (gdiShape.Height / maxSize * resourceImageMultiple) > 1)
                    {
                        resourceImageMultiple -= .01F;
                    }
                }
                SaveResourceImage(gdiShape, Path.Combine(Path.Combine(directory, "Resources"), baseFileName + ".bmp"), (int)Math.Ceiling(gdiShape.Width), (int)Math.Ceiling(gdiShape.Height), ImageFormat.Bmp, resourceImageMultiple);
                SaveTxt(Path.Combine(Path.Combine(directory, "Connectors"), baseFileName + ".cs"), DotNet.GetShapeClass(canvasSize, gdiShape, gdiShape.PointSizeMultiple));
                SaveTxt(Path.Combine(Path.Combine(directory, "Connectors"), "RectangleFBase.cs"), DotNet.GetRectangleFBaseClass(canvasSize, gdiShape, gdiShape.PointSizeMultiple));
            };
            SaveFileDialog(initialDirectory, fileName, "gdi", NFile.GDIFileDialogExtFilter, save);
        }

        #region File saving methods

        private static void SaveFileDialog(string directory, string fileName, string extention, string extentionFilter, Action<string> fileNameToWriteTo)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = directory;
            saveFileDialog.FileName = fileName.Replace(" ", "");
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = extention;
            saveFileDialog.Filter = extentionFilter;
            saveFileDialog.FilterIndex = 1;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileNameToWriteTo(saveFileDialog.FileName);
            }
        }

        private static void SaveResourceImage(GDIShape gdiShape, string fileName, int width, int height, ImageFormat imageFormat, float outputMultiple = 1)
        {
            string directory = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (Bitmap bmp = new Bitmap((int)Math.Ceiling((width * outputMultiple) + 2), (int)Math.Ceiling((height * outputMultiple) + 2)))
            {
                bmp.SetResolution(300, 300);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    NGraphics.DrawGDIPaths(g, bmp.Size, gdiShape, outputMultiple);
                }
                bmp.Save(fileName, imageFormat);
            }
        }

        private static void SaveTxt(string fileName, string fileContent)
        {
            string directory = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(fileContent);
                    sw.Flush();
                }
            }
        }

        private static void SaveGDI(GDIShape gdiShape, string fileName)
        {
            string directory = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(GDIShape));
                serializer.Serialize(fs, gdiShape);
            }
        }

        #endregion File saving methods

        #region DialogBox extention filters

        private static string PNGFileDialogExtFilter
        {
            get
            {
                return "png files (*.png)|*.png";
            }
        }

        private static string GDIFileDialogExtFilter
        {
            get
            {
                return "gdi files (*.gdi)|*.gdi";
            }
        }

        private static string BMPFileDialogExtFilter
        {
            get
            {
                return "bmp files (*.bmp)|*.bmp";
            }
        }

        private static string CSharpFileDialogExtFilter
        {
            get
            {
                return "cs files (*.cs)|*.cs";
            }
        }

        #endregion DialogBox extention filters

    }

    internal static class DotNet
    {
        private const string tab1 = "\t";
        private const string tab2 = "\t\t";
        private const string tab3 = "\t\t\t";
        private const string tab4 = "\t\t\t\t";
        private const string tab5 = "\t\t\t\t\t";
        private const string tab6 = "\t\t\t\t\t\t";
        private const string tab7 = "\t\t\t\t\t\t\t";
        private const string tab8 = "\t\t\t\t\t\t\t\t";

        public static string GetShapeClass(Size canvasSize, GDIShape gdiShape, float multiplier)
        {
            StringBuilder sb = new StringBuilder();
            if (gdiShape != null)
            {
                int segmentIndex = 0;
                string libraryName = "ElectricalConnectors";
                string className = gdiShape.Name.Replace(" ", "");
                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Drawing;");
                sb.AppendLine("using System.Drawing.Drawing2D;");
                sb.AppendLine("using Dataweb.NShape;");
                sb.AppendLine("using Dataweb.NShape.Advanced;");
                sb.AppendLine("");
                sb.AppendLine("namespace Windstream.Shapes.NShape.Electrical.Connectors");
                sb.AppendLine("{");
                sb.AppendLine("");
                sb.AppendLine(tab1 + "public class " + className + " : RectangleFBase");
                sb.AppendLine(tab1 + "{");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "#region Constructors");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "protected internal " + className + "(ShapeType shapeType, Template template)");
                sb.AppendLine(tab3 + ": base(shapeType, template)");
                sb.AppendLine(tab2 + "{");
                sb.AppendLine(tab3 + "Width = " + gdiShape.Width + "F;");
                sb.AppendLine(tab3 + "Height = " + gdiShape.Height + "F;");
                sb.AppendLine(tab2 + "}");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "protected internal " + className + "(ShapeType shapeType, IStyleSet styleSet)");
                sb.AppendLine(tab3 + ": base(shapeType, styleSet)");
                sb.AppendLine(tab2 + "{");
                sb.AppendLine(tab3 + "Width = " + gdiShape.Width + "F;");
                sb.AppendLine(tab3 + "Height = " + gdiShape.Height + "F;");
                sb.AppendLine(tab2 + "}");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "#endregion Constructors");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "#region Required Methods");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "public override Shape Clone()");
                sb.AppendLine(tab2 + "{");
                sb.AppendLine(tab3 + "Shape result = new " + className + "(Type, Template);");
                sb.AppendLine(tab3 + "result.CopyFrom(this);");
                sb.AppendLine(tab3 + "return result;");
                sb.AppendLine(tab2 + "}");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "protected override bool CalculatePath()");
                sb.AppendLine(tab2 + "{");
                sb.AppendLine(tab3 + "if (base.CalculatePath())");
                sb.AppendLine(tab3 + "{");
                sb.AppendLine(tab4 + "Path.Reset();");
                string pathName = null;
                sb.AppendLine(tab4 + "Path.Reset();");
                if (String.IsNullOrEmpty(gdiShape.Name))
                {
                    pathName = "Path";
                }
                else
                {
                    pathName = gdiShape.Name.Replace(" ", "");
                }
                for (segmentIndex = 0; segmentIndex < gdiShape.Segments.Count; segmentIndex++)
                {
                    if (gdiShape.Segments[segmentIndex].IsValidPathSegment)
                    {
                        if (gdiShape.Segments[segmentIndex].StartFigure)
                        {
                            sb.AppendLine(tab4 + "Path.StartFigure();");
                        }
                        switch (gdiShape.Segments[segmentIndex].SegmentType)
                        {
                            case GDIPathSegmentTypeName.Arc:
                                Arc arc = (Arc)gdiShape.Segments[segmentIndex];
                                if (arc.StartAngle > 180)
                                {
                                    arc.StartAngle -= 180;
                                }
                                else
                                {
                                    arc.StartAngle += 180;
                                }
                                sb.AppendLine(tab4 + "Path.AddArc(" + arc.X + "F * Scale * _pixelToMmMultiple, " + arc.Y + "F * -1 * Scale * _pixelToMmMultiple, " + arc.Width + "F * Scale * _pixelToMmMultiple, " + arc.Height + "F * Scale * _pixelToMmMultiple, " + arc.StartAngle + "F, " + arc.SweepAngle + "F);");
                                break;
                            case GDIPathSegmentTypeName.CubicBezier:
                                CubicBezier cub = (CubicBezier)gdiShape.Segments[segmentIndex];
                                sb.AppendLine(tab4 + "Path.AddBezier(" + cub.StartingPointX + "F * Scale * _pixelToMmMultiple, " + cub.StartingPointY + "F * -1 * Scale * _pixelToMmMultiple, " + cub.ControlPointOneX + "F * Scale * _pixelToMmMultiple, " + cub.ControlPointOneY + "F * -1 * Scale * _pixelToMmMultiple, " + cub.ControlPointTwoX + "F * Scale * _pixelToMmMultiple, " + cub.ControlPointTwoY + "F * -1 * Scale * _pixelToMmMultiple, " + cub.EndingPointX + "F * Scale * _pixelToMmMultiple, " + cub.EndingPointY + "F * -1 * Scale * _pixelToMmMultiple);");
                                break;
                            case GDIPathSegmentTypeName.Lines:
                                Lines lin = (Lines)gdiShape.Segments[segmentIndex];

                                sb.Append(tab4 + "Path.AddLines(new PointF[]{");
                                for (int p = 0; p < lin.Points.Length; p++)
                                {
                                    sb.Append("new PointF(" + lin.Points[p].X + "F * Scale * _pixelToMmMultiple, " + lin.Points[p].Y + "F * -1 * Scale * _pixelToMmMultiple)");
                                    if (p + 1 < lin.Points.Length)
                                    {
                                        sb.Append(", ");
                                    }
                                }
                                sb.AppendLine("});");
                                break;
                            case GDIPathSegmentTypeName.Polygon:
                                Polygon pol = (Polygon)gdiShape.Segments[segmentIndex];
                                sb.AppendLine(tab4 + "Path.AddPolygon(new PointF[]");
                                sb.AppendLine(tab4 + "{");
                                for (int p = 0; p < pol.Points.Length; p++)
                                {
                                    sb.Append(tab5 + "new PointF(" + pol.Points[p].X + "F * Scale * _pixelToMmMultiple, " + pol.Points[p].Y + "F * -1 * Scale * _pixelToMmMultiple)");
                                    if ((p + 1) < pol.Points.Length)
                                    {
                                        sb.Append(", ");
                                    }
                                    sb.AppendLine();
                                }
                                sb.AppendLine(tab4 + "});");
                                break;
                            case GDIPathSegmentTypeName.Rectangle:
                                GDI.Rectangle rec = (GDI.Rectangle)gdiShape.Segments[segmentIndex];
                                sb.AppendLine(tab4 + "Path.AddRectangle(new RectangleF(" + rec.X + "F * Scale * _pixelToMmMultiple, " + rec.Y + "F * -1 * Scale * _pixelToMmMultiple, " + rec.Width + "F * Scale * _pixelToMmMultiple, " + rec.Height + "F * Scale * _pixelToMmMultiple));");
                                break;
                        }

                        if (gdiShape.Segments[segmentIndex].CloseFigure)
                        {
                            sb.AppendLine(tab4 + "Path.CloseFigure();");
                        }
                    }
                }
                sb.AppendLine(tab4 + "return true;");
                sb.AppendLine(tab3 + "}");
                sb.AppendLine(tab3 + "return false;");
                sb.AppendLine(tab2 + "}");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "public static ShapeType GetShapeType()");
                sb.AppendLine(tab2 + "{");
                sb.AppendLine(tab3 + "string libraryName = \"" + libraryName + "\";");
                sb.AppendLine(tab3 + "return new ShapeType(\"" + className + "\", libraryName, libraryName,");
                sb.AppendLine(tab5 + "delegate(ShapeType s, Template t) { return new " + className + "(s, t); },");
                sb.AppendLine(tab3 + "RectangleFBase.GetPropertyDefinitions, Windstream.Shapes.NShape.Electrical.Connectors.Properties.Resources." + className + ");");
                sb.AppendLine(tab2 + "}");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "protected override void InitializeToDefault(IStyleSet styleSet)");
                sb.AppendLine(tab2 + "{");
                sb.AppendLine(tab3 + "base.InitializeToDefault(styleSet);");
                sb.AppendLine(tab3 + "Width = " + gdiShape.Width + "F;");
                sb.AppendLine(tab3 + "Height = " + gdiShape.Height + "F;");
                sb.AppendLine(tab2 + "}");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "#endregion Required Methods");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "#region ControlPoints");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "#region Required Sub-Class");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "new public class ControlPointIds");
                sb.AppendLine(tab2 + "{");
                sb.AppendLine(tab3 + "public const int Reference = ControlPointId.Reference;");
                sb.AppendLine(tab3 + "public const int MiddleCenter = 1;");
                sb.AppendLine(tab3 + "public const int " + className + "MiddleCenter = 2;");
                sb.AppendLine(tab2 + "}");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "#endregion Required Sub-Class");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "protected override void CalcControlPoints()");
                sb.AppendLine(tab2 + "{");
                sb.AppendLine(tab3 + "ControlPoints[ControlPointIds.Reference] = Point.Empty;");
                sb.AppendLine(tab3 + "ControlPoints[ControlPointIds.MiddleCenter] = Point.Empty;");
                sb.AppendLine(tab3 + "ControlPoints[ControlPointIds." + className + "MiddleCenter] = Point.Empty;");
                sb.AppendLine(tab2 + "}");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "public override IEnumerable<ControlPointId> GetControlPointIds(ControlPointCapabilities controlPointCapability)");
                sb.AppendLine(tab2 + "{");
                sb.AppendLine(tab3 + "List<ControlPointId> returnValue = new List<ControlPointId>();");
                sb.AppendLine(tab3 + "if ((controlPointCapability & ControlPointCapabilities.Rotate) != 0)");
                sb.AppendLine(tab3 + "{");
                sb.AppendLine(tab4 + "returnValue.Add(ControlPointIds.MiddleCenter);");
                sb.AppendLine(tab4 + "returnValue.Add(ControlPointIds.Reference);");
                sb.AppendLine(tab3 + "}");
                sb.AppendLine(tab3 + "if ((controlPointCapability & ControlPointCapabilities.Connect) != 0)");
                sb.AppendLine(tab3 + "{");
                sb.AppendLine(tab4 + "returnValue.Add(ControlPointIds." + className + "MiddleCenter);");
                sb.AppendLine(tab3 + "}");
                sb.AppendLine(tab3 + "return returnValue;");
                sb.AppendLine(tab2 + "}");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "public override Point GetControlPointPosition(ControlPointId controlPointId)");
                sb.AppendLine(tab2 + "{");
                sb.AppendLine(tab3 + "return ControlPoints[controlPointId];");
                sb.AppendLine(tab2 + "}");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability)");
                sb.AppendLine(tab2 + "{");
                sb.AppendLine(tab3 + "bool returnValue = false;");
                sb.AppendLine(tab3 + "switch (controlPointId)");
                sb.AppendLine(tab3 + "{");
                sb.AppendLine(tab4 + "case ControlPointIds.MiddleCenter:");
                sb.AppendLine(tab4 + "case ControlPointId.Reference:");
                sb.AppendLine(tab5 + "returnValue = (controlPointCapability & ControlPointCapabilities.Rotate) != 0;");
                sb.AppendLine(tab5 + "break;");
                sb.AppendLine(tab4 + "case ControlPointIds." + className + "MiddleCenter:");
                sb.AppendLine(tab5 + "returnValue = (controlPointCapability & ControlPointCapabilities.Connect) != 0;");
                sb.AppendLine(tab5 + "break;");
                sb.AppendLine(tab3 + "}");
                sb.AppendLine(tab3 + "return returnValue;");
                sb.AppendLine(tab2 + "}");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "private const int _controlPointCount = 3;");
                sb.AppendLine(tab2 + "protected override int ControlPointCount");
                sb.AppendLine(tab2 + "{");
                sb.AppendLine(tab3 + "get");
                sb.AppendLine(tab3 + "{");
                sb.AppendLine(tab4 + "return _controlPointCount;");
                sb.AppendLine(tab3 + "}");
                sb.AppendLine(tab2 + "}");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "#endregion ControlPoints");
                sb.AppendLine("");
                sb.AppendLine(tab1 + "}");
                sb.AppendLine("}");
            }
            return sb.ToString();
        }

        public static string GetRectangleFBaseClass(Size canvasSize, GDIShape gdiShape, float multiplier)
        {
            string className = "RectangleFBase";
            string nameSpace = "Windstream.Shapes.NShape.Electrical.Connectors";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.ComponentModel;");
            sb.AppendLine("using System.Drawing;");
            sb.AppendLine("using Dataweb.NShape;");
            sb.AppendLine("using Dataweb.NShape.Advanced;");
            sb.AppendLine("");
            sb.AppendLine("namespace " + nameSpace);
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine(tab1 + "public abstract class " + className + " : RectangleBase");
            sb.AppendLine(tab1 + "{");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "#region Fields");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "private float _width;");
            sb.AppendLine(tab2 + "private float _height;");
            sb.AppendLine(tab2 + "protected const float _pixelToMmMultiple = 3.779528F;");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "#endregion Fields");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "#region Constructors");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "protected internal " + className + "(ShapeType shapeType, Template template)");
            sb.AppendLine(tab3 + ": base(shapeType, template)");
            sb.AppendLine(tab2 + "{");
            sb.AppendLine(tab3 + "Scale = 1F;");
            sb.AppendLine(tab3 + "Width = 10F;");
            sb.AppendLine(tab3 + "Height = 10F;");
            sb.AppendLine(tab2 + "}");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "protected internal " + className + "(ShapeType shapeType, IStyleSet styleSet)");
            sb.AppendLine(tab3 + ": base(shapeType, styleSet)");
            sb.AppendLine(tab2 + "{");
            sb.AppendLine(tab3 + "Scale = 1F;");
            sb.AppendLine(tab3 + "Width = 10F;");
            sb.AppendLine(tab3 + "Height = 10F;");
            sb.AppendLine(tab2 + "}");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "#endregion Constructors");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "#region Required Methods");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "protected override void InitializeToDefault(IStyleSet styleSet)");
            sb.AppendLine(tab2 + "{");
            sb.AppendLine(tab3 + "base.InitializeToDefault(styleSet);");
            sb.AppendLine(tab3 + "Width = 10F;");
            sb.AppendLine(tab3 + "Height = 10F;");
            sb.AppendLine(tab2 + "}");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "#endregion Required Methods");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "#region Properties");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "[Browsable(false)]");
            sb.AppendLine(tab2 + "public new float Width");
            sb.AppendLine(tab2 + "{");
            sb.AppendLine(tab3 + "get");
            sb.AppendLine(tab3 + "{");
            sb.AppendLine(tab4 + "return _width * Scale * _pixelToMmMultiple;");
            sb.AppendLine(tab3 + "}");
            sb.AppendLine(tab3 + "set");
            sb.AppendLine(tab3 + "{");
            sb.AppendLine(tab4 + "_width = value;");
            sb.AppendLine(tab4 + "base.Width = (int)Math.Ceiling(_width * Scale * _pixelToMmMultiple);");
            sb.AppendLine(tab3 + "}");
            sb.AppendLine(tab2 + "}");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "[Browsable(false)]");
            sb.AppendLine(tab2 + "public new float Height");
            sb.AppendLine(tab2 + "{");
            sb.AppendLine(tab3 + "get");
            sb.AppendLine(tab3 + "{");
            sb.AppendLine(tab4 + "return _height * Scale * _pixelToMmMultiple;");
            sb.AppendLine(tab3 + "}");
            sb.AppendLine(tab3 + "set");
            sb.AppendLine(tab3 + "{");
            sb.AppendLine(tab4 + "_height = value;");
            sb.AppendLine(tab4 + "base.Height = (int)Math.Ceiling(_height * Scale * _pixelToMmMultiple);");
            sb.AppendLine(tab3 + "}");
            sb.AppendLine(tab2 + "}");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "private float _scale;");
            sb.AppendLine(tab2 + "[Browsable(true)]");
            sb.AppendLine(tab2 + "[Category(\"Layout\")]");
            sb.AppendLine(tab2 + "public float Scale");
            sb.AppendLine(tab2 + "{");
            sb.AppendLine(tab3 + "get");
            sb.AppendLine(tab3 + "{");
            sb.AppendLine(tab4 + "return _scale;");
            sb.AppendLine(tab3 + "}");
            sb.AppendLine(tab3 + "set");
            sb.AppendLine(tab3 + "{");
            sb.AppendLine(tab4 + "if (value > 0)");
            sb.AppendLine(tab4 + "{");
            sb.AppendLine(tab5 + "_scale = value;");
            sb.AppendLine(tab5 + "base.Width = (int)Math.Ceiling(_width * Scale * _pixelToMmMultiple);");
            sb.AppendLine(tab5 + "base.Height = (int)Math.Ceiling(_height * Scale * _pixelToMmMultiple);");
            sb.AppendLine(tab4 + "}");
            sb.AppendLine(tab3 + "}");
            sb.AppendLine(tab2 + "}");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "#endregion Properties");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "protected static void AddChildShape(PathBasedPlanarShape parentShape, PathBasedPlanarShape childShape, string colorName, Color baseColor, Color secondaryColor)");
            sb.AppendLine(tab2 + "{");
            sb.AppendLine(tab3 + "childShape.FillStyle = new FillStyle(colorName,");
            sb.AppendLine(tab4 + "new ColorStyle(baseColor.Name, baseColor),");
            sb.AppendLine(tab4 + "new ColorStyle(secondaryColor.Name, secondaryColor));");
            sb.AppendLine(tab3 + "if (!parentShape.Children.Contains(childShape))");
            sb.AppendLine(tab3 + "{");
            sb.AppendLine(tab4 + "parentShape.Children.Add(childShape);");
            sb.AppendLine(tab3 + "}");
            sb.AppendLine(tab2 + "}");
            sb.AppendLine("");
            sb.AppendLine(tab1 + "}");
            sb.AppendLine("}");
            return sb.ToString();
        }

    }

}