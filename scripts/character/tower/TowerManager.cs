using Godot;
using System;

public partial class TowerManager : Node
{
    [Export] private GridManager gridManager;
    [Export] private PackedScene towerScene;

    [Signal] public delegate void TowerPlacedEventHandler(Vector3 position);

    public bool PlaceTower(int x, int z)
    {
        if (!gridManager.IsBuildable(x, z)) return false;

        Vector3 worldPos = new Vector3(x, 0, z); // ajustar Y si quieres altura
        var tower = towerScene.Instantiate<Node3D>();
        tower.Position = worldPos;
        AddChild(tower);

        gridManager.Grid[x, z].HasTower = true;

    EmitSignal("TowerPlaced", worldPos);
        return true;
    }
}