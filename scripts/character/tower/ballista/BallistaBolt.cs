using System;
using Godot;

public partial class BallistaBolt : Area2D
{
	[Export] public float Speed = 1000.0f; // Más rápida que una flecha
	[Export] public int PiercingPower = 3;  // A cuántos atraviesa
	[Export] public float LifeTimeAfterStop = 2.0f;
	[Export] public float MaxFlightTime = 1.5f;

	private Vector2 direction;
	private bool isStopped = false;
	private float Damage;
	private float flightTimer = 0.0f;
	private int enemiesHit = 0;

	public void Launch(Vector2 dir, float damageValue)
	{
		Damage = damageValue;
		direction = dir.Normalized();
		Rotation = direction.Angle() + Mathf.Pi; // Mismo offset visual que tu flecha
	}

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	public override void _Process(double delta)
	{
		if (isStopped) return;

		flightTimer += (float)delta;
		if (flightTimer >= MaxFlightTime)
		{
			StopBolt();
			return;
		}

		// Movimiento recto (sin gravedad para la Ballista)
		GlobalPosition += direction * Speed * (float)delta;
	}

	private void OnBodyEntered(Node body)
	{
		if (isStopped) return;

		// 1. Lógica de impacto con enemigos
		if (body.IsInGroup("enemies"))
		{
			GD.Print($"[BALLISTA] Perforando a: {body.Name}");
			if (body.HasMethod("TakeDamage"))
			{
				body.Call("TakeDamage", Damage);
			}

			enemiesHit++;
			// Si ya perforó el límite, se queda clavada en el último enemigo
			if (enemiesHit >= PiercingPower)
			{
				StopBolt();
			}
			return; // Importante para que no ejecute la lógica de "suelo"
		}

		// 2. Lógica de clavarse en el mapa
		bool isMap = body is TileMapLayer || body.Name.ToString().Contains("map");
		if (isMap && !body.Name.ToString().Contains("Tower")) 
		{
			StopBolt();
		}
	}

	private async void StopBolt()
	{
		if (isStopped) return;
		isStopped = true;

		SetDeferred("monitoring", false);
		SetDeferred("monitorable", false);

		// Esperar antes de borrar (clavada)
		await ToSignal(GetTree().CreateTimer(LifeTimeAfterStop), "timeout");
		QueueFree();
	}
}
