using Godot;

public partial class ArcherTower : BaseTower
{
    [Export] public float GravityCompensation = 0.18f;
    // Referencia al nodo de animación (ajústalo si usas AnimationPlayer)
    [Export] public AnimatedSprite2D MyAnimation; 

    public override void _Ready()
    {
        base._Ready();
        CanTargetLand = true;
        CanTargetAir = true;
        CanTargetWater = true;

        // Si no lo asignas en el inspector, intenta buscarlo automáticamente
        if (MyAnimation == null)
            MyAnimation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    protected override void Shoot()
    {
        if (!IsInstanceValid(currentTarget) || BulletScene == null || muzzle == null)
            return;

        // --- REPRODUCIR ANIMACIÓN ---
        if (MyAnimation != null)
        {
            MyAnimation.Play("default");
        }

        var arrowNode = BulletScene.Instantiate();
        if (arrowNode is Arrow arrow)
        {
            GetTree().CurrentScene.AddChild(arrow);
            arrow.GlobalPosition = muzzle.GlobalPosition;

            Vector2 targetPos = currentTarget.GlobalPosition;
            
            if (currentTarget is CharacterBody2D enemyBody)
            {
                float distance = muzzle.GlobalPosition.DistanceTo(targetPos);
                float timeToReach = distance / arrow.Speed;
                targetPos += enemyBody.Velocity * timeToReach;
            }

            Vector2 toTarget = targetPos - muzzle.GlobalPosition;
            float dist = toTarget.Length();
            Vector2 offset = new Vector2(0, -dist * GravityCompensation);
            Vector2 direction = (toTarget + offset).Normalized();

            arrow.Launch(direction, Damage);
        }
    }

    public override void Build()
    {
        int amountGold = 100, amountWood = 50, amountStone = 0, amountIron = 0;

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
