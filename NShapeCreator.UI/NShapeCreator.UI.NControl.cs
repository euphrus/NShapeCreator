using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using NShapeCreator.GDI;
using NShapeCreator.Message;
using System.IO;
using System.Xml.Serialization;

namespace NShapeCreator.UI
{

    public partial class NControl : MessageForm
    {

        #region Fields

        private const string _AddSegmentToSelectedGDIPath = "Add Segment To Selected GDIPath";
        private const string _InsertSegmentBeforeSelectedSegment = "Insert Segment Before Selected Segment";
        private GDIShape _GDI;
        private int? _selectedGDIPathSegmentIndex;

        #endregion Fields

        #region Methods

        private void SendGDIShapeUpdatedMessage()
        {
            try
            {
                MessageEventArgs e = new MessageEventArgs();
                e.Messages[MessageID.GDIShapeUpdated] = _GDI.Clone();
                SendMessage(e);
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex.ToString());
                throw;
            }
        }

        private void PaintGDIPathTree()
        {
            treeGdiPath.BeginUpdate();
            treeGdiPath.Nodes.Clear();
            if (_GDI != null)
            {
                if (String.IsNullOrEmpty(_GDI.Name))
                {
                    treeGdiPath.Nodes.Add("Shape");
                }
                else
                {
                    treeGdiPath.Nodes.Add(_GDI.Name);
                }
                if (_GDI.Segments != null)
                {
                    for (int i2 = 0; i2 < _GDI.Segments.Count; i2++)
                    {
                        if (String.IsNullOrEmpty(_GDI.Segments[i2].Name))
                        {
                            treeGdiPath.Nodes[0].Nodes.Add("Segment " + (i2 + 1) + ": " + _GDI.Segments[i2].SegmentType);
                        }
                        else
                        {
                            treeGdiPath.Nodes[0].Nodes.Add(_GDI.Segments[i2].Name);
                        }
                    }
                }
                if (_selectedGDIPathSegmentIndex.HasValue)
                {
                    treeGdiPath.SelectedNode = treeGdiPath.Nodes[0].Nodes[_selectedGDIPathSegmentIndex.Value];
                }
                else
                {
                    treeGdiPath.SelectedNode = treeGdiPath.Nodes[0];
                }
                treeGdiPath.ExpandAll();
            }
            treeGdiPath.EndUpdate();
        }

