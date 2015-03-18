using System;
using System.Windows.Forms;
using NShapeCreator.IO;
using NShapeCreator.Message;

namespace NShapeCreator.UI
{
    public partial class NDisplay : MessageForm
    {

        private NShape _GDI;
        private int? _selectedPathIndex;

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
                        if (actionE.Messages.ContainsKey(MessageID.SelectedPathIndex))
                        {
                            _selectedPathIndex = (int?)actionE.Messages[MessageID.SelectedPathIndex];
                        }
                        _GDI = (NShape)actionE.Messages[MessageID.GDIShapeUpdated];
                        Invalidate();
                    }
                    if (actionE.Messages.ContainsKey(MessageID.SaveFiles))
                    {
                        _GDI = (NShape)actionE.Messages[MessageID.SaveFiles];
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
                if (_selectedPathIndex.HasValue)
                {
                    NGraphics.DrawGrid(e.Graphics, Size, _GDI, _GDI.Paths[_selectedPathIndex.Value].DisplaySizeMultiple);
                    NGraphics.DrawGDIPaths(e.Graphics, Size, _GDI, _GDI.Paths[_selectedPathIndex.Value], _GDI.Paths[_selectedPathIndex.Value].DisplaySizeMultiple);
                    if (_GDI.Paths[_selectedPathIndex.Value].DisplayBoundingBox)
                    {
                        NGraphics.DrawBoundingBox(e.Graphics, Size, _GDI, _GDI.Paths[_selectedPathIndex.Value], _GDI.Paths[_selectedPathIndex.Value].DisplaySizeMultiple);
                    }
                }
                else
                {
                    NGraphics.DrawGrid(e.Graphics, Size, _GDI, 40F);
                }
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
                _GDI = new NShape();
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
