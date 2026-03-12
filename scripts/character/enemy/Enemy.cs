using Godot;

public partial class Enemy : Node2D
{
    [Export] public float Speed = 120f;
    [Export] public int DamageToBase = 1;
    [Export] public AnimatedSprite2D sprite; 

    private PathFollow2D follow;
    private Path2D path; // Referencia al Path2D para acceder a la curva
    private bool finished = false;

    public override void _Ready()
    {
        follow = GetParent<PathFollow2D>();
        ZIndex = 1;

        if (follow == null)
        {
            GD.PrintErr("Enemy: no se encontró PathFollow2D padre.");
        }
        else
        {
            // El Path2D suele ser el padre del PathFollow2D
            path = follow.GetParent<Path2D>();
        }
    }

public override void _Process(double delta)
{
    if (follow == null || path == null || finished)
        return;

    follow.Progress += Speed * (float)delta;

    Vector2 direccion = GetManualDirection(path, follow.Progress);

    // Si X es positivo, va a la derecha. Si es negativo, a la izquierda.
    if (direccion.X > 0)
    {
        sprite.FlipH = false;  // Mirar a la derecha (normal)
    }
    else if (direccion.X < 0)
    {
        sprite.FlipH = true;// Mirar a la izquierda (espejo)
    }

    // Lógica de llegada a la base
    float pathLength = path.Curve.GetBakedLength();
    if (follow.Progress >= pathLength)
    {
        finished = true;
        follow.QueueFree();
        QueueFree();
    }
}

    public Vector2 GetManualDirection(Path2D path, float progress)
    {
        Curve2D curve = path.Curve;
        Vector2 posActual = curve.SampleBaked(progress);
        
        // Miramos 1 píxel más adelante
        Vector2 posSiguiente = curve.SampleBaked(progress + 1.0f);
        
        // Retornamos el vector normalizado (dirección pura)
        return (posSiguiente - posActual).Normalized();
    }
}