        private bool GDIPathSegmentTypeChanged()
        {
            try
            {
                bool returnValue = false;
                if (_selectedGDIPathSegmentIndex.HasValue)
                {
                    returnValue = Enum.GetName(typeof(GDIPathSegmentTypeName), _GDI.Segments[_selectedGDIPathSegmentIndex.Value].SegmentType) !=
                        _GDI.Segments[_selectedGDIPathSegmentIndex.Value].GetType().Name;
                }
                return returnValue;
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex.ToString());
                throw;
            }
        }

        private void GDIPathSegmentTypeHasChanged()
        {
            GDIPathSegmentTypeName segmentName = _GDI.Segments[_selectedGDIPathSegmentIndex.Value].SegmentType;
            _GDI.Segments[_selectedGDIPathSegmentIndex.Value] = GetGDIPathSegmentInstance(segmentName);
            switch (segmentName)
            {
                case GDIPathSegmentTypeName.Arc:
                    gdiControlGrid.SelectedObject = (Arc)_GDI.Segments[_selectedGDIPathSegmentIndex.Value];
                    break;
                case GDIPathSegmentTypeName.CubicBezier:
                    gdiControlGrid.SelectedObject = (CubicBezier)_GDI.Segments[_selectedGDIPathSegmentIndex.Value];
                    break;
                case GDIPathSegmentTypeName.Lines:
                    gdiControlGrid.SelectedObject = (Lines)_GDI.Segments[_selectedGDIPathSegmentIndex.Value];
                    break;
                case GDIPathSegmentTypeName.Polygon:
                    gdiControlGrid.SelectedObject = (Polygon)_GDI.Segments[_selectedGDIPathSegmentIndex.Value];
                    break;
            }
        }

        private static GDIPathSegment GetGDIPathSegmentInstance(GDIPathSegmentTypeName segmentName)
        {
            string typeName = "NShapeCreator.GDI." + Enum.GetName(typeof(GDIPathSegmentTypeName), segmentName);
            return (GDIPathSegment)Assembly.GetExecutingAssembly().CreateInstance(typeName);
        }

        #endregion

        #region Event Handlers

        #region Message Handler

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

        #endregion

        #region CustomCollectionEditor

        private void CustomCollectionEditor_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            SendGDIShapeUpdatedMessage();
        }

        private void CustomCollectionEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            PaintGDIPathTree();
        }

        #endregion

        #region treeGdiPath

        private void treeGdiPath_BeforeSelect(Object sender, TreeViewCancelEventArgs e)
        {
            try
            {
                if (treeGdiPath.SelectedNode != null)
                {
                    treeGdiPath.BeginUpdate();
                    treeGdiPath.SelectedNode.BackColor = Color.White;
                    treeGdiPath.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex.ToString());
                throw;
            }
        }

        private void treeGdiPath_AfterSelect(Object sender, TreeViewEventArgs e)
        {
            try
            {
                if (treeGdiPath.SelectedNode.Parent == null)
                {
                    _selectedGDIPathSegmentIndex = null;
                    btnAddSegmentToGDIPath.Text = _AddSegmentToSelectedGDIPath;
                    gdiControlGrid.SelectedObject = _GDI;
                }
                else
                {
                    if (treeGdiPath.SelectedNode.Parent != null)
                    {
                        _selectedGDIPathSegmentIndex = treeGdiPath.SelectedNode.Index;
                        btnAddSegmentToGDIPath.Text = _InsertSegmentBeforeSelectedSegment;
                        gdiControlGrid.SelectedObject = _GDI.Segments[_selectedGDIPathSegmentIndex.Value];
                    }
                    else
                    {
                        _selectedGDIPathSegmentIndex = null;
                        btnAddSegmentToGDIPath.Text = _AddSegmentToSelectedGDIPath;
                        gdiControlGrid.SelectedObject = _GDI;
                    }
                }
                treeGdiPath.BeginUpdate();
                treeGdiPath.SelectedNode.BackColor = Color.LightGray;
                treeGdiPath.EndUpdate();
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex.ToString());
                throw;
            }
        }

        #endregion

        #region gdiControlGrid

        private void gdiControlGrid_PropertyValueChanged(Object sender, PropertyValueChangedEventArgs e)
        {
            try
            {
                if (GDIPathSegmentTypeChanged())
                {
                    GDIPathSegmentTypeHasChanged();
                }
                SendGDIShapeUpdatedMessage();
                PaintGDIPathTree();
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex.ToString());
                throw;
            }
        }

        private void gdiControlGrid_SelectedObjectsChanged(Object sender, EventArgs e)
        {
            try
            {
                if (GDIPathSegmentTypeChanged())
                {
                    GDIPathSegmentTypeHasChanged();
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex.ToString());
                throw;
            }
        }

        private void gdiControlGrid_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!String.IsNullOrEmpty(gdiControlGrid.ActiveControl.AccessibleName))
            {
                float val;
                System.Windows.Forms.Control c = gdiControlGrid.ActiveControl;
                switch (gdiControlGrid.ActiveControl.AccessibleName.Replace("\t", ""))
                {
                    case "X":
                    case "Y":
                    case "Start Angle":
                    case "Sweep Angle":
                    case "Starting X":
                    case "Starting Y":
                    case "Control Point One X":
                    case "Control Point One Y":
                    case "Control Point Two X":
                    case "Control Point Two Y":
                    case "Ending X":
                    case "Ending Y":
                        if (float.TryParse(gdiControlGrid.ActiveControl.Text, out val))
                        {
                            if (e.Delta > 0)
                            {
                                val += .1f;
                            }
                            else
                            {
                                val -= .1f;
                            }
                            c.AccessibilityObject.Value = val.ToString();
                            gdiControlGrid.Refresh();
                            c.Select();
                        }
                        break;
                    case "X-Axis Grid Multiple":
                    case "Y-Axis Grid Multiple":
                    case "Pen Width":
                    case "Width":
                    case "Height":
                        if (float.TryParse(gdiControlGrid.ActiveControl.Text, out val))
                        {
                            if (e.Delta > 0)
                            {
                                val += .1f;
                            }
                            else
                            {
                                if (val > 1)
                                {
                                    val -= .1f;
                                }
                            }
                            c.AccessibilityObject.Value = val.ToString();
                            gdiControlGrid.Refresh();
                            c.Select();
                        }
                        break;
                    case "PNG Size Multiple":
                    case "Display Size Multiple":
                    case "Point Size Multiple":
                        if (float.TryParse(gdiControlGrid.ActiveControl.Text, out val))
                        {
                            if (e.Delta > 0)
                            {
                                val += 1;
                            }
                            else
                            {
                                if (val > 1)
                                {
                                    val -= 1;
                                }
                            }
                            c.AccessibilityObject.Value = val.ToString();
                            gdiControlGrid.Refresh();
                            c.Select();
                        }
                        break;
                    case "Point Index":
                        if (float.TryParse(gdiControlGrid.ActiveControl.Text, out val))
                        {
                            if (e.Delta > 0)
                            {
                                val += 1;
                            }
                            else
                            {
                                if (val >= 1)
                                {
                                    val -= 1;
                                }
                            }
                            c.AccessibilityObject.Value = val.ToString();
                            gdiControlGrid.Refresh();
                            c.Select();
                        }
                        break;
                }
            }
        }

        #endregion

        #region btn (buttons)

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                //fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                fileDialog.InitialDirectory = @"C:\Users\n9948482\Desktop\Dev\STEMS\CurrentWork\NShape Source Files\ElectricalConnectors\GDI";
                fileDialog.DefaultExt = "gdi";
                fileDialog.Filter = "gdi files (*.gdi)|*.gdi";
                fileDialog.FilterIndex = 1;
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream fs = new FileStream(fileDialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(GDIShape));
                        _GDI = (GDIShape)serializer.Deserialize(fs);
                        PaintGDIPathTree();
                        SendGDIShapeUpdatedMessage();
                    }
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex.ToString());
                throw;
            }
        }

        private void btnSaveToFile_Click(Object sender, EventArgs e)
        {
            try
            {
                MessageEventArgs E = new MessageEventArgs();
                E.Messages[MessageID.SaveFiles] = _GDI.Clone();
                SendMessage(E);
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex.ToString());
                throw;
            }
        }

        private void btnAddSegmentToSelectedGDIPath_Click(Object sender, EventArgs e)
        {
            try
            {
                    if (_selectedGDIPathSegmentIndex.HasValue)
                    {
                        _GDI.Segments.InsertAt(_selectedGDIPathSegmentIndex.Value, new GDIPathSegment());
                    }
                    else
                    {
                        _GDI.Segments.Add(new GDIPathSegment());
                        _selectedGDIPathSegmentIndex = _GDI.Segments.Count - 1;
                    }
                    PaintGDIPathTree();
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex.ToString());
                throw;
            }
        }

        private void btnRemoveSelectedGDIPathSegment_Click(Object sender, EventArgs e)
        {
            try
            {
                if (_selectedGDIPathSegmentIndex.HasValue)
                {
                    _GDI.Segments.RemoveAt(_selectedGDIPathSegmentIndex.Value);
                    if (_GDI.Segments.Count == 0)
                    {
                        _selectedGDIPathSegmentIndex = null;
                    }
                    else
                    {
                        _selectedGDIPathSegmentIndex = _GDI.Segments.Count - 1;
                    }
                    PaintGDIPathTree();
                    SendGDIShapeUpdatedMessage();
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex.ToString());
                throw;
            }
        }

        #endregion

        #endregion Event Handlers

        #region Constructor(s)

        public NControl()
        {
            InitializeComponent();

            treeGdiPath.BeforeSelect += new TreeViewCancelEventHandler(treeGdiPath_BeforeSelect);
            treeGdiPath.AfterSelect += new TreeViewEventHandler(treeGdiPath_AfterSelect);

            gdiControlGrid.SelectedObjectsChanged += new EventHandler(gdiControlGrid_SelectedObjectsChanged);
            gdiControlGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(gdiControlGrid_PropertyValueChanged);
            gdiControlGrid.MouseWheel += new MouseEventHandler(gdiControlGrid_MouseWheel);

            CustomCollectionEditor.CustomCollectionEditorFormClosed += new CustomCollectionEditor.CustomCollectionEditorFormClosedEventHandler(CustomCollectionEditor_FormClosed);
            CustomCollectionEditor.CustomCollectionEditorPropertyValueChanged += new CustomCollectionEditor.CustomCollectionEditorPropertyValueChangedEventHandler(CustomCollectionEditor_PropertyValueChanged);

            _GDI = new GDIShape();
            gdiControlGrid.SelectedObject = _GDI;
            PaintGDIPathTree();
        }

        #endregion Constructor(s)

    }

}
