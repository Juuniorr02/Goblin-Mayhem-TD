using Godot;

public partial class Cannon : BaseTower
{
    protected override void Shoot()
    {
        if (!IsInstanceValid(currentTarget) || BulletScene == null || muzzle == null)
            return;

        var bulletNode = BulletScene.Instantiate();
        GetTree().CurrentScene.AddChild(bulletNode);
        
        // Usamos GlobalPosition directamente en el nodo
        if (bulletNode is Node2D bullet2D)
        {
            bullet2D.GlobalPosition = muzzle.GlobalPosition;
            
            // Intentamos configurar las propiedades si el script existe
            if (bulletNode is Bullet bulletScript)
            {
                Vector2 direction = (currentTarget.GlobalPosition - muzzle.GlobalPosition).Normalized();
                bulletScript.Direction = direction;
                bulletScript.Damage = Damage;
                bulletScript.Rotation = direction.Angle();
            }
            
            GD.Print("[CANNON] ¡Fuego!");
        }
    }
}
