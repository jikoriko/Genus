using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgEditor.CommandDataPresets
{
    public partial class ChangePlayerSpritePreset : UserControl, CommandDataInterface
    {

        Genus2D.GameData.EventCommand _command;

        public ChangePlayerSpritePreset(Genus2D.GameData.EventCommand command)
        {
            InitializeComponent();
            _command = command;

            List<string> sprites = Genus2D.GameData.SpriteData.GetSpriteNames();
            SpriteSelection.Items.Add("None");
            SpriteSelection.Items.AddRange(sprites.ToArray());
            SpriteSelection.SelectedIndex = (int)command.GetParameter("SpriteID") + 1;
        }

        public void ApplyData()
        {
            _command.SetParameter("SpriteID", SpriteSelection.SelectedIndex - 1);
        }
    }
}
