using Godot;
using System;

public partial class GridIdEditor : CanvasLayer
{
    [Export] public NodePath gridManagerPath = new NodePath("../Map/GridManager");
    private GridManager gridManager;

    private SpinBox pathSpin;
    private SpinBox spawnSpin;
    private SpinBox goalSpin;

    public override void _Ready()
    {
        gridManager = GetNodeOrNull<GridManager>(gridManagerPath);
        if (gridManager == null)
        {
            GD.PrintErr($"GridIdEditor: could not find GridManager at '{gridManagerPath}'");
            return;
        }

        pathSpin = GetNode<SpinBox>("UI/PathHBox/PathSpin");
        spawnSpin = GetNode<SpinBox>("UI/SpawnHBox/SpawnSpin");
        goalSpin = GetNode<SpinBox>("UI/GoalHBox/GoalSpin");

        pathSpin.Value = gridManager.pathItemId;
        spawnSpin.Value = gridManager.spawnItemId;
        goalSpin.Value = gridManager.goalItemId;

        var apply = GetNode<Button>("UI/ApplyBtn");
        apply.Connect("pressed", new Callable(this, nameof(OnApplyPressed)));
    }

    private void OnApplyPressed()
    {
        if (gridManager == null) return;
        gridManager.pathItemId = (int)pathSpin.Value;
        gridManager.spawnItemId = (int)spawnSpin.Value;
        gridManager.goalItemId = (int)goalSpin.Value;
        GD.Print($"GridIdEditor: applying ids path={gridManager.pathItemId} spawn={gridManager.spawnItemId} goal={gridManager.goalItemId}");
        gridManager.RefreshFromGridMap();
    }
}
