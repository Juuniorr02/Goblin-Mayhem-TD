using Godot;

public partial class FlameThrower : BaseTower
{
    [Export] public float Spread = 0.2f; 

    public override void _Ready()
    {
        base._Ready();
    if (shootTimer != null) 
    {
        shootTimer.WaitTime = 0.1f; 
    }
        CanTargetLand = true;
        CanTargetAir = false;
    }

   protected override void Shoot()
{
    if (!IsInstanceValid(currentTarget) || BulletScene == null) return;

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
}
