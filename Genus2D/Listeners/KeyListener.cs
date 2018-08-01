
namespace Genus2D.Listeners
{
    public interface KeyListener
    {
        void OnKeyPress(OpenTK.KeyPressEventArgs e);
        void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e);
        void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e);
    }
}
