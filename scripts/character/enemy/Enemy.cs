using Godot;


public partial class Enemy : CharacterBody2D 
{
    [Export] public bool IsFacingLeftByDefault = false;
    [Export] public AnimatedSprite2D sprite;
    [Export] public EnemyData Data;
    [Export] public TextureProgressBar HealthBar; 
    [Export] public Vector2 HealthBarOffset = new Vector2(-50, -60); // Ajusta esto para centrarla

    private PathFollow2D follow;
    private Path2D path;
    private bool finished = false;

    [Export] public float Health = 50f;
    private float maxHealth;

    public override void _Ready()
    {
        follow = GetParent<PathFollow2D>();
        path = follow?.GetParent<Path2D>();
        maxHealth = Health;

        if (HealthBar != null)
        {
            HealthBar.MaxValue = maxHealth;
            HealthBar.Value = Health;
            HealthBar.Visible = false;
            HealthBar.TopLevel = true; // Forzamos que ignore escalas del padre
            HealthBar.ZIndex = 100;
        }

        if (Data != null)
            ZIndex = Data.IsFlying ? 5 : 1;
    }

public override void _Process(double delta)
{
    if (follow == null || path == null || finished || Data == null)
        return;

    follow.Progress += Data.Speed * (float)delta;

    if (HealthBar != null && HealthBar.Visible)
    {
        HealthBar.GlobalPosition = GlobalPosition + HealthBarOffset;
    }

    Vector2 direccion = GetManualDirection(path, follow.Progress);
    
    // 2. Lógica de volteo ajustada
    if (direccion.X != 0)
    {
        // Si el enemigo mira a la izquierda por defecto, invertimos la lógica del Flip
        bool mirandoDerecha = direccion.X > 0;
        sprite.FlipH = IsFacingLeftByDefault ? mirandoDerecha : !mirandoDerecha;
    }

    QuitarVidaBase();
}

    public void TakeDamage(float amount)
    {
        Health -= amount;
        
        if (HealthBar != null)
        {
            HealthBar.Visible = true;
            HealthBar.Value = Health;

            float healthPercent = Mathf.Clamp(Health / maxHealth, 0, 1);
            HealthBar.TintProgress = new Color(1 - healthPercent, healthPercent, 0);
        }
        
        if (Health <= 0) EliminarEnemigo();
    }

    public void QuitarVidaBase()
    {
        float pathLength = path.Curve.GetBakedLength();
        if (follow.Progress >= pathLength)
        {
            finished = true;
            if (Base.Instance != null) Base.Instance.Health -= Data.DamageToBase;
            EliminarEnemigo();
        }
    }

    public Vector2 GetManualDirection(Path2D path, float progress)
    {
        Curve2D curve = path.Curve;
        return (curve.SampleBaked(progress - 1.0f) - curve.SampleBaked(progress)).Normalized();
    }

    private void EliminarEnemigo()
    {
        if (follow != null) follow.QueueFree();
        QueueFree();
    }
}
