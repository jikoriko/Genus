using System;

using Genus2D.GUI;
using Genus2D.Entities;

using OpenTK;
using Genus2D.Utililities;
using Genus2D.Core;

namespace Genus2DTest
{
    public class TestState : State
    {

        float timer = 0.3f;

        SpriteComponent spriteComponent;

        public TestState()
            : base()
        {
            //*
            ScrollPanel scrollPanel = new ScrollPanel(10, 400, 400, 400, Panel.BarMode.Close_Drag, this);
            scrollPanel.SetScrollDimensions(1000, 1000);

            TextBox textBox = new TextBox(10, 10, 290, 310, this);
            scrollPanel.AddControl(textBox);

            Button button = new Button("Button", 420, 10, 80, 32, this);
            button.OnTrigger += ButtonPress;
            scrollPanel.AddControl(button);

            TextField textField = new TextField(420, 52, 100, 32, this);
            scrollPanel.AddControl(textField);

            NumberControl numberControl = new NumberControl(420, 94, this);
            numberControl.SetMinimum(10);
            numberControl.SetMaximum(20);
            scrollPanel.AddControl(numberControl);

            RadioButton radioButton = new RadioButton(520, 100, this);
            scrollPanel.AddControl(radioButton);

            string[] items = new string[]
            {
                "Item 1", "Item 2", "Item 3", "Item 4", "Item 5", "Item 6", "Item 7", "Item 8", "Item 9", "Item 10", "Item 11"
            };
            DropDownBox dropDownBox = new DropDownBox(420, 146, 120, items, this);
            scrollPanel.AddControl(dropDownBox);

            string[] menuOptions = new string[]
            {
                "Item 1", "Item 2", "Item 3", "Item 4", "Item 5", "Item 6", "Item 7", "Item 8", "Item 9", "Item 10"
            };
            DropDownMenu dropDownMenu = new DropDownMenu(560, 146, 120, "Test Menu", menuOptions, this);
            scrollPanel.AddControl(dropDownMenu);

            ListBox listBox = new ListBox(310, 10, 100, 200, 10, this);
            scrollPanel.AddControl(listBox);

            string[] radioItems = new string[]
            {
                "option 1", "option 2", "option 3"
            };
            RadioControl radioControl = new RadioControl(550, 10, radioItems, this);
            scrollPanel.AddControl(radioControl);

            Label label = new Label(550, 80, 100, 60, this);
            label.SetText("A label." + '\n' + "Line 2.");
            scrollPanel.AddControl(label);

            this.AddControl(scrollPanel);

            string message = "";
            for (int i = 0; i < 20; i++) message += "this is a message box" + '\n';
            MessageBox messageBox = new MessageBox(message, this);
            this.AddControl(messageBox);

            Entity entity = Entity.CreateInstance(this.EntityManager, Vector3.Zero);
            entity.SetCollisionSize(100, 100);
            entity.Collider.Draggable = true;
            this.EntityManager.AddEntity(entity);

            Entity entity2 = Entity.CreateInstance(this.EntityManager, new Vector3(400, 600, 0));

            spriteComponent = new SpriteComponent(entity2);
            spriteComponent.SetXFrames(3);
            spriteComponent.Transform.Parent = entity.GetTransform();
            spriteComponent.SetTexture(Assets.GetTexture("sprite.png"));
            spriteComponent.SetSpriteCenter(SpriteComponent.SpriteCenter.Top);
            entity2.SetCollisionSize(spriteComponent.GetFrameWidth(), spriteComponent.GetFrameHeight());
            entity2.SetColliderPosition(new Vector2(-spriteComponent.GetFrameWidth() / 2, -spriteComponent.GetFrameHeight()));
            entity2.Collider.Draggable = true;

            //*/


        }

        private void ButtonPress()
        {
            Console.WriteLine("button press.");
        }

        public override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == OpenTK.Input.Key.W)
            {
            }

        }

    }
}
