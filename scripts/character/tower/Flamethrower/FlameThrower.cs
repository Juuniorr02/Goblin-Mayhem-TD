using Godot;

public partial class FlameThrower : BaseTower
{
    [Export] public float Spread = 0.2f; 
    [Export] public AnimatedSprite2D MyAnimation;

    public override void _Ready()
    {
        base._Ready();
        if (MyAnimation == null)
            MyAnimation = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");

        if (shootTimer != null) shootTimer.WaitTime = 0.1f; 
        
        CanTargetLand = true;
        CanTargetAir = false;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        // Lógica de control de animaciones
        if (IsInstanceValid(currentTarget))
        {
            // Si hay un objetivo y no se está reproduciendo ya la animación de disparo
            if (MyAnimation?.Animation != "default")
            {
                MyAnimation?.Play("default");
            }
        }
        else
        {
            // Si NO hay objetivo y todavía estamos en la animación de disparo (o en reposo)
            // Solo disparamos la animación de salida una vez
            if (MyAnimation?.Animation == "default")
            {
                MyAnimation?.Play("dejarDeDisparar");
            }
        }
    }

    protected override void Shoot()
    {
        if (!IsInstanceValid(currentTarget) || BulletScene == null) return;

        // Ya no llamamos a Play("default") aquí, lo hace el _Process 
        // para que no se reinicie el frame con cada bolita de fuego.

        var shotNode = BulletScene.Instantiate();
        GetTree().CurrentScene.AddChild(shotNode);

        if (shotNode is Flame flame)
        {
            flame.GlobalPosition = muzzle?.GlobalPosition ?? GlobalPosition;
            Vector2 dir = (currentTarget.GlobalPosition - GlobalPosition).Normalized();
            float variance = (float)GD.RandRange(-Spread, Spread);
            flame.Launch(dir.Rotated(variance), Damage);
        }
    }

    public override void Build()
    {
        // ... (Tu código de Build se mantiene igual)
        int amountGold = 200, amountWood = 100, amountStone = 50, amountIron = 25;
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
