using System;

using Genus2D.Core;
using Genus2D.Graphics;

using OpenTK;
using OpenTK.Graphics;

namespace Genus2D.GUI
{
    public class RadioButton : Control
    {
        protected RadioControl _radioControl;

        public static readonly int RADIO_SIZE = 20;

        private bool _checked;

        public RadioButton(int x, int y, State state)
            : base(x, y, RADIO_SIZE, RADIO_SIZE, state)
        {
            _checked = false;
            _cornerRadius = (RADIO_SIZE / 2);
            this.OnTrigger += OnRadioTrigger;
        }

        public void SetRadioControl(RadioControl radioControl)
        {
            _radioControl = radioControl;
        }

        protected void OnRadioTrigger()
        {
            if (_radioControl == null)
                _checked = _checked ? false : true;
            else
                _radioControl.SelectButton(this);
        }

        public bool IsChecked()
        {
            return _checked;
        }

        public void SetCheck(bool check)
        {
            _checked = check;
        }

        protected override void RenderContent()
        {
            base.RenderContent();
            if (_checked)
            {
                Vector3 position = new Vector3(1, 1, 0);
                Vector3 scale = new Vector3(GetContentWidth() - 2, GetContentHeight() - 2, 1);
                Color4 colour = Color4.LimeGreen;
                Color4 endColour = Renderer.GetDarkerColour(colour);
                Renderer.SetGradientMode(Renderer.GradientMode.RadialFit);
                Renderer.FillRoundedRectangle(ref position, ref scale, _cornerRadius, ref colour, ref endColour);
                Renderer.SetGradientMode(Renderer.GradientMode.None);
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            this.OnTrigger -= OnRadioTrigger;
        }
    }
}
