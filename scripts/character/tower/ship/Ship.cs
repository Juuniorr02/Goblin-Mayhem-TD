using Godot;

public partial class Ship : BaseTower
{
    public override void _Ready()
    {
        base._Ready();
        // Configuración inicial por defecto para barcos
        CanTargetAir = false;
        CanTargetLand = true;
        CanTargetWater = true;
    }

    protected override void Shoot()
    {
        if (!IsInstanceValid(currentTarget) || BulletScene == null || muzzle == null)
            return;

        var bulletNode = BulletScene.Instantiate();
        GetTree().CurrentScene.AddChild(bulletNode);
        
        if (bulletNode is Node2D bullet2D)
        {
            bullet2D.GlobalPosition = muzzle.GlobalPosition;
            
            if (bulletNode is Bullet bulletScript)
            {
                // Cálculo de dirección hacia el objetivo
                Vector2 direction = (currentTarget.GlobalPosition - muzzle.GlobalPosition).Normalized();
                bulletScript.Direction = direction;
                bulletScript.Damage = Damage;
                bulletScript.Rotation = direction.Angle();
            }
            
            GD.Print("[BOAT] ¡Cañonazo desde el agua!");
        }
    }
}