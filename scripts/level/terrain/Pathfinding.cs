using Godot;
using System.Collections.Generic;

public partial class Pathfinding : Node
{
    public TileMapLayer tileMap;

    public const float TILE_WIDTH = 232f;   // ancho isométrico
    public const float TILE_HEIGHT = 110f;  // alto isométrico

    public override void _Ready()
    {
        if (tileMap == null)
            tileMap = GetTree().Root.FindChild("Suelo", true, false) as TileMapLayer;

        if (tileMap == null)
            GD.PrintErr("Pathfinding: no se encontró la capa de suelo.");
    }

    // Convierte posición de tile a posición global
    public Vector2 TileToGlobalPos(Vector2I tilePos)
    {
        if (tileMap == null) return Vector2.Zero;

        float x = (tilePos.X - tilePos.Y) * TILE_WIDTH / 2f;
        float y = (tilePos.X + tilePos.Y) * TILE_HEIGHT / 2f;

        return tileMap.GlobalPosition + new Vector2(x, y);
    }

    // Construye un Path2D a partir de un arreglo de tiles
    public Path2D BuildPath2D(Vector2I[] pathTiles)
    {
        if (pathTiles.Length == 0)
        {
            GD.PrintErr("Pathfinding: no se pudo generar path.");
            return null;
        }

        Curve2D curve = new Curve2D();
        foreach (Vector2I tile in pathTiles)
            curve.AddPoint(TileToGlobalPos(tile));

        curve.BakeInterval = 8; // suaviza el path

        return new Path2D { Curve = curve };
    }
}