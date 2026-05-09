using Godot;

public partial class Martillo : Sprite2D
{
    private float _radiusX = 0f;
    private const float IsometricRatio = 0.5f;

    // Aceptamos rx y ry para que TowerBuilder no de error, 
    // pero calculamos el óvalo nosotros para que sea isométrico real.
    public void UpdateRangeVisual(float rx, float ry = 0)
    {
        _radiusX = rx;
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (_radiusX <= 0) return;

        Color fill = Modulate; fill.A = 0.3f;
        Color line = Modulate; line.A = 0.7f;

        int puntos = 64;
        Vector2[] vertices = new Vector2[puntos];
        
        for (int i = 0; i < puntos; i++)
        {
            float angle = i * Mathf.Tau / puntos;
            vertices[i] = new Vector2(
                Mathf.Cos(angle) * _radiusX,
                Mathf.Sin(angle) * (_radiusX * IsometricRatio)
            );
        }

        DrawPolygon(vertices, new Color[] { fill });
        for (int i = 0; i < puntos; i++)
        {
            DrawLine(vertices[i], vertices[(i + 1) % puntos], line, 2.0f, true);
        }
    }
}
