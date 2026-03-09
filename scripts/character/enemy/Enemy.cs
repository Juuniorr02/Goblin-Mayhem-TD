using Godot;

public partial class Enemy : Node2D
{
    [Export] public float Speed = 100f; // píxeles por segundo
    [Export] public int DamageToBase = 1;

    private Vector2[] path;
    private int currentIndex = 0;

    public void SetPath(Vector2[] path)
    {
        if (path == null || path.Length == 0)
            return;

        this.path = path;
        currentIndex = 0;

        // Centrar al enemigo en el primer tile
        GlobalPosition = path[0];
        ZIndex = 1;
        GD.Print($"Enemigo spawn en {GlobalPosition}, comenzando path de {path.Length} puntos");
    }

    public override void _Process(double delta)
    {
        if (path == null || currentIndex >= path.Length)
            return;

        Vector2 target = path[currentIndex];
        Vector2 dir = (target - GlobalPosition).Normalized();
        float step = Speed * (float)delta;

        // Moverse hacia el siguiente tile
        if ((target - GlobalPosition).Length() <= step)
        {
            GlobalPosition = target;
            currentIndex++;

            if (currentIndex < path.Length)
                GD.Print($"Enemigo llegó a tile {path[currentIndex-1]} -> siguiente {path[currentIndex]}");
            else
                GD.Print($"Enemigo llegó al final del path en {target}");
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