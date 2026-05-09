using Godot;

public partial class MorteroTower : BaseTower
{
    [Export] public AnimatedSprite2D MyAnimation;

    public override void _Ready()
    {
        base._Ready(); // IMPORTANTE: Llama al padre para configurar señales
        
        if (MyAnimation == null)
            MyAnimation = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");

        CanTargetLand = true;
        CanTargetAir = false; // El mortero normalmente no apunta a aire
    }

    protected override void Shoot()
    {
        // Si no hay objetivo o escena, abortamos
        if (!IsInstanceValid(currentTarget) || BulletScene == null) return;

        // --- REPRODUCIR ANIMACIÓN ---
        MyAnimation?.Play("default");

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

    public override void Build()
    {
        int amountGold = 200, amountWood = 100, amountStone = 25, amountIron = 0;

        if (Recursos.Instance.Gold >= amountGold && Recursos.Instance.Wood >= amountWood && 
            Recursos.Instance.Stone >= amountStone && Recursos.Instance.Iron >= amountIron)
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
