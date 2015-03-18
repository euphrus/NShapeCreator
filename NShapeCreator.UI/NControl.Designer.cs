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
            this.btnAddPath = new System.Windows.Forms.Button();
            this.btnRemoveSelected = new System.Windows.Forms.Button();
            this.btnSaveToFile = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnAddSegment = new System.Windows.Forms.Button();
            this.btnImportPath = new System.Windows.Forms.Button();
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
            this.treeGdiPath.Location = new System.Drawing.Point(13, 103);
            this.treeGdiPath.Name = "treeGdiPath";
            this.treeGdiPath.Size = new System.Drawing.Size(256, 319);
            this.treeGdiPath.TabIndex = 5;
            // 
            // btnAddPath
            // 
            this.btnAddPath.AccessibleDescription = "";
            this.btnAddPath.Location = new System.Drawing.Point(12, 5);
            this.btnAddPath.Name = "btnAddPath";
            this.btnAddPath.Size = new System.Drawing.Size(257, 23);
            this.btnAddPath.TabIndex = 6;
            this.btnAddPath.Text = "Add Path";
            this.btnAddPath.UseVisualStyleBackColor = true;
            this.btnAddPath.Click += new System.EventHandler(this.btnAddPath_Click);
            // 
            // btnRemoveSelected
            // 
            this.btnRemoveSelected.AccessibleDescription = "";
            this.btnRemoveSelected.Location = new System.Drawing.Point(12, 74);
            this.btnRemoveSelected.Name = "btnRemoveSelected";
            this.btnRemoveSelected.Size = new System.Drawing.Size(257, 23);
            this.btnRemoveSelected.TabIndex = 7;
            this.btnRemoveSelected.Text = "Remove Selected";
            this.btnRemoveSelected.UseVisualStyleBackColor = true;
            this.btnRemoveSelected.Click += new System.EventHandler(this.btnRemoveSelected_Click);
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
            // btnAddSegment
            // 
            this.btnAddSegment.AccessibleDescription = "";
            this.btnAddSegment.Location = new System.Drawing.Point(12, 51);
            this.btnAddSegment.Name = "btnAddSegment";
            this.btnAddSegment.Size = new System.Drawing.Size(257, 23);
            this.btnAddSegment.TabIndex = 10;
            this.btnAddSegment.Text = "Add Segment";
            this.btnAddSegment.UseVisualStyleBackColor = true;
            this.btnAddSegment.Click += new System.EventHandler(this.btnAddSegment_Click);
            // 
            // btnImportPath
            // 
            this.btnImportPath.AccessibleDescription = "";
            this.btnImportPath.Location = new System.Drawing.Point(12, 28);
            this.btnImportPath.Name = "btnImportPath";
            this.btnImportPath.Size = new System.Drawing.Size(257, 23);
            this.btnImportPath.TabIndex = 11;
            this.btnImportPath.Text = "Import Path";
            this.btnImportPath.UseVisualStyleBackColor = true;
            this.btnImportPath.Click += new System.EventHandler(this.btnImportPath_Click);
            // 
            // NControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(761, 451);
            this.Controls.Add(this.btnImportPath);
            this.Controls.Add(this.btnAddSegment);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSaveToFile);
            this.Controls.Add(this.btnRemoveSelected);
            this.Controls.Add(this.btnAddPath);
            this.Controls.Add(this.treeGdiPath);
            this.Controls.Add(this.gdiControlGrid);
            this.Name = "NControl";
            this.Text = "NShapeCreator Controller";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid gdiControlGrid;
        private System.Windows.Forms.TreeView treeGdiPath;
        private System.Windows.Forms.Button btnAddPath;
        private System.Windows.Forms.Button btnRemoveSelected;
        private System.Windows.Forms.Button btnSaveToFile;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnAddSegment;
        private System.Windows.Forms.Button btnImportPath;
    }
}