using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Genus2D.GameData;

namespace RpgEditor.ItemDataPresets
{
    public partial class ToolPreset : UserControl
    {
        public ToolPreset()
        {
            InitializeComponent();
        }

        public ToolType GetToolType()
        {
            return (ToolType)ToolTypeSelection.SelectedIndex;
        }

        public void SetToolType(ToolType type)
        {
            ToolTypeSelection.SelectedIndex = (int)type;
        }
    }
}
