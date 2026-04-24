using Godot;

public partial class Martillo : Sprite2D
{
    private float _radiusX = 0f;
    private float _radiusY = 0f;

    // Ahora recibimos dos valores para el óvalo
    public void UpdateRangeVisual(float rx, float ry)
    {
        _radiusX = rx;
        _radiusY = ry;
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (_radiusX > 0 && _radiusY > 0)
        {
            Color color = Modulate;
            color.A = 0.3f;

            float ratio = _radiusY / _radiusX;
            
            DrawSetTransform(Vector2.Zero, 0, new Vector2(1.0f, ratio));
            
            DrawCircle(Vector2.Zero, _radiusX, color);
            DrawArc(Vector2.Zero, _radiusX, 0, Mathf.Tau, 64, color, 2.0f / (1.0f/ratio), true);
            
            DrawSetTransform(Vector2.Zero, 0, Vector2.One);
        }
    }
}
