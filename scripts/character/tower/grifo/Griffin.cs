using Godot;
using System;

public partial class Griffin : CharacterBody2D
{
	[Export] public float Speed = 250f;
	[Export] public float IdleRadius = 25f;

	public Node2D Target;
	public Vector2 HomePosition;
	
	private AnimatedSprite2D _sprite;
	private Vector2 _idleTarget;
	private float _idleWaitTimer = 0f;
	private bool _isReady = false;

	public void SetupGriffin(Vector2 pos)
	{
		GlobalPosition = pos;
		HomePosition = pos;
		_idleTarget = pos;
		_sprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		_isReady = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_isReady) return;

		Vector2 targetPos;

		if (IsInstanceValid(Target))
		{
			// MODO ATAQUE: Revoloteo circular suave
			float time = Time.GetTicksMsec() / 1000f;
			Vector2 offset = new Vector2(Mathf.Cos(time * 4f), Mathf.Sin(time * 4f)) * 35f;
			targetPos = Target.GlobalPosition + offset;
			_idleWaitTimer = 0f;
		}
		else
		{
			// MODO NIDO: Patrulla controlada
			if (GlobalPosition.DistanceTo(_idleTarget) < 10f)
			{
				_idleWaitTimer += (float)delta;
				if (_idleWaitTimer > 2.5f) 
				{
					_idleTarget = HomePosition + new Vector2(
						(float)GD.RandRange(-IdleRadius, IdleRadius),
						(float)GD.RandRange(-IdleRadius, IdleRadius)
					);
					_idleWaitTimer = 0f;
				}
				Velocity = Vector2.Zero;
				targetPos = GlobalPosition;
			}
			else
			{
				targetPos = _idleTarget;
			}
		}

		MoverHacia(targetPos);
	}

	private void MoverHacia(Vector2 targetPos)
	{
		if (GlobalPosition.DistanceTo(targetPos) > 5f)
		{
			Vector2 direction = (targetPos - GlobalPosition).Normalized();
			Velocity = direction * Speed;
			MoveAndSlide();

			if (_sprite != null && Mathf.Abs(Velocity.X) > 1f)
			{
				_sprite.FlipH = Velocity.X < 0;
			}
		}
	}
}
