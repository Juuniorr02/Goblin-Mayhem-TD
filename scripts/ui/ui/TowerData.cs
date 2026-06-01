using Godot;

[GlobalClass]
public partial class TowerData : Resource
{
    [Export] public string Name;
    [Export] public string Cost;
    [Export] public string Attack;
    [Export] public string Damage;
    [Export] public Texture2D Icon;
}