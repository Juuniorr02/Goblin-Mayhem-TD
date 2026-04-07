using Godot;

public partial class Cannon : BaseTower
{
    protected override void Shoot()
    {
        if (!IsInstanceValid(currentTarget) || BulletScene == null || muzzle == null)
            return;

        var bullet = BulletScene.Instantiate<Bullet>();

        // Añadir a la escena actual (correcto)
        GetTree().CurrentScene.AddChild(bullet);

        bullet.GlobalPosition = muzzle.GlobalPosition;

        Vector2 direction = (currentTarget.GlobalPosition - muzzle.GlobalPosition).Normalized();

        bullet.Direction = direction;
        bullet.Rotation = direction.Angle();

        // Pasar daño
        bullet.Damage = Damage;
    }
}