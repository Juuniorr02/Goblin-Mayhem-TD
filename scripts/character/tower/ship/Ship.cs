using Godot;

public partial class Ship : BaseTower
{
    [ExportGroup("Animaciones y Visuales")]
    [Export] public AnimatedSprite2D MyAnimation;
    [Export] public float FloatAmplitude = 4.0f; // Cuántos píxeles sube y baja
    [Export] public float FloatSpeed = 2.0f;     // Qué tan rápido flota

    private float _timePassed = 0.0f;
    private float _initialSpriteY;

    public override void _Ready()
    {
        base._Ready();

        if (MyAnimation == null)
            MyAnimation = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");

        // Guardamos la posición Y local original del sprite
        if (MyAnimation != null)
            _initialSpriteY = MyAnimation.Position.Y;

        CanTargetAir = false;
        CanTargetLand = true;
        CanTargetWater = true;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        // --- LÓGICA DE FLOTACIÓN (Agua) ---
        if (MyAnimation != null)
        {
            _timePassed += (float)delta;
            
            // Calculamos el desplazamiento vertical suave
            float newY = _initialSpriteY + (Mathf.Sin(_timePassed * FloatSpeed) * FloatAmplitude);
            
            // Aplicamos solo a la posición local del sprite para no afectar a la torre global
            MyAnimation.Position = new Vector2(MyAnimation.Position.X, newY);
        }
    }

    protected override void Shoot()
    {
        if (!IsInstanceValid(currentTarget) || BulletScene == null || muzzle == null)
            return;

        // Reproducir la animación de disparo (velas/cañón)
        MyAnimation?.Play("default");

        var bulletNode = BulletScene.Instantiate();
        GetTree().CurrentScene.AddChild(bulletNode);
        
        if (bulletNode is Node2D bullet2D)
        {
            bullet2D.GlobalPosition = muzzle.GlobalPosition;
            
            if (bulletNode is Bullet bulletScript)
            {
                Vector2 direction = (currentTarget.GlobalPosition - muzzle.GlobalPosition).Normalized();
                bulletScript.Direction = direction;
                bulletScript.Damage = Damage;
                bulletScript.Rotation = direction.Angle();
            }
            
            GD.Print("[BOAT] ¡Cañonazo desde el agua!");
        }
    }

    public override void Build()
    {
        // ... (Tu código de Build se mantiene igual)
        int amountGold = 150, amountWood = 100, amountStone = 0, amountIron = 0;
        if (Recursos.Instance.Gold >= amountGold && Recursos.Instance.Wood >= amountWood && 
            Recursos.Instance.Stone >= amountStone && Recursos.Instance.Iron >= amountIron)
        {
            Recursos.Instance.Gold -= amountGold;
            Recursos.Instance.Wood -= amountWood;
            Recursos.Instance.Stone -= amountStone;
            Recursos.Instance.Iron -= amountIron;
            CanBuild = true;
        }
        else { CanBuild = false; }
    }
}
