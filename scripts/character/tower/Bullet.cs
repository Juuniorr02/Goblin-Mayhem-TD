using Godot;

public partial class Bullet : Area2D
{
    [Export] public float Speed = 600.0f;
    [Export] public float LifeTime = 3.0f;

    public Vector2 Direction = Vector2.Zero;
    public float Damage = 10f;

    private float lifeTimer = 0f;

    public override void _Ready()
    {
        // Conectamos la señal de colisión
        BodyEntered += OnEnemyHit;
        // También conectamos AreaEntered por si tus enemigos son áreas
        AreaEntered += OnEnemyHit;
    }

    public override void _Process(double delta)
    {
        if (Direction != Vector2.Zero)
        {
            GlobalPosition += Direction * Speed * (float)delta;
        }

        lifeTimer += (float)delta;
        if (lifeTimer >= LifeTime)
            QueueFree();
    }

    private void OnEnemyHit(Node body)
    {
        // 1. IGNORAR SI ES UNA TORRE (Para que no desaparezca al nacer)
        if (body.Name.ToString().Contains("Tower") || body is BaseTower)
            return;

        // 2. COMPROBAR SI ES ENEMIGO
        if (!body.IsInGroup("enemies"))
            return;

        GD.Print($"[IMPACTO CANNON] Golpeó a: {body.Name}");

        if (body.HasMethod("TakeDamage"))
        {
            body.Call("TakeDamage", Damage);
        }

        QueueFree(); // Desaparece solo al dar al enemigo
    }
}
