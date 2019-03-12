using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgEditor.ItemDataPresets
{
    public partial class MatarialPreset : UserControl
    {
        public MatarialPreset()
        {
            InitializeComponent();
        }

        public int GetMaterialID()
        {
            return (int)MaterialIdControl.Value;
        }

        public void SetMaterialID(int id)
        {
            MaterialIdControl.Value = id;
        }
    }
}
