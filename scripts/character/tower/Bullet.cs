using Godot;

public partial class Bullet : Area2D
{
    [Export] public float Speed = 600.0f;
    [Export] public float LifeTime = 3.0f;

    public Vector2 Direction = Vector2.Zero;
    public float Damage = 10f;

    private float lifeTimer = 3f;

    public override void _Ready()
    {
        BodyEntered += OnEnemyHit;
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
        // Corregido al grupo "enemies" en minúscula
        if (!body.IsInGroup("enemies"))
            return;

        GD.Print($"[IMPACTO] Bala golpeó a: {body.Name}");

        if (body.HasMethod("TakeDamage"))
        {
            body.Call("TakeDamage", Damage);
        }

        QueueFree();
    }
}
