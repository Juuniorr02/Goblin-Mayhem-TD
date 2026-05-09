using Godot;
using System;

public partial class Tuna : CharacterBody2D
{
    [ExportGroup("Visuals")]
    [Export] public PackedScene ExplosionScene;
    [Export] public AnimatedSprite2D MyAnimation; // Referencia al sprite

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

        // Buscamos el nodo de animación al nacer
        if (MyAnimation == null)
            MyAnimation = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
        
        // Iniciamos la animación de nado
        MyAnimation?.Play("default");
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

        // --- LÓGICA VISUAL ---
        if (Velocity.Length() > 0.1f)
        {
            // Girar el sprite según la dirección (opcional, si tus sprites miran a la derecha)
            // Rotation = Velocity.Angle(); 
            
            // O simplemente hacer FlipH si prefieres que solo mire izquierda/derecha
            if (MyAnimation != null)
                MyAnimation.FlipH = Velocity.X < 0;
            
            // Asegurarnos de que siempre esté nadando (default)
            if (MyAnimation?.IsPlaying() == false || MyAnimation?.Animation != "default")
                MyAnimation?.Play("default");
        }
    }

    private void Explode()
    {
        if (ExplosionScene != null)
        {
            var effect = ExplosionScene.Instantiate<Node2D>();
            GetTree().CurrentScene.AddChild(effect);
            effect.GlobalPosition = GlobalPosition;
        }

        var spaceState = GetWorld2D().DirectSpaceState;
        var query = new PhysicsShapeQueryParameters2D();
        var circle = new CircleShape2D { Radius = ExplosionRadius };
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
