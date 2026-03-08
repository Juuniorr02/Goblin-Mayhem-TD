using Godot;

public partial class Enemy : Node2D
{
    [Export] public float Speed = 100f; // píxeles por segundo
    [Export] public int DamageToBase = 1;

    private Vector2[] path;
    private int currentIndex = 0;

    public void SetPath(Vector2[] path)
    {
        this.path = path;
        if (path.Length > 0)
            GlobalPosition = path[0];

        ZIndex = 1; // para que aparezca sobre el suelo
    }

    public override void _Process(double delta)
    {
        if (path == null || currentIndex >= path.Length)
            return;

        Vector2 target = path[currentIndex];
        Vector2 dir = (target - GlobalPosition).Normalized();
        float step = Speed * (float)delta;

        if ((target - GlobalPosition).Length() <= step)
        {
            GlobalPosition = target;
            currentIndex++;
        }
        else
        {
            GlobalPosition += dir * step;
        }

        // Llegó a la base
        if (currentIndex >= path.Length)
        {
            GD.Print($"Enemigo llegó a la base y quita {DamageToBase} de vida!");
            QueueFree();
        }
    }
}