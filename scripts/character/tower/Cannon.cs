using Godot;

public partial class Cannon : BaseTower
{
    protected override void Shoot()
    {
        if (!IsInstanceValid(currentTarget) || BulletScene == null || muzzle == null)
            return;

        // Cambiamos el casteo a Node para evitar el error de InvalidCastException
        var bulletNode = BulletScene.Instantiate();
        
        if (bulletNode is Bullet bullet)
        {
            GetTree().CurrentScene.AddChild(bullet);
            bullet.GlobalPosition = muzzle.GlobalPosition;

            Vector2 direction = (currentTarget.GlobalPosition - muzzle.GlobalPosition).Normalized();
            bullet.Direction = direction;
            
            // Si quieres que la bala no rote visualmente, comenta la siguiente línea:
            // bullet.Rotation = direction.Angle();

            bullet.Damage = Damage;
            GD.Print("[CANNON] ¡Disparo realizado con éxito!");
        }
        else
        {
            GD.PrintErr("[ERROR] El objeto instanciado no tiene el script 'Bullet.cs'");
            bulletNode.QueueFree(); // Limpiamos el nodo fallido
        }
    }
}
