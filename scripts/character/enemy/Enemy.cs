using Godot;
using System.Collections.Generic;

public partial class Enemy : Node3D
{
    public List<Vector3> PathPoints;
    public float Speed = 2f;
    private int targetIndex = 0;

    public override void _Ready()
    {
        if (PathPoints != null && PathPoints.Count > 0)
            GlobalPosition = PathPoints[0];
    }

    public override void _Process(double delta)
    {
        if (PathPoints == null || targetIndex >= PathPoints.Count)
            return;

        Vector3 target = PathPoints[targetIndex];
        GlobalPosition = GlobalPosition.MoveToward(target, Speed * (float)delta);

        if (GlobalPosition.DistanceTo(target) < 0.05f)
            targetIndex++;
    }
}