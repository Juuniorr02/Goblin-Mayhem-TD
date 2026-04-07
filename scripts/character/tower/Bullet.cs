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
        BodyEntered += OnEnemyHit;
    }

    public override void _Process(double delta)
    {
        // Movimiento
        if (Direction != Vector2.Zero)
        {
            GlobalPosition += Direction * Speed * (float)delta;
        }

        // Autodestrucción por tiempo
        lifeTimer += (float)delta;
        if (lifeTimer >= LifeTime)
            QueueFree();
    }

    private void OnEnemyHit(Node2D body)
    {
        // 🔹 Solo afectar enemigos
        if (!body.IsInGroup("Enemies"))
            return;

        // 🔹 Aplicar daño si existe el método
        if (body.HasMethod("TakeDamage"))
            body.Call("TakeDamage", Damage);

        GD.Print("Impacto a: " + body.Name);

        QueueFree();
    }
}