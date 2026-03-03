using Godot;
using System;
using System.Collections.Generic;

public partial class AStarManager : Node
{
    [Export] private GridManager gridManager;
    private Godot.NavigationRegion3D astar = new();

    public List<Vector3> GetPath(Vector3 start, Vector3 goal)
    {
        List<Vector3> path = new List<Vector3>();
        if (gridManager == null) return path;

        int sx = (int)start.X;
        int sz = (int)start.Z;
        int gx = (int)goal.X;
        int gz = (int)goal.Z;

        GD.Print($"AStar: request start=({sx},{sz}) goal=({gx},{gz})");

        if (sx < 0 || sx >= GridManager.WIDTH || sz < 0 || sz >= GridManager.HEIGHT) return path;
        if (gx < 0 || gx >= GridManager.WIDTH || gz < 0 || gz >= GridManager.HEIGHT) return path;

        bool[,] closed = new bool[GridManager.WIDTH, GridManager.HEIGHT];
        float[,] gScore = new float[GridManager.WIDTH, GridManager.HEIGHT];
        (int px, int pz)[,] cameFrom = new (int, int)[GridManager.WIDTH, GridManager.HEIGHT];

        for (int x = 0; x < GridManager.WIDTH; x++)
            for (int z = 0; z < GridManager.HEIGHT; z++)
                gScore[x, z] = float.MaxValue;

        List<(int x,int z,float f)> open = new List<(int,int,float)>();

        gScore[sx, sz] = 0;
        float h0 = Math.Abs(gx - sx) + Math.Abs(gz - sz);
        open.Add((sx, sz, h0));

        int[] dx = new int[] {1,-1,0,0};
        int[] dz = new int[] {0,0,1,-1};

        while (open.Count > 0)
        {
            // pop lowest f
            open.Sort((a,b) => a.f.CompareTo(b.f));
            var node = open[0];
            open.RemoveAt(0);
            int cx = node.x; int cz = node.z;
            if (closed[cx, cz]) continue;
            closed[cx, cz] = true;

                if (cx == gx && cz == gz)
            {
                // reconstruct
                List<Vector3> rev = new List<Vector3>();
                int rx = gx, rz = gz;
                rev.Add(new Vector3(rx * GridManager.TILE_SIZE, 0, rz * GridManager.TILE_SIZE));
                while (!(rx == sx && rz == sz))
                {
                    var prev = cameFrom[rx, rz];
                    rx = prev.px; rz = prev.pz;
                    rev.Add(new Vector3(rx * GridManager.TILE_SIZE, 0, rz * GridManager.TILE_SIZE));
                }
                rev.Reverse();
                    GD.Print($"AStar: path found length={rev.Count}");
                return rev;
            }

            for (int i = 0; i < 4; i++)
            {
                int nx = cx + dx[i];
                int nz = cz + dz[i];
                if (nx < 0 || nx >= GridManager.WIDTH || nz < 0 || nz >= GridManager.HEIGHT) continue;
                if (closed[nx,nz]) continue;

                var tile = gridManager.Grid[nx, nz];
                if (tile.Type != TileType.Path && tile.Type != TileType.Spawn && tile.Type != TileType.Goal) continue;

                float tentative = gScore[cx,cz] + 1;
                if (tentative < gScore[nx,nz])
                {
                    gScore[nx,nz] = tentative;
                    cameFrom[nx,nz] = (cx, cz);
                    float f = tentative + Math.Abs(gx - nx) + Math.Abs(gz - nz);
                    open.Add((nx, nz, f));
                }
            }
        }

        GD.Print("AStar: no path found");
        return path;
    }
}