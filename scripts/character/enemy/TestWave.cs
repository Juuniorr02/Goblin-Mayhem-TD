using Godot;

public partial class TestWave : Node
{
    [Export] public EnemySpawner Spawner;
	AStarGrid2D grid = new AStarGrid2D();
    public override void _Ready()
    {
        if (Spawner == null)
        {
            GD.PrintErr("TestWave: Spawner no está asignado en el inspector.");
            return;
        }
    }
}