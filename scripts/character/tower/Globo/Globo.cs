using Godot;

public partial class Globo : BaseTower
{
    [ExportGroup("Movimiento del Globo")]
    [Export] public float FloatAmplitude = 10.0f;
    [Export] public float FloatSpeed = 2.0f;
    [Export] public float DriftRadius = 15.0f;
    [Export] public float DriftSpeed = 0.5f;

    [ExportGroup("Animaciones")]
    [Export] public AnimatedSprite2D MyAnimation;

    private Vector2 _anchorPosition;
    private float _timePassed = 0.0f;

    public override void _Ready()
    {
        base._Ready();
        _anchorPosition = Position;
        
        if (MyAnimation == null)
            MyAnimation = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");

        CanTargetLand = true;
        CanTargetAir = false; 
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        _timePassed += (float)delta;

        float driftX = Mathf.Sin(_timePassed * DriftSpeed) * DriftRadius;
        float driftY = Mathf.Cos(_timePassed * DriftSpeed * 0.7f) * (DriftRadius * 0.5f);
        float floatY = Mathf.Sin(_timePassed * FloatSpeed) * FloatAmplitude;

        Position = new Vector2(
            _anchorPosition.X + driftX,
            _anchorPosition.Y + driftY + floatY
        );
    }

    protected override void Shoot()
    {
        if (!IsInstanceValid(currentTarget) || BulletScene == null) return;

        // --- REPRODUCIR ANIMACIÓN ---
        // Al soltar la bomba, activamos la animación "default"
        MyAnimation?.Play("default");

        var bombNode = BulletScene.Instantiate();
        GetTree().CurrentScene.AddChild(bombNode);

        if (bombNode is GloboBomba bomb)
        {
            bomb.GlobalPosition = muzzle?.GlobalPosition ?? GlobalPosition;
            bomb.Launch(currentTarget.GlobalPosition, Damage);
        }
    }
    
    public override void Build()
    {
        int amountGold = 300, amountWood = 125, amountStone = 75, amountIron = 50;

        if (Recursos.Instance.Gold >= amountGold && Recursos.Instance.Wood >= amountWood && 
            Recursos.Instance.Stone >= amountStone && Recursos.Instance.Iron >= amountIron)
        {
            Recursos.Instance.Gold -= amountGold;
            Recursos.Instance.Wood -= amountWood;
            Recursos.Instance.Stone -= amountStone;
            Recursos.Instance.Iron -= amountIron;

            CanBuild = true;
        }
        else
        {
            CanBuild = false;
        }
    }
}
