using Godot;

public partial class Flame : Area2D
{
    [Export] public float Speed = 350.0f;
    [Export] public float LifeTime = 0.5f; // Alcance del chorro
    [Export] public float BurnDamage = 3.0f; // Daño por segundo de la quemadura
    
    public Vector2 Direction = Vector2.Zero;
    public float Damage = 2.0f; // Daño inmediato al tocar
    private float _timer = 0f;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        AreaEntered += OnBodyEntered;
    }

    public override void _Process(double delta)
    {
        GlobalPosition += Direction * Speed * (float)delta;
        _timer += (float)delta;
        if (_timer >= LifeTime) QueueFree();
    }

    private void OnBodyEntered(Node body)
    {
        if (body.IsInGroup("enemies") && body is Enemy enemy)
        {
            // 1. Daño inmediato
            enemy.TakeDamage(Damage);

            // 2. Aplicar efecto de quemadura (DoT)
            ApplyBurnEffect(enemy);
        }
    }

    private void ApplyBurnEffect(Enemy enemy)
    {
        // Buscamos si ya tiene un efecto para resetearlo o añadirlo
        BurnEffect effect = enemy.GetNodeOrNull<BurnEffect>("BurnEffect");
        if (effect != null)
        {
            effect.Reset();
        }
        else
        {
            var newEffect = new BurnEffect(BurnDamage, 3.0f); // 3 segundos de fuego
            newEffect.Name = "BurnEffect";
            enemy.AddChild(newEffect);
        }
    }

    public void Launch(Vector2 direction, float damageValue)
    {
        Direction = direction;
        Damage = damageValue;
    }
}
