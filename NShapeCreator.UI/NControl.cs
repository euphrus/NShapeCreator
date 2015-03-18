using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using NShapeCreator.Message;

namespace NShapeCreator.UI
{

    public partial class NControl : MessageForm
    {

        #region Fields

        private NShape _GDI;
        private int? _selectedPathIndex;
        private int? _selectedSegmentIndex;

        #endregion Fields

        #region Methods

        private void SendGDIShapeUpdatedMessage()
        {
            try
            {
                MessageEventArgs e = new MessageEventArgs();
                e.Messages[MessageID.GDIShapeUpdated] = _GDI.Clone();
                if (_selectedPathIndex.HasValue)
                {
                    e.Messages[MessageID.SelectedPathIndex] = _selectedPathIndex;
                }
                else if (_GDI.Paths.Count > 0)
                {
                    e.Messages[MessageID.SelectedPathIndex] = 0;
                }
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
                treeGdiPath.Nodes.Add(_GDI.Name);
                for (int i1 = 0; i1 < _GDI.Paths.Count; i1++)
                {
                    treeGdiPath.Nodes[0].Nodes.Add(_GDI.Paths[i1].Name);
                    for (int i2 = 0; i2 < _GDI.Paths[i1].Segments.Count; i2++)
                    {
                        treeGdiPath.Nodes[0].Nodes[i1].Nodes.Add("Segment " + (i2 + 1) + ": " + _GDI.Paths[i1].Segments[i2].SegmentType);
                    }
                }
                if (_selectedPathIndex.HasValue)
                {
                    if (_selectedSegmentIndex.HasValue)
                    {
                        treeGdiPath.SelectedNode = treeGdiPath.Nodes[0].Nodes[_selectedPathIndex.Value].Nodes[_selectedSegmentIndex.Value];
                    }
                    else
                    {
                        treeGdiPath.SelectedNode = treeGdiPath.Nodes[0].Nodes[_selectedPathIndex.Value];
                    }
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
                if (_selectedSegmentIndex.HasValue)
                {
                    returnValue = Enum.GetName(typeof(GDIPathSegmentTypeName), _GDI.Paths[_selectedPathIndex.Value].Segments[_selectedSegmentIndex.Value].SegmentType) !=
                        _GDI.Paths[_selectedPathIndex.Value].Segments[_selectedSegmentIndex.Value].GetType().Name;
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
            GDIPathSegmentTypeName segmentName = _GDI.Paths[_selectedPathIndex.Value].Segments[_selectedSegmentIndex.Value].SegmentType;
            _GDI.Paths[_selectedPathIndex.Value].Segments[_selectedSegmentIndex.Value] = GetGDIPathSegmentInstance(segmentName);
            switch (segmentName)
            {
                case GDIPathSegmentTypeName.Arc:
                    gdiControlGrid.SelectedObject = (Arc)_GDI.Paths[_selectedPathIndex.Value].Segments[_selectedSegmentIndex.Value];
                    break;
                case GDIPathSegmentTypeName.CubicBezier:
                    gdiControlGrid.SelectedObject = (CubicBezier)_GDI.Paths[_selectedPathIndex.Value].Segments[_selectedSegmentIndex.Value];
                    break;
                case GDIPathSegmentTypeName.Lines:
                    gdiControlGrid.SelectedObject = (Lines)_GDI.Paths[_selectedPathIndex.Value].Segments[_selectedSegmentIndex.Value];
                    break;
                case GDIPathSegmentTypeName.Polygon:
                    gdiControlGrid.SelectedObject = (Polygon)_GDI.Paths[_selectedPathIndex.Value].Segments[_selectedSegmentIndex.Value];
                    break;
            }
        }

        private static NSegment GetGDIPathSegmentInstance(GDIPathSegmentTypeName segmentName)
        {
            string typeName = "NShapeCreator." + Enum.GetName(typeof(GDIPathSegmentTypeName), segmentName);
            return (NSegment)Assembly.GetExecutingAssembly().CreateInstance(typeName);
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
                if (treeGdiPath.SelectedNode.Parent != null)
                {
                    if (treeGdiPath.SelectedNode.Parent.Parent != null)
                    {
                        _selectedPathIndex = treeGdiPath.SelectedNode.Parent.Index;
                        _selectedSegmentIndex = treeGdiPath.SelectedNode.Index;
                        gdiControlGrid.SelectedObject = _GDI.Paths[_selectedPathIndex.Value].Segments[_selectedSegmentIndex.Value];
                    }
                    else
                    {
                        _selectedPathIndex = treeGdiPath.SelectedNode.Index;
                        _selectedSegmentIndex = null;
                        gdiControlGrid.SelectedObject = _GDI.Paths[_selectedPathIndex.Value];
                    }
                }
                else
                {
                    _selectedPathIndex = null;
                    _selectedSegmentIndex = null;
                    gdiControlGrid.SelectedObject = _GDI;
                }
                treeGdiPath.BeginUpdate();
                treeGdiPath.SelectedNode.BackColor = Color.LightGray;
                treeGdiPath.EndUpdate();
                SendGDIShapeUpdatedMessage();
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
                fileDialog.InitialDirectory = @"C:\Repo\GitHub\NShape Source Files\ElectricalConnectors";
                fileDialog.DefaultExt = "gdi";
                fileDialog.Filter = "gdi files (*.gdi)|*.gdi";
                fileDialog.FilterIndex = 1;
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream fs = new FileStream(fileDialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(NShape));
                        _GDI = (NShape)serializer.Deserialize(fs);
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

        private void btnAddPath_Click(Object sender, EventArgs e)
        {
            try
            {
                if (_selectedPathIndex.HasValue)
                {
                    if (_selectedPathIndex.Value + 1 == _GDI.Paths.Count)
                    {
                        _GDI.Paths.Add(new NPath());
                        _selectedSegmentIndex = null;
                    }
                    else
                    {
                        _GDI.Paths.InsertAt(_selectedPathIndex.Value + 1, new NPath());
                    }
                    _selectedPathIndex += 1;
                }
                else
                {
                    _GDI.Paths.Add(new NPath());
                    if (_selectedPathIndex.HasValue)
                    {
                        _selectedPathIndex += 1;
                    }
                    else
                    {
                        _selectedPathIndex = 0;
                    }
                }
                PaintGDIPathTree();
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex.ToString());
                throw;
            }
        }

        private void btnImportPath_Click(Object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                //fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                fileDialog.InitialDirectory = @"C:\Repo\GitHub\NShape Source Files\ElectricalConnectors";
                fileDialog.DefaultExt = "gdi";
                fileDialog.Filter = "gdi files (*.gdi)|*.gdi";
                fileDialog.FilterIndex = 1;
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream fs = new FileStream(fileDialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(NShape));
                        NShape tmpGDI = (NShape)serializer.Deserialize(fs);
                        if (tmpGDI.Paths.Count > 0)
                        {
                            for (int i1 = 0; i1 < tmpGDI.Paths.Count; i1++)
                            {
                                _GDI.Paths.Add((NPath)tmpGDI.Paths[i1].Clone());
                            }
                            _selectedPathIndex = _GDI.Paths.Count - 1;
                            _selectedSegmentIndex = null;
                            PaintGDIPathTree();
                            SendGDIShapeUpdatedMessage();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex.ToString());
                throw;
            }
        }

        private void btnAddSegment_Click(Object sender, EventArgs e)
        {
            try
            {
                if (_selectedPathIndex.HasValue)
                {
                    if (_selectedSegmentIndex.HasValue)
                    {
                        if (_selectedSegmentIndex.Value + 1 == _GDI.Paths[_selectedPathIndex.Value].Segments.Count)
                        {
                            _GDI.Paths[_selectedPathIndex.Value].Segments.Add(new NSegment());
                        }
                        else
                        {
                            _GDI.Paths[_selectedPathIndex.Value].Segments.InsertAt(_selectedSegmentIndex.Value + 1, new NSegment());
                        }
                        _selectedSegmentIndex += 1;
                    }
                    else
                    {
                        _GDI.Paths[_selectedPathIndex.Value].Segments.Add(new NSegment());
                        _selectedSegmentIndex = _GDI.Paths[_selectedPathIndex.Value].Segments.Count - 1;
                    }
                    PaintGDIPathTree();
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex.ToString());
                throw;
            }
        }

        private void btnRemoveSelected_Click(Object sender, EventArgs e)
        {
            try
            {
                if (_selectedPathIndex.HasValue)
                {
                    if (_selectedSegmentIndex.HasValue)
                    {
                        _GDI.Paths[_selectedPathIndex.Value].Segments.RemoveAt(_selectedSegmentIndex.Value);
                        if (_selectedSegmentIndex.Value == 0)
                        {
                            _selectedSegmentIndex = null;
                        }
                        else
                        {
                            _selectedSegmentIndex--;
                        }
                    }
                    else
                    {
                        _GDI.Paths.RemoveAt(_selectedPathIndex.Value);
                        if (_selectedPathIndex.Value == 0)
                        {
                            _selectedPathIndex = null;
                            _selectedSegmentIndex = null;
                        }
                        else
                        {
                            _selectedPathIndex--;
                        }
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

            _GDI = new NShape();
            gdiControlGrid.SelectedObject = _GDI;
            PaintGDIPathTree();
        }

        #endregion Constructor(s)

    }

}
