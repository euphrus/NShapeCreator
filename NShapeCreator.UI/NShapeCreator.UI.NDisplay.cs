using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using NShapeCreator.GDI;
using System.Collections.Generic;
using NShapeCreator.Message;
using NShapeCreator.IO;

namespace NShapeCreator.UI
{
    public partial class NDisplay : MessageForm
    {

        private GDIShape _GDI;

        public override void HandleMessage(Object sender, MessageEventArgs e)
        {
            try
            {
                Action<MessageEventArgs> action = (actionE) =>
                {
                    if (actionE.Messages.ContainsKey(MessageID.FormClosed))
                    {
                        Close();
                    }
                    if (actionE.Messages.ContainsKey(MessageID.GDIShapeUpdated))
                    {
                        _GDI = (GDIShape)actionE.Messages[MessageID.GDIShapeUpdated];
                        Invalidate();
                    }
                    if (actionE.Messages.ContainsKey(MessageID.SaveFiles))
                    {
                        _GDI = (GDIShape)actionE.Messages[MessageID.SaveFiles];
                        Invalidate();
                        NFile.SaveFiles(_GDI, Size);
                    }
                };
                if (InvokeRequired)
                {
                    BeginInvoke(action, e);
                }
                else
                {
                    action(e);
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex.ToString());
                throw;
            }
        }

        void OnPaint(Object sender, PaintEventArgs e)
        {
            try
            {
                NGraphics.ClearCanvas(e.Graphics);
                NGraphics.DrawGrid(e.Graphics, Size, _GDI, _GDI.DisplaySizeMultiple);
                NGraphics.DrawBoundingBox(e.Graphics, Size, _GDI, _GDI.DisplaySizeMultiple);
                NGraphics.DrawGDIPaths(e.Graphics, Size, _GDI, _GDI.DisplaySizeMultiple);
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex.ToString());
                throw;
            }
        }

        public NDisplay()
        {
            try
            {
                _GDI = new GDIShape();
                InitializeComponent();
                ResizeRedraw = true;
                Paint += new PaintEventHandler(OnPaint);
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex.ToString());
                throw;
            }
        }

    }
}
