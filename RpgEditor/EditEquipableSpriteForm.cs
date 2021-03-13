using Genus2D.GameData;
using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgEditor
{
    public partial class EditEquipableSpriteForm : Form
    {

        public static EditEquipableSpriteForm Insance { get; private set; }

        private EditorForm _editor;
        private Genus2D.GameData.ItemData _itemData;
        private ItemSpriteViewerPanel _spriteViewerPanel;

        public EditEquipableSpriteForm(EditorForm editor, Genus2D.GameData.ItemData itemData)
        {
            Insance = this;

            InitializeComponent();
            _editor = editor;
            _itemData = itemData;

            _spriteViewerPanel = new ItemSpriteViewerPanel(_itemData);
            _spriteViewerPanel.Size = SpriteViewerPanel.Size;
            SpriteViewerPanel.Controls.Add(_spriteViewerPanel);

            PopulateSpriteSelections();
            PopulateDirectionSelections();
            SetAnchors();
        }

        public FacingDirection GetSelectedDirection()
        {
            return (FacingDirection)DirectionSelection.SelectedIndex;
        }

        public int GetSelectedFrame()
        {
            return (int)FrameSelection.Value - 1;
        }

        public Vector2 GetSelectedAnchor()
        {
            return new Vector2((int)AnchorXSelection.Value, (int)AnchorYSelection.Value);
        }

        private void PopulateSpriteSelections()
        {
            SpriteSelection.Items.Clear();
            SpriteSelection.Items.Add("None");

            if (Directory.Exists("Assets/Textures/Sprites"))
            {
                string[] files = Directory.GetFiles("Assets/Textures/Sprites", "*.png");
                for (int i = 0; i < files.Length; i++)
                {
                    files[i] = Path.GetFileName(files[i]);
                }
                SpriteSelection.Items.AddRange(files);
            }

            SpriteSelection.SelectedIndex = 0;
            for (int i = 1; i < SpriteSelection.Items.Count; i++)
            {
                if (SpriteSelection.Items[i].ToString() == _itemData.EquipableSprite)
                {
                    SpriteSelection.SelectedIndex = i;
                    break;
                }
            }
        }

        private void PopulateDirectionSelections()
        {
            for (int i = 0; i < 4; i++)
            {
                DirectionSelection.Items.Add(((FacingDirection)i).ToString());
            }

            DirectionSelection.SelectedIndex = 0;
        }

        private void SetAnchors()
        {
            if (SpriteSelection.SelectedIndex == 0)
            {
                AnchorXSelection.Value = 0;
                AnchorYSelection.Value = 0;
            }
            else
            {
                Vector2 anchor = _itemData.GetEquipableAnchor(GetSelectedDirection(), GetSelectedFrame());
                AnchorXSelection.Value = (int)anchor.X;
                AnchorYSelection.Value = (int)anchor.Y;
            }
        }

        private void SpriteSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SpriteSelection.SelectedIndex == 0)
            {
                _spriteViewerPanel.SetSprite(null);
            }
            else
            {
                _spriteViewerPanel.SetSprite(Image.FromFile("Assets/Textures/Sprites/" + (string)SpriteSelection.Items[SpriteSelection.SelectedIndex]));
            }
        }

        private void DirectionSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetAnchors();
        }

        private void FrameSelection_ValueChanged(object sender, EventArgs e)
        {
            SetAnchors();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            if (SpriteSelection.SelectedIndex == 0)
            {
                _itemData.EquipableSprite = "";
                for (int dir = 0; dir < 4; dir++)
                {
                    for (int frame = 0; frame < 0; frame++)
                    {
                        _itemData.SetEquipableAnchor((FacingDirection)dir, frame, new Vector2());
                    }
                }
            }
            else
            {
                _itemData.EquipableSprite = (string)SpriteSelection.Items[SpriteSelection.SelectedIndex];
                Vector2 anchor = new Vector2((int)AnchorXSelection.Value, (int)AnchorYSelection.Value);
                _itemData.SetEquipableAnchor(GetSelectedDirection(), GetSelectedFrame(), anchor);
            }
        }

        private void AnchorXSelection_ValueChanged(object sender, EventArgs e)
        {
            _spriteViewerPanel.Refresh();
        }

        private void AnchorYSelection_ValueChanged(object sender, EventArgs e)
        {
            _spriteViewerPanel.Refresh();
        }
    }
}
