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
}
