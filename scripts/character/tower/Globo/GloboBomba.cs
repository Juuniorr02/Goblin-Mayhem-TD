using Godot;

public partial class GloboBomba : Node2D
{
    [Export] public float FallSpeed = 400.0f;
    [Export] public float ExplosionRadius = 100.0f;
    [Export] public PackedScene ExplosionEffect;

    private Vector2 _targetFloorPos;
    private float _damage;
    private bool _exploded = false;

    public void Launch(Vector2 targetPos, float damageValue)
    {
        _targetFloorPos = targetPos;
        _damage = damageValue;
    }

    public override void _Process(double delta)
    {
        if (_exploded) return;

        // La bomba cae verticalmente o hacia el punto del suelo
        GlobalPosition = GlobalPosition.MoveToward(_targetFloorPos, FallSpeed * (float)delta);

        // Si llega al suelo (o al enemigo)
        if (GlobalPosition.DistanceTo(_targetFloorPos) < 10.0f)
        {
            Explode();
        }
    }

    private void Explode()
    {
        _exploded = true;

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
        query.Shape = new CircleShape2D { Radius = ExplosionRadius };
        query.Transform = GlobalTransform;
        query.CollisionMask = 1; // Capa de enemigos

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
