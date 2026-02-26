using Godot;
using System.Collections.Generic;

public partial class AStarManager : Node
{
    [Export] private GridManager gridManager;
    private Godot.NavigationRegion3D astar = new();

    public List<Vector3> GetPath(Vector3 start, Vector3 goal)
    {
        // Por simplicidad, un A* manual sobre GridData
        // Retorna lista de posiciones 3D
        List<Vector3> path = new();
        // TODO: implementar A* aquí o usar NavigationServer
        return path;
    }
}