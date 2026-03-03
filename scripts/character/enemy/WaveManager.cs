using Godot;
using System;
using System.Collections.Generic;

public partial class WaveManager : Node
{
    [Export] private EnemyManager enemyManager;
    [Export] private AStarManager aStarManager;
    [Export] private GridManager gridManager;

    private int currentWave = 0;

    public void StartNextWave()
    {
        currentWave++;
        // Por ejemplo, 5 enemigos por wave
        for (int i = 0; i < 5; i++)
        {
            Vector3 spawn = FindSpawnTile();
            Vector3 goal = FindGoalTile();
            GD.Print($"WaveManager: spawn=({spawn.X},{spawn.Z}) goal=({goal.X},{goal.Z})");
            var path = aStarManager.GetPath(spawn, goal);
            GD.Print($"WaveManager: received path length={path?.Count}");
            enemyManager.SpawnEnemy(spawn, path);
        }
    }

    private Vector3 FindSpawnTile()
    {
        for (int x = 0; x < GridManager.WIDTH; x++)
        for (int z = 0; z < GridManager.HEIGHT; z++)
            if (gridManager.Grid[x, z].Type == TileType.Spawn)
                return new Vector3(x, 0, z);
        return Vector3.Zero;
    }

    private Vector3 FindGoalTile()
    {
        for (int x = 0; x < GridManager.WIDTH; x++)
        for (int z = 0; z < GridManager.HEIGHT; z++)
            if (gridManager.Grid[x, z].Type == TileType.Goal)
                return new Vector3(x, 0, z);
        return Vector3.Zero;
    }
}