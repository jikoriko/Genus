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
    public partial class ChangeSystemVariablePreset : UserControl, CommandDataInterface
    {

        private Genus2D.GameData.EventCommand _command;

        public ChangeSystemVariablePreset(Genus2D.GameData.EventCommand command)
        {
            InitializeComponent();
            _command = command;

            List<string> systemVariables = Genus2D.GameData.SystemVariable.GetVariableNames();
            SystemVariables.Items.AddRange(systemVariables.ToArray());

            SystemVariables.SelectedIndex = (int)command.GetParameter("VariableID");

            Genus2D.GameData.VariableType type = (Genus2D.GameData.VariableType)command.GetParameter("VariableType");
            VariableType.SelectedIndex = (int)type;

            object value = command.GetParameter("VariableValue");

            switch (type)
            {
                case Genus2D.GameData.VariableType.Integer:
                    ValueBox.Text = ((int)value).ToString();
                    break;
                case Genus2D.GameData.VariableType.Float:
                    ValueBox.Text = ((float)value).ToString();
                    break;
                case Genus2D.GameData.VariableType.Bool:
                    ValueBox.Text = ((bool)value).ToString();
                    break;
                case Genus2D.GameData.VariableType.Text:
                    ValueBox.Text = (string)value;
                    break;
            }


        }

        public void ApplyData()
        {
            int variableID = SystemVariables.SelectedIndex;
            if (variableID != -1)
            {
                Genus2D.GameData.VariableType type = (Genus2D.GameData.VariableType)VariableType.SelectedIndex;
                object value = null;

                try
                {
                    switch (type)
                    {
                        case Genus2D.GameData.VariableType.Integer:
                            value = int.Parse(ValueBox.Text);
                            break;
                        case Genus2D.GameData.VariableType.Float:
                            value = float.Parse(ValueBox.Text);
                            break;
                        case Genus2D.GameData.VariableType.Bool:
                            if (ValueBox.Text.ToLower() == "true") value = true;
                            else if (ValueBox.Text.ToLower() == "false") value = false;
                            else throw new Exception("Value must be true or false.");
                            break;
                        case Genus2D.GameData.VariableType.Text:
                            value = ValueBox.Text;
                            break;
                    }

                    _command.SetParameter("VariableID", variableID);
                    _command.SetParameter("VariableType", type);
                    _command.SetParameter("VariableValue", value);

                }
                catch (Exception e)
                {
                    MessageBox.Show("Error parsing variable value." + '\n' + e.Message);
                }

            }
            else
            {
                MessageBox.Show("Select a valid sytstem variable.");
            }
        }
    }
}
