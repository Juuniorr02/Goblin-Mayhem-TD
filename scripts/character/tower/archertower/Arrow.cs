using System;
using Godot;

public partial class Arrow : Area2D
{
    [Export] public float Speed = 600.0f;
    [Export] public float GravityForce = 800.0f; 
    [Export] public float LifeTimeAfterStop = 2.0f; // Tiempo antes de desaparecer tras frenar
    [Export] public float MaxFlightTime = 1.0f;    // Tiempo máximo de vuelo

    private Vector2 velocity;
    private bool isStopped = false;
    private float Damage;
    private float flightTimer = 0.0f;

    public void Launch(Vector2 direction, float damageValue)
    {
        Damage = damageValue;
        velocity = direction * Speed;
        Rotation = velocity.Angle();
    }

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    public override void _Process(double delta)
    {
        if (isStopped) return;

        // Contador de tiempo de vuelo
        flightTimer += (float)delta;
        if (flightTimer >= MaxFlightTime)
        {
            StopArrow();
            return;
        }

        // Física
        velocity.Y += Gravity * (float)delta;
        GlobalPosition += velocity * (float)delta;
        Rotation = velocity.Angle() + Mathf.Pi; 
    }

	private void OnBodyEntered(Node body)
	{
		if (isStopped) return;

        // Corregido al grupo "enemies" en minúscula
        if (!body.IsInGroup("enemies"))
            return;

        GD.Print($"[IMPACTO] Bala golpeó a: {body.Name}");

        if (body.HasMethod("TakeDamage"))
        {
            body.Call("TakeDamage", Damage);
        }

        QueueFree();

		// 2. ¿Es el suelo? (Solo si NO es la torre y NO es un enemigo)
		bool isMap = body is TileMapLayer || body.Name.ToString().Contains("map");
		if (isMap && !body.Name.ToString().Contains("Tower")) 
		{
			StopArrow();
		}
	}

    private async void StopArrow()
    {
        if (isStopped) return;
        isStopped = true;
        

        // Desactivar colisiones para que no afecte a nada más
        SetDeferred("monitoring", false);
        SetDeferred("monitorable", false);

        // Esperar antes de borrar
        await ToSignal(GetTree().CreateTimer(LifeTimeAfterStop), "timeout");
        QueueFree();
    }
}
