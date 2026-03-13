using Godot;

public partial class Enemy : Node2D
{
    [Export] public AnimatedSprite2D sprite;
    [Export] public EnemyData Data;

    private PathFollow2D follow;
    private Path2D path;
    private bool finished = false;

    public override void _Ready()
    {
        follow = GetParent<PathFollow2D>();
        path = follow?.GetParent<Path2D>();

        if (Data != null)
            ZIndex = Data.IsFlying ? 5 : 1;
    }

    public override void _Process(double delta)
    {
        if (follow == null || path == null || finished || Data == null)
            return;

        follow.Progress += Data.Speed * (float)delta;

        Vector2 direccion = GetManualDirection(path, follow.Progress);

        if (direccion.X > 0)
            sprite.FlipH = false;
        else if (direccion.X < 0)
            sprite.FlipH = true;

        QuitarVidaBase();
    }

    public void QuitarVidaBase()
    {
        float pathLength = path.Curve.GetBakedLength();

        if (follow.Progress >= pathLength)
        {
            finished = true;

            GD.Print(Data.EnemyName + " llegó a la base y quitó " + Data.DamageToBase);

            follow.QueueFree();
            QueueFree();
        }
    }

    public Vector2 GetManualDirection(Path2D path, float progress)
    {
        Curve2D curve = path.Curve;

        Vector2 posActual = curve.SampleBaked(progress);
        Vector2 posSiguiente = curve.SampleBaked(progress + 1.0f);

        return (posSiguiente - posActual).Normalized();
    }
}