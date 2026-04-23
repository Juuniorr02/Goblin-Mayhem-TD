using Godot;

public partial class MageTower : BaseTower
{
    public override void _Ready()
    {
        base._Ready();
        CanTargetLand = true;
        CanTargetAir = true;
        CanTargetWater = true;
    }

    protected override void Shoot()
    {
        if (!IsInstanceValid(currentTarget) || BulletScene == null) return;

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
        int amountGold = 0, amountWood = 0, amountStone = 0, amountIron = 0;
        
        amountGold = 100; amountWood = 50; amountStone = 0; amountIron = 0;

        if (Recursos.Instance.Gold >= amountGold && Recursos.Instance.Wood >= amountWood && Recursos.Instance.Stone >= amountStone && Recursos.Instance.Iron >= amountIron)
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
