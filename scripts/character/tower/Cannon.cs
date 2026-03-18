using Godot;

public partial class Cannon : BaseTower
{
    protected override void Shoot()
    {
        // Verificamos que el objetivo siga vivo y tengamos una escena de bala
        if (!IsInstanceValid(currentTarget) || BulletScene == null) return;
        
        // Instanciamos la bala usando la clase 'Bullet' que creamos arriba
        var bullet = BulletScene.Instantiate<Bullet>();
        
        // La añadimos al árbol de nodos (fuera de la torre para que sea independiente)
        GetTree().Root.AddChild(bullet);
        
        // La posicionamos en la punta del cañón
        bullet.GlobalPosition = muzzle.GlobalPosition;
        
        // Le damos la dirección hacia el enemigo
        bullet.Direction = (currentTarget.GlobalPosition - muzzle.GlobalPosition).Normalized();
        
        // Opcional: Hacer que la bala mire hacia donde vuela
        bullet.Rotation = bullet.Direction.Angle();
    }
}
