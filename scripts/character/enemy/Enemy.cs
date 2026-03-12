using Godot;

public partial class Enemy : Node2D
{
    [Export] public float Speed = 120f;
    [Export] public int DamageToBase = 1;

    private PathFollow2D follow;
    private bool finished = false;

    public override void _Ready()
    {
        follow = GetParent<PathFollow2D>();
        ZIndex = 1;

        if (follow == null)
            GD.PrintErr("Enemy: no se encontró PathFollow2D padre.");
    }

    public override void _Process(double delta)
    {
        if (follow == null || finished)
            return;

        follow.Progress += Speed * (float)delta;

        float pathLength = follow.GetParent<Path2D>().Curve.GetBakedLength();
        if (follow.Progress >= pathLength)
        {
            finished = true;
            GD.Print($"Enemigo llegó a la base y hace {DamageToBase} daño!");

            // Daño a la base aquí si quieres
            // Base.ReduceHealth(DamageToBase);

            // Eliminar enemigo y su PathFollow2D
            follow.QueueFree();
            QueueFree();
        }
    }
}