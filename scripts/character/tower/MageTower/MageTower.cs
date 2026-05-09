using Godot;

public partial class MageTower : BaseTower
{
    [Export] public AnimatedSprite2D MyAnimation;

    public override void _Ready()
    {
        base._Ready();
        
        // Si no se asigna en el inspector, busca el nodo automáticamente
        if (MyAnimation == null)
            MyAnimation = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");

        CanTargetLand = true;
        CanTargetAir = true;
        CanTargetWater = true;
    }

    protected override void Shoot()
    {
        if (!IsInstanceValid(currentTarget) || BulletScene == null) return;

        // --- REPRODUCIR ANIMACIÓN ---
        MyAnimation?.Play("default");

        var shotNode = BulletScene.Instantiate();
        GetTree().CurrentScene.AddChild(shotNode);

        if (shotNode is MageShot mageShot)
        {
            mageShot.GlobalPosition = muzzle?.GlobalPosition ?? GlobalPosition;
            // Pasamos la posición final del objetivo
            mageShot.Launch(currentTarget.GlobalPosition, Damage);
        }
    }

    public override void Build()
    {
        int amountGold = 250, amountWood = 150, amountStone = 100, amountIron = 50;

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
