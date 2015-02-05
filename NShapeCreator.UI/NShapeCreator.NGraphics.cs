using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using NShapeCreator.GDI;
using System.Drawing.Drawing2D;

namespace NShapeCreator
{
    internal static class NGraphics
    {

        public static void DrawBoundingBox(Graphics g, Size canvasSize, GDIShape gdiShape, int multiplier)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                float xAxis = canvasSize.Width / 2;
                float yAxis = canvasSize.Height / 2;

                float x = xAxis - gdiShape.Width * multiplier / 2;
                float y = yAxis - gdiShape.Height * multiplier / 2;

                path.AddRectangle(new RectangleF(x, y, gdiShape.Width * multiplier, gdiShape.Height * multiplier));
                PaintGraphicsPath(g, path, Color.Red, .1f, Color.Transparent);
            }
        }

        public static void DrawGrid(Graphics g, Size canvasSize, GDIShape gdiShape, int multiplier)
        {

            using (GraphicsPath path = new GraphicsPath())
            {
                float xAxis = canvasSize.Width / 2;
                float yAxis = canvasSize.Height / 2;

                if (gdiShape.XAxisGridMultiple > 0)
                {
                    for (float i = xAxis; i > 0; i -= (gdiShape.XAxisGridMultiple * multiplier))
                    {
                        path.StartFigure();
                        path.AddLine(new PointF(i, 0), new PointF(i, canvasSize.Height));
                        path.CloseFigure();
                    }

                    for (float i = xAxis; i < canvasSize.Width; i += (gdiShape.XAxisGridMultiple * multiplier))
                    {
                        path.StartFigure();
                        path.AddLine(new PointF(i, 0), new PointF(i, canvasSize.Height));
                        path.CloseFigure();
                    }
                }

                if (gdiShape.YAxisGridMultiple > 0)
                {
                    for (float i = yAxis; i > 0; i -= (gdiShape.YAxisGridMultiple * multiplier))
                    {
                        path.StartFigure();
                        path.AddLine(new PointF(0, i), new PointF(canvasSize.Width, i));
                        path.CloseFigure();
                    }

                    for (float i = yAxis; i < canvasSize.Height; i += (gdiShape.YAxisGridMultiple * multiplier))
                    {
                        path.StartFigure();
                        path.AddLine(new PointF(0, i), new PointF(canvasSize.Width, i));
                        path.CloseFigure();
                    }
                }

                PaintGraphicsPath(g, path, Color.LightGray, .001f, Color.White);

                path.Reset();

                if (gdiShape.XAxisGridMultiple > 0)
                {
                    path.StartFigure();
                    path.AddLine(new PointF(xAxis, 0), new PointF(xAxis, canvasSize.Height));
                    path.CloseFigure();
                }
                if (gdiShape.YAxisGridMultiple > 0)
                {
                    path.StartFigure();
                    path.AddLine(new PointF(0, yAxis), new PointF(canvasSize.Width, yAxis));
                    path.CloseFigure();
                }
                PaintGraphicsPath(g, path, Color.Black, .001f, Color.White);
            }
        }

        public static void ClearCanvas(Graphics g)
        {
            g.Clear(Color.White);
        }

        public static void DrawGDIPaths(Graphics g, Size canvasSize, GDIShape gdiShape, int multiplier)
        {
            if (gdiShape != null)
            {
                GDIShape drawingPath = GDIShape.GetPoints(gdiShape, PointTypeID.GDI, canvasSize, multiplier);
                    using (GraphicsPath drawPath = GetGraphicsPath(drawingPath))
                    {
                        if (drawPath.PointCount > 0)
                        {
                            PaintGraphicsPath(g, drawPath, drawingPath.PenColor, drawingPath.PenWidth, drawingPath.FillColor);
                        }
                    }
            }
        }

        public static GraphicsPath GetGraphicsPath(GDIShape gdiPath)
        {
            GraphicsPath path = new GraphicsPath();
            for (int i2 = 0; i2 < gdiPath.Segments.Count; i2++)
            {
                if (gdiPath.Segments[i2].IsValidPathSegment)
                {
                    if (gdiPath.Segments[i2].StartFigure)
                    {
                        path.StartFigure();
                    }
                    switch (gdiPath.Segments[i2].SegmentType)
                    {
                        case GDIPathSegmentTypeName.Arc:
                            Arc arc = (Arc)((Arc)gdiPath.Segments[i2]);
                            path.AddArc(arc.X, arc.Y, arc.Width, arc.Height, arc.StartAngle, arc.SweepAngle);
                            break;
                        case GDIPathSegmentTypeName.CubicBezier:
                            CubicBezier cub = (CubicBezier)((CubicBezier)gdiPath.Segments[i2]);
                            path.AddBezier(
                                cub.StartingPointX,
                                cub.StartingPointY,
                                cub.ControlPointOneX,
                                cub.ControlPointOneY,
                                cub.ControlPointTwoX,
                                cub.ControlPointTwoY,
                                cub.EndingPointX,
                                cub.EndingPointY
                            );
                            break;
                        case GDIPathSegmentTypeName.Lines:
                            PointF[] gdiLinePoints = (PointF[])((Lines)gdiPath.Segments[i2]).Points;
                            path.AddLines(gdiLinePoints);
                            break;
                        case GDIPathSegmentTypeName.Polygon:
                            PointF[] gdipolyPoints = (PointF[])((Polygon)gdiPath.Segments[i2]).Points;
                            path.AddPolygon(gdipolyPoints);
                            break;
                        case GDIPathSegmentTypeName.Rectangle:
                            System.Drawing.RectangleF rectangle = (System.Drawing.RectangleF)((GDI.Rectangle)gdiPath.Segments[i2]).GDIRectangleF;
                            path.AddRectangle(rectangle);
                            break;
                    }
                    if (gdiPath.Segments[i2].CloseFigure)
                    {
                        path.CloseFigure();
                    }
                }
            }
            return path;
        }

        public static void PaintGraphicsPath(Graphics g, GraphicsPath path, Color linecolor, Single penWidth, Color fillColor)
        {
            using (Pen pen = new Pen(linecolor, penWidth))
            {
                g.FillPath(new SolidBrush(fillColor), path);
                g.DrawPath(pen, path);
            }
        }

    }
}
