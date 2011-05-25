using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using MetroHome65.Widgets;

namespace MetroHome65.Pages
{
    public partial class FrmWidgetSettings : Form
    {
        private WidgetWrapper _Widget = null;
        private IWidget _SelectedWidget = null;
        private Settings_color ctrlColorSelect = null;
        private Settings_image ctrlImageSelect = null;
        private int _ControlTop = 0;

        public FrmWidgetSettings()
        {
            InitializeComponent();

            _ControlTop = panelSize.Bottom;

            ctrlColorSelect = new Settings_color();
            PlaceControl(ctrlColorSelect, false);

            ctrlImageSelect = new Settings_image();
            ctrlImageSelect.Caption = "Button background";
            PlaceControl(ctrlImageSelect, false);
        }

        public WidgetWrapper Widget { set { SetWidget(value); } }

        private void SetWidget(WidgetWrapper value)
        {
            _Widget = value;

            FillWidgetTypes();
            FillButtonColor(_Widget.Color);
            FillButtonImage(_Widget.ButtonImage);

            SetWidgetType(_Widget.Widget);
        }

        private void SetWidgetType(IWidget value)
        {
            if ((_SelectedWidget == null) ||
                ((object)_SelectedWidget).GetType() != ((object)value).GetType())
            {
                _SelectedWidget = value;

                FillSizes();
                FillWidgetProperties();
            }
        }

        private void FillWidgetTypes()
        {
            cbType.Items.Clear();

            foreach (Type Plugin in PluginManager.GetInstance()._plugins.Values)
            {
                cbType.Items.Add(Plugin.FullName);
            }
            cbType.SelectedIndex = 0;
        }

        private void FillSizes()
        {
            cbSize.Items.Clear();

            String SizeStr = "";
            foreach (Size Size in _SelectedWidget.Sizes)
            {
                SizeStr = Size.Width + " x " + Size.Height;
                cbSize.Items.Add(SizeStr);
            }
            cbSize.SelectedIndex = 0;
        }

        private void FillButtonColor(Color Color)
        {
            ctrlColorSelect.ButtonColor = Color;
        }

        private void FillButtonImage(String ButtonImage)
        {
            ctrlImageSelect.Value = ButtonImage;
        }

        private void FillWidgetProperties()
        {
            // first delete previous widget properties
            foreach (Control Control in this.Controls)
            {
                if (Control.Name.StartsWith("__"))
                    this.Controls.Remove(Control);
            }
            _ControlTop = ctrlImageSelect.Bottom;


            // add current widget properties
            foreach (PropertyInfo propertyInfo in ((object)_SelectedWidget).GetType().GetProperties())
            {
                object[] Attributes = propertyInfo.GetCustomAttributes(typeof(WidgetParameter), true);
                if (Attributes.Length > 0)
                {
                    foreach (object Attribute in Attributes)
                    {
                        Control Control = null;

                        switch ((Attribute as WidgetParameter).EditType)
                        {
                            case WidgetParameterEditType.edString:
                            {
                                Settings_string EditControl = new Settings_string();
                                EditControl.Caption = (Attribute as WidgetParameter).Caption;
                                EditControl.Value = (String)propertyInfo.GetValue((object)_SelectedWidget, null);
                                Control = EditControl;
                                break;
                            }

                            case WidgetParameterEditType.edFile:
                            {
                                Settings_file FileControl = new Settings_file();
                                FileControl.Caption = (Attribute as WidgetParameter).Caption;
                                FileControl.Value = (String)propertyInfo.GetValue((object)_SelectedWidget, null);
                                Control = FileControl;
                                break;
                            }

                            case WidgetParameterEditType.edImage:
                            {
                                Settings_image ImgControl = new Settings_image();
                                ImgControl.Caption = (Attribute as WidgetParameter).Caption;
                                ImgControl.Value = (String)propertyInfo.GetValue((object)_SelectedWidget, null);
                                Control = ImgControl;
                                break;
                            }
                        }

                        // place parameter editor control
                        PlaceControl(Control, true);
                    }
                }
            }
        }

        private void PlaceControl(Control Control, Boolean UserParam)
        {
            if (Control != null)
            {
                if (UserParam)
                    Control.Name = "__Control_" + this.Controls.Count.ToString();

                Control.Parent = this;
                Control.Width = this.Width;
                Control.Top = _ControlTop;
                _ControlTop += Control.Height;

                this.Controls.Add(Control);
            }
        }
        

        // chane widget type
        private void cbType_SelectedValueChanged(object sender, EventArgs e)
        {
            String WidgetName = (sender as ComboBox).SelectedItem.ToString();
            SetWidgetType(PluginManager.GetInstance().CreateWidget(WidgetName));
        }

        private void menuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void mnuApply_Click(object sender, EventArgs e)
        {
            try
            {
                _Widget.WidgetClass = ((object)_SelectedWidget).GetType().ToString();
                _Widget.Size = _SelectedWidget.Sizes[cbSize.SelectedIndex];
                _Widget.Color = ctrlColorSelect.ButtonColor;
                _Widget.ButtonImage = ctrlImageSelect.Value;
            }
            catch (Exception ex)
            {
                //!! write to log  (e.StackTrace, "SetBtnImg")
            }

            this.DialogResult = DialogResult.OK;
        }

    }
}