using Godot;

public partial class MorteroTower : BaseTower
{
    public override void _Ready()
    {
        base._Ready(); // IMPORTANTE: Llama al padre para configurar señales
        CanTargetLand = true;
        CanTargetAir = false; // El mortero normalmente no apunta a aire
    }

    protected override void Shoot()
    {
        // Si no hay objetivo o escena, abortamos
        if (!IsInstanceValid(currentTarget) || BulletScene == null) return;

        var shotNode = BulletScene.Instantiate();
        GetTree().CurrentScene.AddChild(shotNode);

        if (shotNode is MorteroShot shot)
        {
            // Posicionamos el proyectil (usa muzzle si existe, sino la torre)
            shot.GlobalPosition = muzzle != null ? muzzle.GlobalPosition : GlobalPosition;
            
            // Pasamos el daño y la posición del suelo del enemigo
            shot.Launch(currentTarget.GlobalPosition, Damage);
            GD.Print("[MORTERO] Disparo lanzado.");
        }
    }
}
