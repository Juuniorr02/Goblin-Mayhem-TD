using Godot;

[GlobalClass]
public partial class EnemyGroup : Resource
{
    [Export] public EnemyData EnemyType;
    [Export] public int Count = 5;
    [Export] public TargetPath Path = TargetPath.Automatico;
    public EnemyGroup() { }
}
