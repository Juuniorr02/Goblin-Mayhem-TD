using Godot;

public partial class MageShot : Node2D
{
    [Export] public float Speed = 500.0f;
    [Export] public float ExplosionRadius = 80.0f;
    [Export] public PackedScene ExplosionEffect;

    private Vector2 _targetPos;
    private float _damage;
    private bool _reachedTarget = false;

    public void Launch(Vector2 target, float damageValue)
    {
        _targetPos = target;
        _damage = damageValue;
        
        // Rotar hacia el objetivo
        LookAt(_targetPos);
    }

    public override void _Process(double delta)
    {
        if (_reachedTarget) return;

        // Moverse directamente hacia el punto objetivo
        GlobalPosition = GlobalPosition.MoveToward(_targetPos, Speed * (float)delta);

        // Si estamos lo suficientemente cerca del punto de impacto
        if (GlobalPosition.DistanceTo(_targetPos) < 5.0f)
        {
            OnImpact();
        }
    }

    private void OnImpact()
    {
        _reachedTarget = true;

        // Visual de explosión
        if (ExplosionEffect != null)
        {
            var exp = ExplosionEffect.Instantiate<Node2D>();
            GetTree().CurrentScene.AddChild(exp);
            exp.GlobalPosition = GlobalPosition;
        }

        ApplyAreaDamage();
        QueueFree();
    }

    private void ApplyAreaDamage()
    {
        var spaceState = GetWorld2D().DirectSpaceState;
        var query = new PhysicsShapeQueryParameters2D();
        var circle = new CircleShape2D { Radius = ExplosionRadius };
        
        query.Shape = circle;
        query.Transform = GlobalTransform;
        query.CollisionMask = 1; // Ajusta según tu capa de enemigos

        var results = spaceState.IntersectShape(query);
        foreach (var result in results)
        {
            var body = result["collider"].As<Node>();
            if (body.IsInGroup("enemies") && body.HasMethod("TakeDamage"))
            {
                body.Call("TakeDamage", _damage);
            }
        }
    }
}
