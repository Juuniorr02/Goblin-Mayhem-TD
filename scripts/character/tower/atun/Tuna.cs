using Godot;

public partial class Tuna : CharacterBody2D
{
	[Export] public float Speed = 400f;
	[Export] public float ExplosionRadius = 100f;
	public Node2D Target;
	public Vector2 HomePosition;
	public float Damage;

	private bool _isExploding = false;

	public override void _PhysicsProcess(double delta)
	{
		if (_isExploding) return;

		if (IsInstanceValid(Target))
		{
			// Movimiento directo al enemigo
			Vector2 direction = (Target.GlobalPosition - GlobalPosition).Normalized();
			Velocity = direction * Speed;
			MoveAndSlide();

			// Detectar choque (si estamos muy cerca)
			if (GlobalPosition.DistanceTo(Target.GlobalPosition) < 20f)
			{
				Explode();
			}
		}
		else
		{
			// Si no hay enemigo, vuelve a esperar cerca del criadero
			if (GlobalPosition.DistanceTo(HomePosition) > 10f)
			{
				Vector2 direction = (HomePosition - GlobalPosition).Normalized();
				Velocity = direction * Speed * 0.5f;
				MoveAndSlide();
			}
		}
	}

	private void Explode()
	{
		_isExploding = true;
		GD.Print("[TUNA] ¡BOOM! Explosión de atún.");

		// Lógica de daño en área
		var enemies = GetTree().GetNodesInGroup("enemies");
		foreach (Node enemy in enemies)
		{
			if (enemy is Node2D e2d && e2d.GlobalPosition.DistanceTo(GlobalPosition) < ExplosionRadius)
			{
				if (e2d.HasMethod("TakeDamage"))
					e2d.Call("TakeDamage", Damage);
			}
		}

		// Aquí podrías instanciar un efecto de partículas de agua/explosión
		QueueFree(); 
	}
}
