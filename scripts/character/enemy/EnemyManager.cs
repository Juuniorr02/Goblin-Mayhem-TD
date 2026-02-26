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
            enemy.Position = startPos;
            AddChild(enemy);

            // If the root node has an EnemyGoblin script attached, call its SetWaypoints
            if (enemy is EnemyGoblin eg)
            {
                eg.SetWaypoints(path);
            }

            enemies.Add(enemy);
        }
    }
}