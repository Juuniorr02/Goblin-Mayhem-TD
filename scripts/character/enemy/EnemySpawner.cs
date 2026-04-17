using Godot;
using System;

public partial class EnemySpawner : Node
{
    [ExportGroup("Configuración de Caminos")]
    [Export] public Path2D CaminoA;
    [Export] public Path2D CaminoB;
    [Export] public Path2D CaminoAcuatico;
    [Export] public Path2D CaminoVolador;

    [ExportGroup("Configuración de Oleadas")]
    [Export] public WaveData[] Waves;

    private int _currentWaveIndex = 0;
    private int _currentGroupIndex = 0;
    private int _enemiesSpawnedInGroup = 0;
    private float _spawnTimer = 0f;
    private bool _isSpawning = false;

    public override void _Process(double delta)
    {
        if (!_isSpawning || _currentWaveIndex >= Waves.Length) return;

        WaveData currentWave = Waves[_currentWaveIndex];
        
        // Si terminamos todos los grupos de la oleada actual
        if (_currentGroupIndex >= currentWave.Groups.Length)
        {
            _isSpawning = false;
            GD.Print($"Spawn finalizado para: {currentWave.WaveName}");
            return;
        }

        _spawnTimer += (float)delta;

        if (_spawnTimer >= currentWave.SpawnInterval)
        {
            EnemyGroup currentGroup = currentWave.Groups[_currentGroupIndex];
            
            SpawnEnemy(currentGroup);
            _spawnTimer = 0f;
            _enemiesSpawnedInGroup++;

            // Al terminar el grupo, pasamos al siguiente dentro de la misma oleada
            if (_enemiesSpawnedInGroup >= currentGroup.Count)
            {
                _currentGroupIndex++;
                _enemiesSpawnedInGroup = 0;
            }
        }
    }

    public void StartWave(int index)
    {
        if (index < 0 || index >= Waves.Length)
        {
            GD.PrintErr($"No hay configuración para la oleada {index}");
            return;
        }

        _currentWaveIndex = index;
        _currentGroupIndex = 0;
        _enemiesSpawnedInGroup = 0;
        _spawnTimer = 0f;
        _isSpawning = true;
        
        GD.Print($"Iniciando spawn de la oleada: {Waves[index].WaveName}");
    }

    private void SpawnEnemy(EnemyGroup group)
    {
        EnemyData data = group.EnemyType;
        if (data?.EnemyScene == null) return;

        Path2D selectedPath = GetPathFromSelection(group.Path, data);
        if (selectedPath == null) return;

        PathFollow2D follow = new PathFollow2D { Loop = false, Progress = 0, Rotates = false };
        selectedPath.AddChild(follow);

        Enemy enemy = data.EnemyScene.Instantiate<Enemy>();
        enemy.Data = data;
        follow.AddChild(enemy);
    }

    private Path2D GetPathFromSelection(TargetPath selection, EnemyData data)
    {
        if (selection == TargetPath.Automatico)
        {
            if (data.IsFlying) return CaminoVolador;
            if (data.IsAquatic) return CaminoAcuatico;
            return GD.Randf() < 0.5f ? CaminoA : CaminoB;
        }

        return selection switch
        {
            TargetPath.CaminoA => CaminoA,
            TargetPath.CaminoB => CaminoB,
            TargetPath.Acuatico => CaminoAcuatico,
            TargetPath.Volador => CaminoVolador,
            _ => CaminoA
        };
    }
}
