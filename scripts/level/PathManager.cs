using Godot;
using System.Collections.Generic;

public partial class PathManager : Node
{
    public List<Vector3> PathPoints = new();

    public override void _Ready()
    {
        // debug information to trace the error location and types
        var parent = GetParent();
        GD.Print("PathManager._Ready called");
        GD.Print($"Parent node: {parent} (type={parent?.GetType()}, name={parent?.Name})");

        // attempt to fetch the child node without generic cast first
        var raw = parent?.GetNode("GridManager");
        GD.Print($"GetNode returned: {raw} (type={raw?.GetType()})");

        // safe cast with check instead of direct generic call
        var grid = raw as GridManager;
        if (grid == null)
        {
            GD.PrintErr("Failed to cast the node to GridManager. Check scene tree path or node type.");
        }

        for (int x = 0; x < GridManager.WIDTH; x++)
        {
            PathPoints.Add(new Vector3(x * GridManager.TILE_SIZE, 0, 10 * GridManager.TILE_SIZE));
        }
    }
}