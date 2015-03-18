using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Xml.Serialization;

namespace NShapeCreator
{
    public class NPath : ICloneable
    {

        #region Constructor

        public NPath()
        {
            Segments = new NSegmentCollection();
            PenColor = Color.Black;
            FillColor = Color.SteelBlue;
            PenWidth = 1;
            DisplaySizeMultiple = 40;
            WidthF = 10;
            HeightF = 10;
            DisplayBoundingBox = true;
        }

        #endregion

        #region Category: Segments

        [Category("Segments")]
        [Description("Collection of segments that define this path")]
        [Editor(typeof(CustomCollectionEditor), typeof(UITypeEditor))]
        public NSegmentCollection Segments { get; set; }

        #endregion

        #region Category: Path Options

        private string _name;
        [Category("\t\tPath Options")]
        [Description("The name that will be assigned this shape")]
        [DisplayName("\tPath Name")]
        public string Name
        {
            get
            {
                if (String.IsNullOrEmpty(_name))
                {
                    _name = "NewPath";
                }
                return _name;
            }
            set
            {
                _name = value.Replace(" ", "");
            }
        }

        [Category("\t\tPath Options")]
        [DisplayName("\tWidth")]
        public float WidthF
        {
            get;
            set;
        }

        [Category("\t\tPath Options")]
        public float HeightF
        {
            get;
            set;
        }

        #endregion

        #region Category: Display Output Options

        [Category("\tDisplay Output Options")]
        [Description("Used to display the shapes bounding box")]
        [DisplayName("Display Bounding Box")]
        public bool DisplayBoundingBox
        {
            get;
            set;
        }

        private float _displaySizeMultiple;
        [Category("\tDisplay Output Options")]
        [Description("Multiplies the display size (X and Y) of the image")]
        [DisplayName("Display Size Multiple")]
        public float DisplaySizeMultiple
        {
            get { return _displaySizeMultiple; }
            set { _displaySizeMultiple = value; }
        }

        [Category("\tDisplay Output Options")]
        [Description("Color of the brush to use to fill this path")]
        [DisplayName("Fill Color")]
        [XmlIgnore()]
        public Color FillColor { get; set; }

        [Browsable(false)]
        [XmlElement("FillColorStringName")]
        public string FillColorStringName
        {
            get
            {
                return NShape.SerializeColor(FillColor);
            }
            set
            {
                FillColor = NShape.DeserializeColor(value);
            }
        }

        [Category("\tDisplay Output Options")]
        [Description("Color of the pen to use to paint this path")]
        [DisplayName("Pen Color")]
        [XmlIgnore()]
        public Color PenColor { get; set; }

        [Browsable(false)]
        [XmlElement("PenColorStringName")]
        public string PenColorStringName
        {
            get
            {
                return NShape.SerializeColor(PenColor);
            }
            set
            {
                PenColor = NShape.DeserializeColor(value);
            }
        }

        [Category("\tDisplay Output Options")]
        [Description("Width of the pen to use to paint this path")]
        [DisplayName("Pen Width")]
        public float PenWidth { get; set; }

        #endregion

        public Object Clone()
        {
            NPath me = new NPath();
            me.DisplayBoundingBox = DisplayBoundingBox;
            me.DisplaySizeMultiple = DisplaySizeMultiple;
            me.FillColor = FillColor;
            me.HeightF = HeightF;
            me.Name = Name;
            me.PenColor = PenColor;
            me.PenWidth = PenWidth;
            me.Segments = (NSegmentCollection)Segments.Clone();
            me.WidthF = WidthF;
            return me;
        }

    }
}
