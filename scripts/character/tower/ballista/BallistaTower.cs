using Godot;

public partial class BallistaTower : BaseTower
{
    [Export] public AnimatedSprite2D MyAnimation;

    public override void _Ready()
    {
        base._Ready();
        // Intenta buscar el nodo si no se asignó en el inspector
        if (MyAnimation == null)
            MyAnimation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    protected override void Shoot()
    {
        if (!IsInstanceValid(currentTarget) || BulletScene == null) return;

        // --- REPRODUCIR ANIMACIÓN ---
        MyAnimation?.Play("default");

        var shotNode = BulletScene.Instantiate();
        GetTree().CurrentScene.AddChild(shotNode);

        if (shotNode is BallistaBolt bolt)
        {
            bolt.GlobalPosition = muzzle?.GlobalPosition ?? GlobalPosition;
            Vector2 dir = (currentTarget.GlobalPosition - GlobalPosition).Normalized();
            bolt.Launch(dir, Damage);
        }
    }

    public override void Build()
    {
        int amountGold = 150, amountWood = 75, amountStone = 0, amountIron = 0;

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
