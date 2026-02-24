using Godot;

public partial class TowerManager : Node
{
    [Export] public PackedScene TowerScene;

    public void PlaceTower(int x, int z)
    {
        var grid = GetParent().GetNode<GridManager>("GridManager");

        if (grid.Grid[x, z].Type != TileType.Buildable)
            return;

        if (grid.Grid[x, z].HasTower)
            return;

        Node3D tower = TowerScene.Instantiate<Node3D>();
        tower.Position = new Vector3(x * GridManager.TILE_SIZE, 0, z * GridManager.TILE_SIZE);
        GetParent().AddChild(tower);

        grid.Grid[x, z].HasTower = true;
    }
}