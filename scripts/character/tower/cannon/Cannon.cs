using Godot;

public partial class Cannon : BaseTower
{
    [Export] public AnimatedSprite2D MyAnimation;

    public override void _Ready()
    {
        base._Ready();
        // Si no se asigna en el inspector, busca el nodo automáticamente
        if (MyAnimation == null)
            MyAnimation = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
    }

    protected override void Shoot()
    {
        if (!IsInstanceValid(currentTarget) || BulletScene == null || muzzle == null)
            return;

        // --- REPRODUCIR ANIMACIÓN ---
        MyAnimation?.Play("default");

        var bulletNode = BulletScene.Instantiate();
        GetTree().CurrentScene.AddChild(bulletNode);
        
        if (bulletNode is Node2D bullet2D)
        {
            bullet2D.GlobalPosition = muzzle.GlobalPosition;
            
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
    
    public override void Build()
    {
        int amountGold = 150, amountWood = 25, amountStone = 0, amountIron = 0;

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
