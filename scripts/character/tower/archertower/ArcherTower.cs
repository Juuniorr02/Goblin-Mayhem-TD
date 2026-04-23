using Godot;

public partial class ArcherTower : BaseTower
{
    [Export] public float GravityCompensation = 0.18f;

    public override void _Ready()
    {
        base._Ready();
        // Según tu tabla: Terrestres, Aéreos y Acuáticos
        CanTargetLand = true;
        CanTargetAir = true;
        CanTargetWater = true;
    }

protected override void Shoot()
{
    if (!IsInstanceValid(currentTarget) || BulletScene == null || muzzle == null)
        return;

    var arrowNode = BulletScene.Instantiate();
    if (arrowNode is Arrow arrow)
    {
        GetTree().CurrentScene.AddChild(arrow);
        arrow.GlobalPosition = muzzle.GlobalPosition;

        Vector2 targetPos = currentTarget.GlobalPosition;
        
        // --- LÓGICA DE PREDICCIÓN ---
        if (currentTarget is CharacterBody2D enemyBody)
        {
            float distance = muzzle.GlobalPosition.DistanceTo(targetPos);
            // Estimamos cuánto tarda la flecha en llegar (distancia / velocidad)
            float timeToReach = distance / arrow.Speed;
            
            // Sumamos a la posición actual: velocidad_enemigo * tiempo_vuelo
            targetPos += enemyBody.Velocity * timeToReach;
        }

        // --- CÁLCULO DE DIRECCIÓN CON PARÁBOLA ---
        Vector2 toTarget = targetPos - muzzle.GlobalPosition;
        float dist = toTarget.Length();
        Vector2 offset = new Vector2(0, -dist * GravityCompensation);
        Vector2 direction = (toTarget + offset).Normalized();

        arrow.Launch(direction, Damage);
    }
    }

    public override void Build()
    {
        int amountGold = 0, amountWood = 0, amountStone = 0, amountIron = 0;
        
        amountGold = 100; amountWood = 50; amountStone = 0; amountIron = 0;

        if (Recursos.Instance.Gold >= amountGold && Recursos.Instance.Wood >= amountWood && Recursos.Instance.Stone >= amountStone && Recursos.Instance.Iron >= amountIron)
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
