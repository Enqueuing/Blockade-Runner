using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;

namespace Blockade_Runner___Item_Editor
{
    public partial class Form1 : Form
    {
        public List<BR_ItemData> Items = new List<BR_ItemData>();
        string[] Directories;
        string MainBRFolder = @"R:\Trunk\BlockadeRunner\BlockadeRunner\bin\x86\Debug\";
        public Form1()
        {
            InitializeComponent();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Directories = System.IO.Directory.GetDirectories(MainBRFolder + @"Content\Items");
            for (int i = 0; i < Directories.Length; i++)
                Items.Add(BR_ItemData.LoadItemFromPath(Directories[i] + @"\", i));
            treeView1.Nodes.Add("Items");
            for (int i = 0; i < Items.Count; i++)
            {
                treeView1.Nodes[0].Nodes.Add(Items[i].Name);
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            propertyGrid1.SelectedObject = Items[e.Node.Index];
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            ((BR_ItemData)((PropertyGrid)s).SelectedObject).Modified = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Directories = System.IO.Directory.GetDirectories(MainBRFolder + @"Content\Items");
            Items.Clear();
            treeView1.Nodes.Clear();
            for (int i = 0; i < Directories.Length; i++)
                Items.Add(BR_ItemData.LoadItemFromPath(Directories[i] + @"\", i));
            treeView1.Nodes.Add("Items");
            for (int i = 0; i < Items.Count; i++)
            {
                treeView1.Nodes[0].Nodes.Add(Items[i].Name);
            }
        }
    }



    [System.Diagnostics.DebuggerDisplay("x:{X} y:{Y} len:{System.Math.Sqrt(X*X+Y*Y)}")]
    public class Vector2
    {
        [XmlAttribute]
        public float X { get; set; }
        [XmlAttribute]
        public float Y { get; set; }
        public Vector2()
        {
            X = Y = 0;
        }
        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

    [Editor(typeof(Vector3Editor), typeof(UITypeEditor))]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [System.Diagnostics.DebuggerDisplay("x:{X} y:{Y} z:{Z} len:{System.Math.Sqrt(X*X+Y*Y+Z*Z)}")]
    public class Vector3
    {
        [XmlAttribute]
        public float X { get; set; }
        [XmlAttribute]
        public float Y { get; set; }
        [XmlAttribute]
        public float Z { get; set; }
        public Vector3()
        {
            X = Y = Z = 0;
        }
        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    [System.Diagnostics.DebuggerDisplay("String:{String} Int:{Int} }")]
    public class StringInt : BaseVariable
    {
        public string String { get; set; }
        public int Int { get; set; }
        public StringInt()
        {
            String = "";
            Int = 0;
        }
        public StringInt(string _string, int _int)
        {
            String = _string;
            Int = _int;
        }
    }

    [System.Diagnostics.DebuggerDisplay("String:{String} Float:{Float} }")]
    public class StringFloat : BaseVariable
    {
        public string String { get; set; }
        public float Float { get; set; }
        public StringFloat()
        {
            String = "";
            Float = 0;
        }
        public StringFloat(string _string, float _float)
        {
            String = _string;
            Float = _float;
        }
    }

    [System.Diagnostics.DebuggerDisplay("Name:{Name} Value:{Value} }")]
    public class StringString
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("value")]
        public string Value { get; set; }

        public StringString()
        {
            Name = "";
            Value = "";
        }
        public StringString(string _name, string _value)
        {
            Name = _name;
            Value = _value;
        }
    }

    public class BaseVariable
    {
        public string Name { get; set; }
        protected object BaseValue;
        public BaseVariable()
        {
            Name = "";
            BaseValue = null;
        }
        public BaseVariable(string name, object value)
        {
            Name = name;
            BaseValue = value;
        }
    }

    [Serializable]
    public class Variables
    {
        [XmlElement("String")]
        public List<StringString> Strings { get; set; }
        public List<StringFloat> Floats { get; set; }

        public Variables()
        {
            Strings = new List<StringString>();
            Floats = new List<StringFloat>();
        }
    }
    class Vector2Converter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(String);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string sValue = value as string;
            object retVal = null;

            if (sValue != null)
            {
                sValue = sValue.Trim();

                if (sValue.Length != 0)
                {
                    // Parse the string
                    if (null == culture)
                        culture = CultureInfo.CurrentCulture;

                    // Split the string based on the cultures list separator
                    string[] parms = sValue.Split(new char[] { culture.TextInfo.ListSeparator[0] });

                    if (parms.Length == 2)
                    {
                        // Should have an integer and a string.
                        float x = Convert.ToSingle(parms[0]);
                        float y = Convert.ToSingle(parms[1]);

                        // And finally create the object
                        retVal = new Vector2(x, y);
                    }
                }
            }
            else
                retVal = base.ConvertFrom(context, culture, value);

            return retVal;

        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ((Vector2)value).X + "," + ((Vector2)value).Y;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
        {
            return new Vector2((float)propertyValues["X"], (float)propertyValues["Y"]);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, attributes);

            string[] sortOrder = new string[2];

            sortOrder[0] = "X";
            sortOrder[1] = "Y";

            // Return a sorted list of properties
            return properties.Sort(sortOrder);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    class Vector3Converter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(String);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string sValue = value as string;
            object retVal = null;

            if (sValue != null)
            {
                sValue = sValue.Trim();

                if (sValue.Length != 0)
                {
                    // Parse the string
                    if (null == culture)
                        culture = CultureInfo.CurrentCulture;

                    // Split the string based on the cultures list separator
                    string[] parms = sValue.Split(new char[] { culture.TextInfo.ListSeparator[0] });

                    if (parms.Length == 3)
                    {
                        // Should have an integer and a string.
                        float x = Convert.ToSingle(parms[0]);
                        float y = Convert.ToSingle(parms[1]);
                        float z = Convert.ToSingle(parms[2]);

                        // And finally create the object
                        retVal = new Vector3(x, y, z);
                    }
                }
            }
            else
                retVal = base.ConvertFrom(context, culture, value);

            return retVal;

        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ((Vector3)value).X + "," + ((Vector3)value).Y + "," + ((Vector3)value).Z;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
        {
            return new Vector3((float)propertyValues["X"], (float)propertyValues["Y"], (float)propertyValues["Z"]);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, attributes);

            string[] sortOrder = new string[3];

            sortOrder[0] = "X";
            sortOrder[1] = "Y";
            sortOrder[2] = "Z";

            // Return a sorted list of properties
            return properties.Sort(sortOrder);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    class Vector3Editor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            Vector3 foo = value as Vector3;
            if (svc != null && foo != null)
            {
                using (Vector3Edit form = new Vector3Edit())
                {
                    form.X = foo.X.ToString();
                    form.Y = foo.Y.ToString();
                    form.Z = foo.Z.ToString();
                    if (svc.ShowDialog(form) == DialogResult.OK)
                    {
                        foo.X = int.Parse(form.X);
                        foo.Y = int.Parse(form.Y);
                        foo.Z = int.Parse(form.Z);
                        
                    }
                }
            }
            return value; // can also replace the wrapper object here
        }
    }
    class Vector3Edit : Form
    {
        private TextBox textboxX;
        private TextBox textboxY;
        private TextBox textboxZ;
        private Button okButton;
        public Vector3Edit()
        {
            textboxX = new TextBox();
            textboxX.Location = new Point(50, 20);
            Controls.Add(textboxX);
            textboxY = new TextBox();
            textboxY.Location = new Point(50, 60);
            Controls.Add(textboxY);
            textboxZ = new TextBox();
            textboxZ.Location = new Point(50, 100);
            Controls.Add(textboxZ);
            okButton = new Button();
            okButton.Text = "OK";
            okButton.Dock = DockStyle.Bottom;
            okButton.DialogResult = DialogResult.OK;
            Controls.Add(okButton);
        }
        public string X
        {
            get { return textboxX.Text; }
            set { textboxX.Text = value; }
        }
        public string Y
        {
            get { return textboxY.Text; }
            set { textboxY.Text = value; }
        }
        public string Z
        {
            get { return textboxZ.Text; }
            set { textboxZ.Text = value; }
        }
    }

}
