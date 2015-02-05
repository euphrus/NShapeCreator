namespace NShapeCreator.UI
{
    partial class NControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gdiControlGrid = new System.Windows.Forms.PropertyGrid();
            this.treeGdiPath = new System.Windows.Forms.TreeView();
            this.btnAddSegmentToGDIPath = new System.Windows.Forms.Button();
            this.btnRemoveSegmentFromGDIPath = new System.Windows.Forms.Button();
            this.btnSaveToFile = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // gdiControlGrid
            // 
            this.gdiControlGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gdiControlGrid.Location = new System.Drawing.Point(280, 5);
            this.gdiControlGrid.Name = "gdiControlGrid";
            this.gdiControlGrid.Size = new System.Drawing.Size(476, 442);
            this.gdiControlGrid.TabIndex = 1;
            this.gdiControlGrid.ToolbarVisible = false;
            // 
            // treeGdiPath
            // 
            this.treeGdiPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.treeGdiPath.Location = new System.Drawing.Point(13, 107);
            this.treeGdiPath.Name = "treeGdiPath";
            this.treeGdiPath.Size = new System.Drawing.Size(256, 313);
            this.treeGdiPath.TabIndex = 5;
            // 
            // btnAddSegmentToGDIPath
            // 
            this.btnAddSegmentToGDIPath.AccessibleDescription = "";
            this.btnAddSegmentToGDIPath.Location = new System.Drawing.Point(12, 55);
            this.btnAddSegmentToGDIPath.Name = "btnAddSegmentToGDIPath";
            this.btnAddSegmentToGDIPath.Size = new System.Drawing.Size(257, 23);
            this.btnAddSegmentToGDIPath.TabIndex = 6;
            this.btnAddSegmentToGDIPath.Text = _AddSegmentToSelectedGDIPath;
            this.btnAddSegmentToGDIPath.UseVisualStyleBackColor = true;
            this.btnAddSegmentToGDIPath.Click += new System.EventHandler(this.btnAddSegmentToSelectedGDIPath_Click);
            // 
            // btnRemoveSegmentFromGDIPath
            // 
            this.btnRemoveSegmentFromGDIPath.AccessibleDescription = "";
            this.btnRemoveSegmentFromGDIPath.Location = new System.Drawing.Point(12, 80);
            this.btnRemoveSegmentFromGDIPath.Name = "btnRemoveSegmentFromGDIPath";
            this.btnRemoveSegmentFromGDIPath.Size = new System.Drawing.Size(257, 23);
            this.btnRemoveSegmentFromGDIPath.TabIndex = 7;
            this.btnRemoveSegmentFromGDIPath.Text = "Remove Selected Segment From GDIPath";
            this.btnRemoveSegmentFromGDIPath.UseVisualStyleBackColor = true;
            this.btnRemoveSegmentFromGDIPath.Click += new System.EventHandler(this.btnRemoveSelectedGDIPathSegment_Click);
            // 
            // btnSaveToFile
            // 
            this.btnSaveToFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSaveToFile.Location = new System.Drawing.Point(144, 424);
            this.btnSaveToFile.Name = "btnSaveToFile";
            this.btnSaveToFile.Size = new System.Drawing.Size(125, 23);
            this.btnSaveToFile.TabIndex = 8;
            this.btnSaveToFile.Text = "Save";
            this.btnSaveToFile.UseVisualStyleBackColor = true;
            this.btnSaveToFile.Click += new System.EventHandler(this.btnSaveToFile_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLoad.Location = new System.Drawing.Point(13, 424);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(125, 23);
            this.btnLoad.TabIndex = 9;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // NShapeCreatorControlView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(761, 451);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSaveToFile);
            this.Controls.Add(this.btnRemoveSegmentFromGDIPath);
            this.Controls.Add(this.btnAddSegmentToGDIPath);
            this.Controls.Add(this.treeGdiPath);
            this.Controls.Add(this.gdiControlGrid);
            this.Name = "NShapeCreatorControlView";
            this.Text = "NShapeCreator Controller";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid gdiControlGrid;
        private System.Windows.Forms.TreeView treeGdiPath;
        private System.Windows.Forms.Button btnAddSegmentToGDIPath;
        private System.Windows.Forms.Button btnRemoveSegmentFromGDIPath;
        private System.Windows.Forms.Button btnSaveToFile;
        private System.Windows.Forms.Button btnLoad;
    }
}