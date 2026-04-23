using Godot;
using System;

public partial class Tuna : CharacterBody2D
{
    [ExportGroup("Visuals")]
    [Export] public PackedScene ExplosionScene; // Arrastra aquí tu ExplosionEffect.tscn

    [ExportGroup("Settings")]
    [Export] public float Speed = 350f;
    [Export] public float ExplosionRadius = 80f;
    
    public Node2D Target;
    public Vector2 HomePosition;
    private float _damage;
    private bool _isReady = false;
    private uint _enemyMask;

    public void SetupTuna(Vector2 pos, float damage)
    {
        HomePosition = pos;
        _damage = damage;
        _isReady = true;
    }

    private void OnDetectionAreaBodyEntered(Node2D body)
    {
        if (!_isReady) return;
        if (body.HasMethod("TakeDamage"))
        {
            if (body is CollisionObject2D col) _enemyMask = col.CollisionLayer;
            Explode();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_isReady) return;
        Vector2 velocity = Vector2.Zero;

        if (IsInstanceValid(Target) && Target.IsInsideTree())
            velocity = (Target.GlobalPosition - GlobalPosition).Normalized() * Speed;
        else if (GlobalPosition.DistanceTo(HomePosition) > 15f)
            velocity = (HomePosition - GlobalPosition).Normalized() * (Speed * 0.5f);

        Velocity = velocity;
        MoveAndSlide();
    }

    private void Explode()
    {
        // 1. INSTANCIAR EL EFECTO VISUAL
        if (ExplosionScene != null)
        {
            var effect = ExplosionScene.Instantiate<Node2D>();
            // Lo añadimos a la escena principal, no a la tuna (porque la tuna va a morir)
            GetTree().CurrentScene.AddChild(effect);
            effect.GlobalPosition = GlobalPosition;
        }

        // 2. DAÑO EN ÁREA
        var spaceState = GetWorld2D().DirectSpaceState;
        var query = new PhysicsShapeQueryParameters2D();
        var circle = new CircleShape2D();
        circle.Radius = ExplosionRadius;
        query.Shape = circle;
        query.Transform = GlobalTransform;
        query.CollisionMask = _enemyMask != 0 ? _enemyMask : (uint)2;

        var results = spaceState.IntersectShape(query);
        foreach (var result in results)
        {
            var collider = (Node2D)result["collider"];
            if (IsInstanceValid(collider) && collider.HasMethod("TakeDamage"))
            {
                collider.Call("TakeDamage", _damage);
            }
        }

        QueueFree();
    }
}
