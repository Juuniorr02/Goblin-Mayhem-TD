using Godot;

[GlobalClass]
public partial class TowerData : Resource
{
    [Export] public string Name;
    [Export] public string Cost;
    [Export] public Texture2D Icon;
}