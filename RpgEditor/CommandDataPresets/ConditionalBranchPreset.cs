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
    public partial class ConditionalBranchPreset : UserControl, CommandDataInterface
    {

        Genus2D.GameData.EventCommand _command;

        public ConditionalBranchPreset(Genus2D.GameData.EventCommand command)
        {
            InitializeComponent();
            _command = command;

            List<string> maps = Genus2D.GameData.MapInfo.GetMapInfoStrings();
            PlayerMap.Items.AddRange(maps.ToArray());
            MapEventMap.Items.AddRange(maps.ToArray());

            List<string> items = Genus2D.GameData.ItemData.GetItemNames();
            ItemEquippedSelection.Items.AddRange(items.ToArray());
            ItemInInventorySelection.Items.AddRange(items.ToArray());

            List<string> variables = Genus2D.GameData.SystemVariable.GetVariableNames();
            VariableSelection.Items.AddRange(variables.ToArray());

            SetConditionalType((Genus2D.GameData.ConditionalBranchType)command.GetParameter("ConditionalBranchType"));

            PlayerMap.SelectedIndex = (int)command.GetParameter("PlayerMapID");
            PlayerX.Value = (int)command.GetParameter("PlayerMapX");
            PlayerY.Value = (int)command.GetParameter("PlayerMapY");

            MapEventMap.SelectedIndex = (int)command.GetParameter("MapEventMapID");
            MapEvent.SelectedIndex = (int)command.GetParameter("MapEventID");
            MapEventX.Value = (int)command.GetParameter("MapEventMapX");
            MapEventY.Value = (int)command.GetParameter("MapEventMapY");

            ItemEquippedSelection.SelectedIndex = (int)command.GetParameter("EquippedItemID");
            ItemInInventorySelection.SelectedIndex = (int)command.GetParameter("InventoryItemID");
            InventoryItemStack.Value = (int)command.GetParameter("InventoryItemAmount");

            VariableSelection.SelectedIndex = (int)command.GetParameter("VariableID");
            SetVariableType((Genus2D.GameData.VariableType)command.GetParameter("VariableType"));
            VariableIntegerValue.Value = (int)command.GetParameter("VariableIntegerValue");
            VariableFloatValue.Value = (decimal)((float)command.GetParameter("VariableFloatValue"));
            VariableBoolValue.SelectedIndex = (bool)command.GetParameter("VariableBoolValue") == true ? 0 : 1;
            VariableTextValue.Text = (string)command.GetParameter("VariableTextValue");
            VariableValueCondition.SelectedIndex = (int)command.GetParameter("ValueCondition");
            VariableTextCondition.SelectedIndex = (int)command.GetParameter("TextCondition");

            QuestStatus.SelectedIndex = (int)command.GetParameter("QuestStatus");

        }

        private void SetConditionalType(Genus2D.GameData.ConditionalBranchType type)
        {
            switch (type)
            {
                case Genus2D.GameData.ConditionalBranchType.PlayerPosition:
                    PlayerPositionCheck.Checked = true;
                    break;
                case Genus2D.GameData.ConditionalBranchType.MapEventPosition:
                    MapEventPositionCheck.Checked = true;
                    break;
                case Genus2D.GameData.ConditionalBranchType.ItemEquipped:
                    ItemEquippedCheck.Checked = true;
                    break;
                case Genus2D.GameData.ConditionalBranchType.ItemInInventory:
                    ItemInInventoryCheck.Checked = true;
                    break;
                case Genus2D.GameData.ConditionalBranchType.SystemVariable:
                    SystemVariableCheck.Checked = true;
                    break;
                case Genus2D.GameData.ConditionalBranchType.QuestStatus:
                    QuestStatusCheck.Checked = true;
                    break;
            }
        }

        private Genus2D.GameData.ConditionalBranchType GetConditionalType()
        {
            if (PlayerPositionCheck.Checked)
                return Genus2D.GameData.ConditionalBranchType.PlayerPosition;
            if (MapEventPositionCheck.Checked)
                return Genus2D.GameData.ConditionalBranchType.MapEventPosition;
            if (ItemEquippedCheck.Checked)
                return Genus2D.GameData.ConditionalBranchType.ItemEquipped;
            if (ItemInInventoryCheck.Checked)
                return Genus2D.GameData.ConditionalBranchType.ItemInInventory;
            if (SystemVariableCheck.Checked)
                return Genus2D.GameData.ConditionalBranchType.SystemVariable;

            return Genus2D.GameData.ConditionalBranchType.QuestStatus;
        }

        private void SetVariableType(Genus2D.GameData.VariableType type)
        {
            switch (type)
            {
                case Genus2D.GameData.VariableType.Integer:
                    VariableIntegerCheck.Checked = true;
                    break;
                case Genus2D.GameData.VariableType.Float:
                    VariableFloatCheck.Checked = true;
                    break;
                case Genus2D.GameData.VariableType.Bool:
                    VariableBoolCheck.Checked = true;
                    break;
                case Genus2D.GameData.VariableType.Text:
                    VariableTextCheck.Checked = true;
                    break;
            }
        }

        private Genus2D.GameData.VariableType GetVariableType()
        {
            if (VariableIntegerCheck.Checked)
                return Genus2D.GameData.VariableType.Integer;
            if (VariableFloatCheck.Checked)
                return Genus2D.GameData.VariableType.Float;
            if (VariableBoolCheck.Checked)
                return Genus2D.GameData.VariableType.Bool;

            return Genus2D.GameData.VariableType.Text;
        }

        private void PlayerMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            int mapID = PlayerMap.SelectedIndex;
            if (mapID != -1)
            {
                PlayerX.Maximum = Genus2D.GameData.MapInfo.GetMapInfo(mapID).Width - 1;
                PlayerY.Maximum = Genus2D.GameData.MapInfo.GetMapInfo(mapID).Height - 1;
            }
            else
            {
                PlayerX.Maximum = 0;
                PlayerY.Maximum = 0;
            }
        }

        private void MapEventMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            MapEvent.Items.Clear();
            int selection = MapEventMap.SelectedIndex;
            if (selection != -1)
            {
                MapEventX.Maximum = Genus2D.GameData.MapInfo.GetMapInfo(selection).Width;
                MapEventY.Maximum = Genus2D.GameData.MapInfo.GetMapInfo(selection).Height;

                Genus2D.GameData.MapData data = Genus2D.GameData.MapInfo.LoadMap(selection);
                for (int i = 0; i < data.MapEventsCount(); i++)
                {
                    MapEvent.Items.Add(data.GetMapEvent(i).Name);
                }
                MapEvent.SelectedIndex = MapEvent.Items.Count > 0 ? 0 : -1;
            }
            else
            {
                MapEvent.SelectedIndex = -1;
                MapEventX.Maximum = 0;
                MapEventY.Maximum = 0;
            }

            MapEventX.Value = 0;
            MapEventY.Value = 0;
        }

        private void ItemInInventorySelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selection = ItemInInventorySelection.SelectedIndex;
            if (selection > 0)
            {
                InventoryItemStack.Maximum = Genus2D.GameData.ItemData.GetItemData(selection).GetMaxStack();
            }
            else
            {
                InventoryItemStack.Maximum = 1;
            }
        }

        public void ApplyData()
        {
            Genus2D.GameData.ConditionalBranchType condition = GetConditionalType();
            _command.SetParameter("ConditionalBranchType", condition);

            int mapID;

            switch (condition)
            {
                case Genus2D.GameData.ConditionalBranchType.PlayerPosition:
                    mapID = PlayerMap.SelectedIndex;

                    if (mapID == -1)
                    {
                        MessageBox.Show("Please select a valid map.");
                        return;
                    }

                    _command.SetParameter("PlayerMapID", mapID);
                    _command.SetParameter("PlayerMapX", (int)PlayerX.Value);
                    _command.SetParameter("PlayerMapY", (int)PlayerY.Value);
                    break;
                case Genus2D.GameData.ConditionalBranchType.MapEventPosition:
                    mapID = MapEventMap.SelectedIndex;
                    int eventID = MapEvent.SelectedIndex;

                    if (mapID == -1)
                    {
                        MessageBox.Show("Please select a valid map.");
                        return;
                    }
                    if (eventID == -1)
                    {
                        MessageBox.Show("Please select a valid map event.");
                        return;
                    }

                    _command.SetParameter("MapEventMapID", mapID);
                    _command.SetParameter("MapEventID", eventID);
                    _command.SetParameter("MapEventMapX", (int)MapEventX.Value);
                    _command.SetParameter("MapEventMapY", (int)MapEventY.Value);

                    break;
                case Genus2D.GameData.ConditionalBranchType.ItemEquipped:
                    _command.SetParameter("EquippedItemID", ItemEquippedSelection.SelectedIndex);
                    break;
                case Genus2D.GameData.ConditionalBranchType.ItemInInventory:
                    _command.SetParameter("InventoryItemID", ItemInInventorySelection.SelectedIndex);
                    _command.SetParameter("InventoryItemAmount", (int)InventoryItemStack.Value);
                    break;
                case Genus2D.GameData.ConditionalBranchType.SystemVariable:

                    _command.SetParameter("VariableID", VariableSelection.SelectedIndex);
                    _command.SetParameter("VariableType", GetVariableType());

                    switch (GetVariableType())
                    {
                        case Genus2D.GameData.VariableType.Integer:
                            _command.SetParameter("VariableIntegerValue", (int)VariableIntegerValue.Value);
                            _command.SetParameter("ValueCondition", (Genus2D.GameData.ConditionValueCheck)VariableValueCondition.SelectedIndex);
                            break;
                        case Genus2D.GameData.VariableType.Float:
                            _command.SetParameter("VariableFloatValue", (float)VariableFloatValue.Value);
                            _command.SetParameter("ValueCondition", (Genus2D.GameData.ConditionValueCheck)VariableValueCondition.SelectedIndex);
                            break;
                        case Genus2D.GameData.VariableType.Bool:
                            _command.SetParameter("VariableBoolValue", VariableBoolValue.SelectedIndex == 0 ? true : false);
                            break;
                        case Genus2D.GameData.VariableType.Text:
                            _command.SetParameter("VariableTextValue", VariableTextValue.Text);
                            _command.SetParameter("TextCondition", (Genus2D.GameData.ConditionalTextCheck)VariableTextCondition.SelectedIndex);
                            break;
                    }

                    break;
                case Genus2D.GameData.ConditionalBranchType.QuestStatus:
                    _command.SetParameter("QuestStatus", (Genus2D.GameData.QuestStatus)QuestStatus.SelectedIndex);
                    break;
            }

        }

        
    }
}
