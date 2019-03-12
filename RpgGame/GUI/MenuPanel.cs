using Genus2D.Graphics;
using Genus2D.GUI;
using RpgGame.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.GUI
{
    public class MenuPanel : Panel
    {
        public static MessagePanel Instance { get; private set; }
        private GameState _gameState;

        private Button _inventoryButton;
        private Button _equipmentButton;
        private Button _statsButton;

        public MenuPanel(GameState state)
            : base((int)Renderer.GetResoultion().X - 400, (int)Renderer.GetResoultion().Y - 60, 400, 60, BarMode.Empty, state)
        {
            _gameState = state;

            int buttonSize = GetContentHeight() - 4;
            _inventoryButton = new Button("inv", 2, 2, buttonSize, buttonSize, state);
            _inventoryButton.OnTrigger += ToggleInventory;
            _equipmentButton = new Button("eq", 6 + buttonSize, 2, buttonSize, buttonSize, state);
            _equipmentButton.OnTrigger += ToggleEquipment;
            _statsButton = new Button("st", 10 + (buttonSize * 2), 2, buttonSize, buttonSize, state);
            _statsButton.OnTrigger += ToggleStats;

            this.AddControl(_inventoryButton);
            this.AddControl(_equipmentButton);
            this.AddControl(_statsButton);

        }

        private void ToggleInventory()
        {
            _gameState.ToggleInventory();
        }

        private void ToggleEquipment()
        {
            _gameState.ToggleEquipmentPanel();
        }

        private void ToggleStats()
        {
            _gameState.ToggleStatsPanel();
        }
    }
}
