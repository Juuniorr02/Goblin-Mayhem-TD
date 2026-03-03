using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyManager : Node
{
    [Export] private PackedScene enemyScene;
    private List<Node3D> enemies = new();

    public void SpawnEnemy(Vector3 startPos, List<Vector3> path)
    {
        var inst = enemyScene.Instantiate();
        if (inst is Node3D enemy)
        {
            GD.Print($"EnemyManager: spawning enemy at ({startPos.X},{startPos.Z}) with path length={(path==null?0:path.Count)}");
            enemy.Position = startPos;
            AddChild(enemy);

            // If the root node has an EnemyGoblin script attached, call its SetWaypoints
            if (enemy is EnemyGoblin eg)
            {
                eg.SetWaypoints(path);
                GD.Print($"EnemyManager: SetWaypoints called on instance, waypoints={(eg==null?0:eg)}");
            }

            enemies.Add(enemy);
        }
    }
}