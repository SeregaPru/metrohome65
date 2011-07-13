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
        private List<Type> _WidgetTypes = new List<Type>();

        public FrmWidgetSettings()
        {
            InitializeComponent();
        }

        public WidgetWrapper Widget { set { SetWidget(value); } }

        private void SetWidget(WidgetWrapper value)
        {
            _Widget = value;

            SetWidgetType(_Widget.Widget);

            FillWidgetTypes();
        }

        private void SetWidgetType(IWidget value)
        {
            if ((_SelectedWidget == null) ||
                (((object)_SelectedWidget).GetType() != ((object)value).GetType()) )
            {
                _SelectedWidget = value;

                FillSizes();
                FillWidgetProperties();
            }
        }

        private void FillWidgetTypes()
        {
            cbType.Items.Clear();
            _WidgetTypes.Clear();

            int CurIndex;
            String WidgetName;
            foreach (Type Plugin in PluginManager.GetInstance()._plugins.Values)
            {
                // get human readable widget name for display in list
                WidgetName = "";
                object[] Attributes = Plugin.GetCustomAttributes(typeof(WidgetInfoAttribute), true);
                if (Attributes.Length > 0)
                    WidgetName = (Attributes[0] as WidgetInfoAttribute).Caption;
                if (WidgetName == "")
                    WidgetName = Plugin.Name;

                CurIndex = cbType.Items.Add(WidgetName);
                _WidgetTypes.Add(Plugin);

                if (Plugin.FullName == _Widget.Widget.GetType().FullName)
                    cbType.SelectedIndex = CurIndex;
            }
            if (cbType.SelectedIndex == -1)
                cbType.SelectedIndex = 0;
        }

        private void FillSizes()
        {
            cbSize.Items.Clear();

            String SizeStr = "";
            int CurIndex;
            foreach (Size Size in _SelectedWidget.Sizes)
            {
                SizeStr = Size.Width + " x " + Size.Height;
                CurIndex = cbSize.Items.Add(SizeStr);
                if (Size.Equals(_Widget.Size))
                    cbSize.SelectedIndex = CurIndex;
            }
            if (cbSize.SelectedIndex == -1)
                cbSize.SelectedIndex = 0;
        }


        private void FillWidgetProperties()
        {
            // first delete previous widget properties
            foreach (Control Control in this.Controls)
            {
                if (Control.Name.StartsWith("__"))
                    this.Controls.Remove(Control);
            }

            List<Control> Controls = _SelectedWidget.EditControls;
            if (Controls != null)
            {
                foreach (Control UserControl in Controls)
                    PlaceControl(UserControl, true);
            }
        }

        private void PlaceControl(Control Control, Boolean UserParam)
        {
            if (Control != null)
            {
                int _ControlTop = 0;
                foreach (Control ctrl in this.Controls)
                    _ControlTop = Math.Max(_ControlTop, ctrl.Bottom);
                
                if (UserParam)
                    Control.Name = "__Control_" + this.Controls.Count.ToString();

                Control.Parent = this;
                Control.Top = _ControlTop;

                this.Controls.Add(Control);
            }
        }


        // change widget type
        private void cbType_SelectedValueChanged(object sender, EventArgs e)
        {
            String WidgetName = _WidgetTypes[(sender as ComboBox).SelectedIndex].FullName;
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

                // apply custom widget parameters
                foreach (PropertyInfo propertyInfo in ((object)_SelectedWidget).GetType().GetProperties())
                {
                    object[] Attributes = propertyInfo.GetCustomAttributes(typeof(WidgetParameterAttribute), true);
                    if (Attributes.Length > 0)
                    {
                        foreach (object Attribute in Attributes)
                        {
                            _Widget.SetParameter(propertyInfo.Name, propertyInfo.GetValue((object)_SelectedWidget, null));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //!! write to log  (e.StackTrace, "SetBtnImg")
            }

            this.DialogResult = DialogResult.OK;
        }

    }
}