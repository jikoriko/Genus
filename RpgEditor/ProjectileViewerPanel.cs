using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgEditor
{
    public class ProjectileViewerPanel : Panel
    {

        private EditorForm _editorForm;
        private Genus2D.GameData.ProjectileData _projectileData;
        private Image _iconSheetImage;

        public ProjectileViewerPanel(EditorForm editor)
        {
            _editorForm = editor;
            _projectileData = null;
            _iconSheetImage = null;
            this.DoubleBuffered = true;
        }

        public void SetProjectileData(Genus2D.GameData.ProjectileData data)
        {
            _projectileData = data;
            if (_iconSheetImage != null)
            {
                _iconSheetImage.Dispose();
                _iconSheetImage = null;
            }

            if (_projectileData != null)
            {
                if (_projectileData.IconSheetImage != "")
                {
                    _iconSheetImage = Image.FromFile("Assets/Textures/Icons/" + _projectileData.IconSheetImage);
                }
            }

            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_projectileData != null)
            {
                Rectangle dest;
                if (_iconSheetImage != null)
                {
                    Rectangle src = new Rectangle(_projectileData.IconID % 8 * 32, _projectileData.IconID / 8 * 32, 32, 32);
                    dest = new Rectangle(0, 0, 32, 32);
                    e.Graphics.DrawImage(_iconSheetImage, dest, src, GraphicsUnit.Pixel);

                    Point anchor = _editorForm.GetProjectilelAnchor();
                    Point bounds = _editorForm.GetProjectileBounds();
                    dest = new Rectangle(anchor.X - 1, anchor.Y - 1, 2, 2);
                    e.Graphics.FillRectangle(new SolidBrush(Color.Red), dest);
                    dest = new Rectangle(anchor.X - (bounds.X / 2), anchor.Y - (bounds.Y / 2), bounds.X, bounds.Y);
                    e.Graphics.DrawRectangle(new Pen(Color.Red, 2), dest);
                }
            }

        }
    }
}
