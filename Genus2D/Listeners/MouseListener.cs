
namespace Genus2D.Listeners
{
    public interface MouseListener
    {
        void OnMouseMove(OpenTK.Input.MouseMoveEventArgs e);
        void OnMouseDown(OpenTK.Input.MouseButtonEventArgs e);
        void OnMouseUp(OpenTK.Input.MouseButtonEventArgs e);
        void OnMouseWheel(OpenTK.Input.MouseWheelEventArgs e);
    }
}
