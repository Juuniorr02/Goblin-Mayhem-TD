using Godot;

public partial class Bullet : Area2D
{
    [Export] public float Speed = 600.0f;
    public Vector2 Direction = Vector2.Zero;

    public override void _Process(double delta)
    {
        // Si no hay dirección, no se mueve
        if (Direction == Vector2.Zero) return;

        // Movimiento lineal simple
        GlobalPosition += Direction * Speed * (float)delta;
    }

    // Conecta la señal 'body_entered' desde el editor o aquí
    public override void _Ready()
    {
        BodyEntered += OnEnemyHit;
    }

    private void OnEnemyHit(Node2D body)
    {
        // Aquí irá la lógica de daño al enemigo más adelante
        GD.Print("¡Bala impactó contra: " + body.Name + "!");
        QueueFree(); // Eliminar la bala al chocar
    }
}
