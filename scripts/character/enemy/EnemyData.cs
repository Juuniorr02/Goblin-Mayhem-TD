using Godot;

[GlobalClass]
public partial class EnemyData : Resource
{
    [Export] public string EnemyName;
    [Export] public float Speed = 120;
    [Export] public int DamageToBase = 1;
    [Export] public bool IsFlying = false;
    [Export] public bool IsAquatic = false;

    [Export] public PackedScene EnemyScene;
}