using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using MetroHome65.Interfaces;
using MetroHome65.Routines;

namespace MetroHome65.HomeScreen
{
    public partial class FrmWidgetSettings : Form
    {
        private TileWrapper _tile = null;
        private ITile _selectedTile = null;
        private List<Type> _WidgetTypes = new List<Type>();

        public FrmWidgetSettings()
        {
            InitializeComponent();
        }

        public TileWrapper Tile { set { SetWidget(value); } }

        private void SetWidget(TileWrapper value)
        {
            _tile = value;

            this.SuspendLayout();

            SetWidgetType(_tile.Tile);

            FillWidgetTypes();

            this.ResumeLayout(false);
        }

        private void SetWidgetType(ITile value)
        {
            if ((_selectedTile == null) ||
                (((object)_selectedTile).GetType() != ((object)value).GetType()) )
            {
                _selectedTile = value;

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
                object[] Attributes = Plugin.GetCustomAttributes(typeof(TileInfoAttribute), true);
                if (Attributes.Length > 0)
                    WidgetName = (Attributes[0] as TileInfoAttribute).Caption;
                if (WidgetName == "")
                    WidgetName = Plugin.Name;

                CurIndex = cbType.Items.Add(WidgetName);
                _WidgetTypes.Add(Plugin);

                if (Plugin.FullName == _tile.Tile.GetType().FullName)
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
            foreach (Size GridSize in _selectedTile.Sizes)
            {
                SizeStr = GridSize.Width + " x " + GridSize.Height;
                CurIndex = cbSize.Items.Add(SizeStr);
                if (GridSize.Equals(_tile.GridSize))
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

            List<Control> Controls = _selectedTile.EditControls;
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
            SetWidgetType(PluginManager.GetInstance().CreateTile(WidgetName));
        }


        private void menuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void mnuApply_Click(object sender, EventArgs e)
        {
            try
            {
                _tile.TileClass = ((object)_selectedTile).GetType().ToString();
                _tile.GridSize = _selectedTile.Sizes[cbSize.SelectedIndex];

                // apply custom widget parameters
                foreach (PropertyInfo propertyInfo in ((object)_selectedTile).GetType().GetProperties())
                {
                    object[] Attributes = propertyInfo.GetCustomAttributes(typeof(TileParameterAttribute), true);
                    if (Attributes.Length > 0)
                    {
                        foreach (object Attribute in Attributes)
                        {
                            _tile.SetParameter(propertyInfo.Name, propertyInfo.GetValue((object)_selectedTile, null));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.StackTrace, "Apply settings error");
            }

            this.DialogResult = DialogResult.OK;
        }

    }
}