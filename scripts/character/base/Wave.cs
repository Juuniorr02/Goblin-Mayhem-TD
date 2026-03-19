using Godot;

public partial class Wave : Node
{
    public static Wave Instance;

    public int CurrentWave = 0;

    public override void _Ready()
    {
        Instance = this;
    }

    public void StartNextWave()
    {
        CurrentWave++;
    }

    public void ResetWaves()
    {
        CurrentWave = 0;
    }
}