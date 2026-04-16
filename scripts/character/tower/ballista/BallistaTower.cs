using Godot;

public partial class BallistaTower : BaseTower
{
    protected override void Shoot()
    {
        if (!IsInstanceValid(currentTarget) || BulletScene == null) return;

        var shotNode = BulletScene.Instantiate();
        GetTree().CurrentScene.AddChild(shotNode);

        if (shotNode is BallistaBolt bolt)
        {
            bolt.GlobalPosition = muzzle?.GlobalPosition ?? GlobalPosition;
            Vector2 dir = (currentTarget.GlobalPosition - GlobalPosition).Normalized();
            bolt.Launch(dir, Damage);
        }
    }
}