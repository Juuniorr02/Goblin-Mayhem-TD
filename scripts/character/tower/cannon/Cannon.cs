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
