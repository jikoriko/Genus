using System;

using Genus2D.Utililities;
using Genus2D.Graphics;
using Genus2D.Core;

using OpenTK;
using OpenTK.Graphics;

namespace Genus2D.GUI
{
    public class PanelBar : Panel
    {
        public static readonly int BAR_HEIGHT = 30;

        private Panel _panel;
        private Button _closeButton;

        private Color4 _buttonBackgroundColour = new Color4(255, 70, 71, 255);
        private Color4 _buttonBorderColour = new Color4(210, 25, 44, 255);

        private string _label;

        public PanelBar(int x, int y, int width, State state, Panel panel)
            : base(x, y, width, BAR_HEIGHT, BarMode.Empty, state)
        {
            _borderColour = Renderer.GetDarkerColour(_borderColour);
            _backgroundColour = _borderColour;

            _panel = panel;
            if (_panel.IsClosable())
            {
                int ButtonSize = GetContentHeight();
                int buttonY = (GetContentHeight() / 2) - (ButtonSize / 2);
                _closeButton = new Button("", GetContentWidth() - ButtonSize, buttonY, ButtonSize, ButtonSize, state);
                _closeButton.SetButtonImage(Assets.GetTexture("GUI_Textures/Cross.png"));
                _closeButton.SetImageColour(Color4.LightYellow);
                _closeButton.SetBorderColour(_buttonBorderColour);
                _closeButton.SetBackgroundColour(_buttonBackgroundColour);
                this.AddControl(_closeButton);
                _closeButton.OnTrigger += ClosePanel;
                _closeButton.SetBackgroundGradientMode(Renderer.GradientMode.RadialFit);
            }
            _strokeBorder = true;
            _backgroundGradientMode = Renderer.GradientMode.VerticalMidBand;
            _label = "";
        }

        public void SetLabel(string label)
        {
            _label = label;
        }

        public override void SetSize(int width, int height)
        {
            height = BAR_HEIGHT;
            base.SetSize(width, height);
            OnResize();
        }

        public void OnResize()
        {
            if (_closeButton != null)
            {
                int buttonY = (GetContentHeight() / 2) - (GetContentHeight() / 2);
                _closeButton.SetPosition(GetContentWidth() - GetContentHeight(), buttonY);
            }
        }

        protected void ClosePanel()
        {
            if (_closeButton != null)
            {
                _panel.Close();
            }
        }

        public override void Move(int x, int y)
        {
            //base.Move(x, y);
            _panel.Move(x, y);
        }

        public override void OnMouseMove(OpenTK.Input.MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            if (_panel.IsDraggable())
            {
                if (_pressed)
                {
                    Vector2 movement = StateWindow.Instance.GetMouseMovement();
                    this.Move((int)movement.X, (int)movement.Y);
                }
            }
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            if (_label != "")
            {
                int x = 10;
                int y = (GetContentHeight() / 2) - (Renderer.GetFont().GetTextHeight(_label) / 2);
                Vector3 pos = new Vector3(x, y, 0);
                Color4 colour = Color4.White;
                Renderer.PrintText(_label, ref pos, ref colour);
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            if (_closeButton != null)
                _closeButton.OnTrigger -= ClosePanel;
        }

    }
}
