using Godot;

// Esta enumeración debe estar fuera de las clases o en su propio archivo
public enum TargetPath { Automatico, CaminoA, CaminoB, Acuatico, Volador }



[GlobalClass]
public partial class WaveData : Resource
{
    [Export] public string WaveName = "Nueva Oleada";
    [Export] public EnemyGroup[] Groups;
    [Export] public float SpawnInterval = 1.0f;

    public WaveData() { }
}
