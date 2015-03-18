using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using NShapeCreator.UI;

namespace NShapeCreator.IO
{

    internal static class NFile
    {

        public static void SaveFiles(NShape nshape, Size canvasSize)
        {
            //string initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string initialDirectory = @"C:\Repo\GitHub\NShape Source Files\ElectricalConnectors";
            string fileName = nshape.Name.Replace(" ", "");

            Action<string> save = (s) =>
            {
                string directory = Path.GetDirectoryName(s);
                string baseFileName = Path.GetFileNameWithoutExtension(s);
                SaveGDI(nshape, Path.Combine(Path.Combine(directory, "GDI"), baseFileName + ".gdi"));
                float resourceImageMultiple = 1;
                int maxSize = 60;
                if (((nshape.Paths[0].WidthF / maxSize * resourceImageMultiple) < 1
                    || (nshape.Paths[0].HeightF / maxSize * resourceImageMultiple) < 1))
                {
                    while ((nshape.Paths[0].WidthF / maxSize * resourceImageMultiple) < 1
                        || (nshape.Paths[0].HeightF / maxSize * resourceImageMultiple) < 1)
                    {
                        resourceImageMultiple += .01F;
                    }
                    while ((nshape.Paths[0].WidthF / maxSize * resourceImageMultiple) > 1
                        || (nshape.Paths[0].HeightF / maxSize * resourceImageMultiple) > 1)
                    {
                        resourceImageMultiple -= .01F;
                    }
                }
                SaveResourceImage(nshape, nshape.Paths[0], Path.Combine(Path.Combine(directory, "Resources"), baseFileName + ".bmp"), (int)Math.Ceiling(nshape.Paths[0].WidthF), (int)Math.Ceiling(nshape.Paths[0].HeightF), ImageFormat.Bmp, resourceImageMultiple);
                SaveTxt(Path.Combine(Path.Combine(directory, "Shapes"), baseFileName + ".cs"), DotNet.GetShapeClass(canvasSize, nshape));
                SaveTxt(Path.Combine(directory, "Initializer.cs"), DotNet.GetNShapeLibraryInitializerClass(canvasSize, nshape));
                SaveTxt(Path.Combine(Path.Combine(directory, "Shapes"), "RectangleFBase.cs"), DotNet.GetRectangleFBaseClass(canvasSize, nshape));
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

        private static void SaveResourceImage(NShape gdiShape, NPath npath, string fileName, int width, int height, ImageFormat imageFormat, float outputMultiple = 1)
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
                    NGraphics.DrawGDIPaths(g, bmp.Size, gdiShape, npath, outputMultiple);
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

        private static void SaveGDI(NShape gdiShape, string fileName)
        {
            string directory = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(NShape));
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

        public static string GetShapeClass(Size canvasSize, NShape nshape)
        {
            StringBuilder sb = new StringBuilder();
            if (nshape != null)
            {
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Drawing;");
                sb.AppendLine("using Dataweb.NShape;");
                sb.AppendLine("using Dataweb.NShape.Advanced;");
                sb.AppendLine("");
                sb.AppendLine("namespace " + nshape.Namespace);
                sb.AppendLine("{");
                sb.AppendLine("");
                sb.AppendLine(tab1 + "public class " + nshape.ClassName + " : RectangleFBase");
                sb.AppendLine(tab1 + "{");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "public enum MemberShape");
                sb.AppendLine(tab2 + "{");
                for (int i1 = 0; i1 < nshape.Paths.Count; i1++)
                {
                    sb.Append(tab3 + nshape.Paths[i1].Name);
                    if (i1 != nshape.Paths.Count - 1)
                    {
                        sb.AppendLine(",");
                    }
                    else
                    {
                        sb.AppendLine("");
                    }
                }
                sb.AppendLine(tab2 + "}");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "private MemberShape _member;");
                sb.AppendLine(tab2 + "public MemberShape Member");
                sb.AppendLine(tab2 + "{");
                sb.AppendLine(tab3 + "get");
                sb.AppendLine(tab3 + "{");
                sb.AppendLine(tab4 + "return _member;");
                sb.AppendLine(tab3 + "}");
                sb.AppendLine(tab3 + "set");
                sb.AppendLine(tab3 + "{");
                sb.AppendLine(tab4 + "if (_member != value)");
                sb.AppendLine(tab4 + "{");
                sb.AppendLine(tab5 + "_member = value;");
                sb.AppendLine(tab5 + "Invalidate();");
                sb.AppendLine(tab5 + "InvalidateDrawCache();");
                sb.AppendLine(tab4 + "}");
                sb.AppendLine(tab3 + "}");
                sb.AppendLine(tab2 + "}");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "#region Constructors");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "protected internal " + nshape.ClassName + "(ShapeType shapeType, Template template)");
                sb.AppendLine(tab3 + ": base(shapeType, template)");
                sb.AppendLine(tab2 + "{");
                sb.AppendLine(tab3 + "WidthF = " + nshape.Paths[0].WidthF + "F;");
                sb.AppendLine(tab3 + "HeightF = " + nshape.Paths[0].HeightF + "F;");
                sb.AppendLine(tab2 + "}");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "protected internal " + nshape.ClassName + "(ShapeType shapeType, IStyleSet styleSet)");
                sb.AppendLine(tab3 + ": base(shapeType, styleSet)");
                sb.AppendLine(tab2 + "{");
                sb.AppendLine(tab3 + "WidthF = " + nshape.Paths[0].WidthF + "F;");
                sb.AppendLine(tab3 + "HeightF = " + nshape.Paths[0].HeightF + "F;");
                sb.AppendLine(tab2 + "}");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "#endregion Constructors");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "#region Required Methods");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "public override Shape Clone()");
                sb.AppendLine(tab2 + "{");
                sb.AppendLine(tab3 + "Shape result = new " + nshape.ClassName + "(Type, Template);");
                sb.AppendLine(tab3 + "result.CopyFrom(this);");
                sb.AppendLine(tab3 + "return result;");
                sb.AppendLine(tab2 + "}");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "protected override bool CalculatePath()");
                sb.AppendLine(tab2 + "{");
                sb.AppendLine(tab3 + "if (base.CalculatePath())");
                sb.AppendLine(tab3 + "{");
                sb.AppendLine(tab4 + "Path.Reset();");
                sb.AppendLine(tab4 + "switch (Member)");
                sb.AppendLine(tab4 + "{");
                for (int i1 = 0; i1 < nshape.Paths.Count; i1++)
                {
                    sb.AppendLine(tab5 + "case MemberShape." + nshape.Paths[i1].Name + ":");
                    sb.AppendLine(tab6 + nshape.Paths[i1].Name + "Path();");
                    sb.AppendLine(tab6 + "break;");
                }
                sb.AppendLine(tab4 + "}");
                sb.AppendLine(tab4 + "return true;");
                sb.AppendLine(tab3 + "}");
                sb.AppendLine(tab3 + "return false;");
                sb.AppendLine(tab2 + "}");
                sb.AppendLine("");
                for (int i1 = 0; i1 < nshape.Paths.Count; i1++)
                {
                    sb.AppendLine(tab2 + "private void " + nshape.Paths[i1].Name + "Path()");
                    sb.AppendLine(tab2 + "{");
                    sb.AppendLine(tab3 + "WidthF = " + nshape.Paths[i1].WidthF + "F;");
                    sb.AppendLine(tab3 + "HeightF = " + nshape.Paths[i1].HeightF + "F;");
                    for (int i2 = 0; i2 < nshape.Paths[i1].Segments.Count; i2++)
                    {
                        if (nshape.Paths[i1].Segments[i2].IsValidPathSegment)
                        {
                            if (nshape.Paths[i1].Segments[i2].StartFigure)
                            {
                                sb.AppendLine(tab3 + "Path.StartFigure();");
                            }
                            switch (nshape.Paths[i1].Segments[i2].SegmentType)
                            {
                                case GDIPathSegmentTypeName.Arc:
                                    Arc arc = (Arc)nshape.Paths[i1].Segments[i2];
                                    if (arc.StartAngle > 180)
                                    {
                                        arc.StartAngle -= 180;
                                    }
                                    else
                                    {
                                        arc.StartAngle += 180;
                                    }
                                    sb.AppendLine(tab3 + "Path.AddArc(" + arc.X + "F * Scale * _pixelToMmMultiple, " + arc.Y + "F * -1 * Scale * _pixelToMmMultiple, " + arc.Width + "F * Scale * _pixelToMmMultiple, " + arc.Height + "F * Scale * _pixelToMmMultiple, " + arc.StartAngle + "F, " + arc.SweepAngle + "F);");
                                    break;
                                case GDIPathSegmentTypeName.CubicBezier:
                                    CubicBezier cub = (CubicBezier)nshape.Paths[i1].Segments[i2];
                                    sb.AppendLine(tab3 + "Path.AddBezier(" + cub.StartingPointX + "F * Scale * _pixelToMmMultiple, " + cub.StartingPointY + "F * -1 * Scale * _pixelToMmMultiple, " + cub.ControlPointOneX + "F * Scale * _pixelToMmMultiple, " + cub.ControlPointOneY + "F * -1 * Scale * _pixelToMmMultiple, " + cub.ControlPointTwoX + "F * Scale * _pixelToMmMultiple, " + cub.ControlPointTwoY + "F * -1 * Scale * _pixelToMmMultiple, " + cub.EndingPointX + "F * Scale * _pixelToMmMultiple, " + cub.EndingPointY + "F * -1 * Scale * _pixelToMmMultiple);");
                                    break;
                                case GDIPathSegmentTypeName.Lines:
                                    Lines lin = (Lines)nshape.Paths[i1].Segments[i2];

                                    sb.Append(tab3 + "Path.AddLines(new PointF[]{");
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
                                    Polygon pol = (Polygon)nshape.Paths[i1].Segments[i2];
                                    sb.AppendLine(tab3 + "Path.AddPolygon(new PointF[]");
                                    sb.AppendLine(tab3 + "{");
                                    for (int p = 0; p < pol.Points.Length; p++)
                                    {
                                        sb.Append(tab5 + "new PointF(" + pol.Points[p].X + "F * Scale * _pixelToMmMultiple, " + pol.Points[p].Y + "F * -1 * Scale * _pixelToMmMultiple)");
                                        if ((p + 1) < pol.Points.Length)
                                        {
                                            sb.Append(", ");
                                        }
                                        sb.AppendLine();
                                    }
                                    sb.AppendLine(tab3 + "});");
                                    break;
                                case GDIPathSegmentTypeName.Rectangle:
                                    NShapeCreator.Rectangle rec = (NShapeCreator.Rectangle)nshape.Paths[i1].Segments[i2];
                                    sb.AppendLine(tab3 + "Path.AddRectangle(new RectangleF(" + rec.X + "F * Scale * _pixelToMmMultiple, " + rec.Y + "F * -1 * Scale * _pixelToMmMultiple, " + rec.Width + "F * Scale * _pixelToMmMultiple, " + rec.Height + "F * Scale * _pixelToMmMultiple));");
                                    break;
                            }

                            if (nshape.Paths[i1].Segments[i2].CloseFigure)
                            {
                                sb.AppendLine(tab3 + "Path.CloseFigure();");
                            }
                        }
                    }
                    sb.AppendLine(tab2 + "}");
                    sb.AppendLine("");
                }
                sb.AppendLine(tab2 + "public static ShapeType GetShapeType()");
                sb.AppendLine(tab2 + "{");
                sb.AppendLine(tab3 + "string libraryName = \"" + nshape.LibraryName + "\";");
                sb.AppendLine(tab3 + "return new ShapeType(\"" + nshape.ClassName + "\", libraryName, libraryName,");
                sb.AppendLine(tab5 + "delegate(ShapeType s, Template t) { return new " + nshape.ClassName + "(s, t); },");
                sb.AppendLine(tab3 + nshape.ClassName + ".GetPropertyDefinitions, " + nshape.Namespace + ".Properties.Resources." + nshape.ClassName + ");");
                sb.AppendLine(tab2 + "}");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "protected override void InitializeToDefault(IStyleSet styleSet)");
                sb.AppendLine(tab2 + "{");
                sb.AppendLine(tab3 + "base.InitializeToDefault(styleSet);");
                sb.AppendLine(tab3 + "WidthF = " + nshape.Paths[0].WidthF + "F;");
                sb.AppendLine(tab3 + "HeightF = " + nshape.Paths[0].HeightF + "F;");
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
                sb.AppendLine(tab3 + "public const int " + nshape.ClassName + "MiddleCenter = 2;");
                sb.AppendLine(tab2 + "}");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "#endregion Required Sub-Class");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "protected override void CalcControlPoints()");
                sb.AppendLine(tab2 + "{");
                sb.AppendLine(tab3 + "ControlPoints[ControlPointIds.Reference] = Point.Empty;");
                sb.AppendLine(tab3 + "ControlPoints[ControlPointIds.MiddleCenter] = Point.Empty;");
                sb.AppendLine(tab3 + "ControlPoints[ControlPointIds." + nshape.ClassName + "MiddleCenter] = Point.Empty;");
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
                sb.AppendLine(tab4 + "returnValue.Add(ControlPointIds." + nshape.ClassName + "MiddleCenter);");
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
                sb.AppendLine(tab4 + "case ControlPointIds." + nshape.ClassName + "MiddleCenter:");
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
                for (int i1 = 0; i1 < nshape.Properties.Count; i1++)
                {
                    sb.AppendLine(tab2 + "public " + nshape.Properties[i1].Type + " " + nshape.Properties[i1].Name);
                    sb.AppendLine(tab2 + "{");
                    sb.AppendLine(tab3 + "get;set;");
                    sb.AppendLine(tab2 + "}");
                    sb.AppendLine("");
                }
                sb.AppendLine(tab2 + "new public static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version)");
                sb.AppendLine(tab2 + "{");
                sb.AppendLine(tab3 + "foreach (EntityPropertyDefinition pi in RectangleFBase.GetPropertyDefinitions(version))");
                sb.AppendLine(tab3 + "{");
                sb.AppendLine(tab4 + "yield return pi;");
                sb.AppendLine(tab3 + "}");
                for (int i1 = 0; i1 < nshape.Properties.Count; i1++)
                {
                    sb.AppendLine(tab3 + "yield return new EntityFieldDefinition(\"" + nshape.Properties[i1].Name + "\", typeof(" + nshape.Properties[i1].Type + "));");
                }
                sb.AppendLine(tab2 + "}");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "protected override void LoadFieldsCore(IRepositoryReader reader, int version)");
                sb.AppendLine(tab2 + "{");
                sb.AppendLine(tab3 + "base.LoadFieldsCore(reader, version);");
                for (int i1 = 0; i1 < nshape.Properties.Count; i1++)
                {
                    sb.AppendLine(tab3 + nshape.Properties[i1].Name + " = reader.Read" + nshape.Properties[i1].Type + "();");
                }
                sb.AppendLine(tab2 + "}");
                sb.AppendLine("");
                sb.AppendLine(tab2 + "protected override void SaveFieldsCore(IRepositoryWriter writer, int version)");
                sb.AppendLine(tab2 + "{");
                sb.AppendLine(tab3 + "base.SaveFieldsCore(writer, version);");
                for (int i1 = 0; i1 < nshape.Properties.Count; i1++)
                {
                    sb.AppendLine(tab3 + "writer.Write" + nshape.Properties[i1].Type + "(" + nshape.Properties[i1].Name + ");");
                }
                sb.AppendLine(tab2 + "}");
                sb.AppendLine("");
                sb.AppendLine(tab1 + "}");
                sb.Append("}");
            }
            return sb.ToString();
        }

        public static string GetNShapeLibraryInitializerClass(Size canvasSize, NShape nshape)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using Dataweb.NShape.Advanced;");
            sb.AppendLine("");
            sb.AppendLine("namespace " + nshape.Namespace);
            sb.AppendLine("{");
            sb.AppendLine(tab1 + "public static class NShapeLibraryInitializer");
            sb.AppendLine(tab1 + "{");
            sb.AppendLine(tab2 + "public static void Initialize(IRegistrar registrar)");
            sb.AppendLine(tab2 + "{");
            sb.AppendLine(tab3 + "registrar.RegisterLibrary(libraryName, repositoryVersion);");
            sb.AppendLine(tab3 + "registrar.RegisterShapeType(" + nshape.ClassName + ".GetShapeType());");
            sb.AppendLine(tab2 + "}");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "private const string libraryName = \"" + nshape.LibraryName + "\";");
            sb.AppendLine(tab2 + "private const int repositoryVersion = 1;");
            sb.AppendLine(tab1 + "}");
            sb.Append("}");
            return sb.ToString();
        }

        public static string GetRectangleFBaseClass(Size canvasSize, NShape gdiShape)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.ComponentModel;");
            sb.AppendLine("using Dataweb.NShape;");
            sb.AppendLine("using Dataweb.NShape.Advanced;");
            sb.AppendLine("");
            sb.AppendLine("namespace " + gdiShape.Namespace);
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine(tab1 + "public abstract class RectangleFBase : RectangleBase");
            sb.AppendLine(tab1 + "{");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "#region Fields");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "private float _scale;");
            sb.AppendLine(tab2 + "private float _widthF;");
            sb.AppendLine(tab2 + "private float _heightF;");
            sb.AppendLine(tab2 + "protected const float _pixelToMmMultiple = 3.779528F;");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "#endregion Fields");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "#region Constructors");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "protected internal RectangleFBase(ShapeType shapeType, Template template)");
            sb.AppendLine(tab3 + ": base(shapeType, template)");
            sb.AppendLine(tab2 + "{");
            sb.AppendLine(tab2 + "}");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "protected internal RectangleFBase(ShapeType shapeType, IStyleSet styleSet)");
            sb.AppendLine(tab3 + ": base(shapeType, styleSet)");
            sb.AppendLine(tab2 + "{");
            sb.AppendLine(tab2 + "}");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "#endregion Constructors");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "#region Required Methods");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "protected override void InitializeToDefault(IStyleSet styleSet)");
            sb.AppendLine(tab2 + "{");
            sb.AppendLine(tab3 + "base.InitializeToDefault(styleSet);");
            sb.AppendLine(tab3 + "WidthF = 10F;");
            sb.AppendLine(tab3 + "HeightF = 10F;");
            sb.AppendLine(tab2 + "}");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "#endregion Required Methods");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "#region Properties");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "[Browsable(false)]");
            sb.AppendLine(tab2 + "public float WidthF");
            sb.AppendLine(tab2 + "{");
            sb.AppendLine(tab3 + "get");
            sb.AppendLine(tab3 + "{");
            sb.AppendLine(tab4 + "return _widthF;");
            sb.AppendLine(tab3 + "}");
            sb.AppendLine(tab3 + "set");
            sb.AppendLine(tab3 + "{");
            sb.AppendLine(tab4 + "if (value < 0)");
            sb.AppendLine(tab4 + "{");
            sb.AppendLine(tab5 + "throw new ArgumentException(\"Value must be greater than zero\", \"Width\");");
            sb.AppendLine(tab4 + "}");
            sb.AppendLine(tab4 + "if (_widthF != value)");
            sb.AppendLine(tab4 + "{");
            sb.AppendLine(tab5 + "_widthF = value;");
            sb.AppendLine(tab5 + "Width = (int)Math.Ceiling(_widthF * _pixelToMmMultiple * Scale);");
            sb.AppendLine(tab4 + "}");
            sb.AppendLine(tab3 + "}");
            sb.AppendLine(tab2 + "}");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "[Browsable(false)]");
            sb.AppendLine(tab2 + "public float HeightF");
            sb.AppendLine(tab2 + "{");
            sb.AppendLine(tab3 + "get");
            sb.AppendLine(tab3 + "{");
            sb.AppendLine(tab4 + "return _heightF * Scale * _pixelToMmMultiple;");
            sb.AppendLine(tab3 + "}");
            sb.AppendLine(tab3 + "set");
            sb.AppendLine(tab3 + "{");
            sb.AppendLine(tab4 + "if (value < 0)");
            sb.AppendLine(tab4 + "{");
            sb.AppendLine(tab5 + "throw new ArgumentException(\"Value must be greater than zero\", \"Height\");");
            sb.AppendLine(tab4 + "}");
            sb.AppendLine(tab4 + "if (_heightF != value)");
            sb.AppendLine(tab4 + "{");
            sb.AppendLine(tab5 + "_heightF = value;");
            sb.AppendLine(tab5 + "Height = (int)Math.Ceiling(_heightF * _pixelToMmMultiple * Scale);");
            sb.AppendLine(tab4 + "}");
            sb.AppendLine(tab3 + "}");
            sb.AppendLine(tab2 + "}");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "[Browsable(false)]");
            sb.AppendLine(tab2 + "new public int Width");
            sb.AppendLine(tab2 + "{");
            sb.AppendLine(tab3 + "get");
            sb.AppendLine(tab3 + "{");
            sb.AppendLine(tab4 + "return base.Width;");
            sb.AppendLine(tab3 + "}");
            sb.AppendLine(tab3 + "set");
            sb.AppendLine(tab3 + "{");
            sb.AppendLine(tab4 + "base.Width = value;");
            sb.AppendLine(tab3 + "}");
            sb.AppendLine(tab2 + "}");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "[Browsable(false)]");
            sb.AppendLine(tab2 + "new public int Height");
            sb.AppendLine(tab2 + "{");
            sb.AppendLine(tab3 + "get");
            sb.AppendLine(tab3 + "{");
            sb.AppendLine(tab4 + "return base.Height;");
            sb.AppendLine(tab3 + "}");
            sb.AppendLine(tab3 + "set");
            sb.AppendLine(tab3 + "{");
            sb.AppendLine(tab4 + "base.Height = value;");
            sb.AppendLine(tab3 + "}");
            sb.AppendLine(tab2 + "}");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "[Category(\"Layout\")]");
            sb.AppendLine(tab2 + "[Browsable(true)]");
            sb.AppendLine(tab2 + "public float Scale");
            sb.AppendLine(tab2 + "{");
            sb.AppendLine(tab3 + "get");
            sb.AppendLine(tab3 + "{");
            sb.AppendLine(tab4 + "if (_scale <= 0)");
            sb.AppendLine(tab4 + "{");
            sb.AppendLine(tab5 + "_scale = 1F;");
            sb.AppendLine(tab4 + "}");
            sb.AppendLine(tab4 + "return _scale;");
            sb.AppendLine(tab3 + "}");
            sb.AppendLine(tab3 + "set");
            sb.AppendLine(tab3 + "{");
            sb.AppendLine(tab4 + "if (value > 0)");
            sb.AppendLine(tab4 + "{");
            sb.AppendLine(tab5 + "_scale = value;");
            sb.AppendLine(tab5 + "base.Width = (int)Math.Ceiling(_widthF * Scale * _pixelToMmMultiple);");
            sb.AppendLine(tab5 + "base.Height = (int)Math.Ceiling(_heightF * Scale * _pixelToMmMultiple);");
            sb.AppendLine(tab4 + "}");
            sb.AppendLine(tab3 + "}");
            sb.AppendLine(tab2 + "}");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "#endregion Properties");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "new public static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version)");
            sb.AppendLine(tab2 + "{");
            sb.AppendLine(tab3 + "foreach (EntityPropertyDefinition pi in RectangleBase.GetPropertyDefinitions(version))");
            sb.AppendLine(tab3 + "{");
            sb.AppendLine(tab4 + "yield return pi;");
            sb.AppendLine(tab3 + "}");
            sb.AppendLine(tab3 + "yield return new EntityFieldDefinition(\"Scale\", typeof(float));");
            sb.AppendLine(tab2 + "}");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "protected override void LoadFieldsCore(IRepositoryReader reader, int version)");
            sb.AppendLine(tab2 + "{");
            sb.AppendLine(tab3 + "base.LoadFieldsCore(reader, version);");
            sb.AppendLine(tab3 + "Scale = reader.ReadFloat();");
            sb.AppendLine(tab2 + "}");
            sb.AppendLine("");
            sb.AppendLine(tab2 + "protected override void SaveFieldsCore(IRepositoryWriter writer, int version)");
            sb.AppendLine(tab2 + "{");
            sb.AppendLine(tab3 + "base.SaveFieldsCore(writer, version);");
            sb.AppendLine(tab3 + "writer.WriteFloat(Scale);");
            sb.AppendLine(tab2 + "}");
            sb.AppendLine("");
            sb.AppendLine(tab1 + "}");
            sb.Append("}");
            return sb.ToString();
        }

    }

}