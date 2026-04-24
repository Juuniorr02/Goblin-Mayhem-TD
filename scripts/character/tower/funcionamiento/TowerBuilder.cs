using Godot;
using System.Collections.Generic;

public partial class TowerBuilder : Node2D
{
    private List<TileMapLayer> allLayers = new();
    [Export] public Node2D TowersParent;              
    [Export] public PackedScene GhostTowerScene;       
    [Export] public Node2D EnemiesContainer;          
    [Export] public Godot.Collections.Dictionary<string, PackedScene> TowersScenes = new();
    [Export] public Vector2 GhostOffset = new Vector2(0, 0); 

    private string currentTowerName = "";
    private bool isDeconstructing = false; 
    private Node2D ghostInstance;
    private Dictionary<Vector2I, Node2D> occupiedTiles = new();

    public override void _Ready()
    {
        RefreshMapLayers();
        SetupButtons();

        if (GhostTowerScene != null)
        {
            ghostInstance = GhostTowerScene.Instantiate<Node2D>();
            AddChild(ghostInstance);
            ghostInstance.Visible = false;
        }
    }

    private void RefreshMapLayers()
    {
        allLayers.Clear();
        var nodes = GetTree().CurrentScene.FindChildren("*", "TileMapLayer", true, false);
        foreach (var node in nodes)
        {
            if (node is TileMapLayer layer) allLayers.Add(layer);
        }
    }

    private void SetupButtons()
    {
        // Buscamos todos los botones en la escena que estén en el grupo "botones_torres"
        var botones = GetTree().GetNodesInGroup("botones_torres");
        
        foreach (Node nodo in botones)
        {
            if (nodo is BaseButton btn) // BaseButton cubre Button y TextureButton
            {
                // Limpiamos conexiones previas para evitar duplicados
                if (btn.IsConnected(BaseButton.SignalName.Pressed, Callable.From(() => SelectTower(btn.Name))))
                    continue;

                btn.Pressed += () => {
                    GD.Print("Botón presionado: " + btn.Name); // Para depuración
                    SelectTower(btn.Name);
                };
            }
        }
    }

    public void SelectTower(string towerName)
    {
        currentTowerName = towerName;
        // Importante: Esto debe coincidir exactamente con el nombre del nodo en el Inspector
        isDeconstructing = (towerName == "Borrar");

        if (ghostInstance == null) return;

        if (string.IsNullOrEmpty(towerName))
        {
            ghostInstance.Visible = false;
            return;
        }

        ghostInstance.Visible = true;
        
        if (isDeconstructing)
        {
            GD.Print("Modo desmantelar activado");
            if (ghostInstance.HasMethod("UpdateRangeVisual"))
                ghostInstance.Call("UpdateRangeVisual", 0, 0);
            return;
        }

        if (TowersScenes.TryGetValue(towerName, out PackedScene towerScene))
        {
            var dummyTower = towerScene.Instantiate<Node2D>();
            float rangeX = 0; float rangeY = 0;
            var area = dummyTower.GetNodeOrNull<Area2D>("DetectionRange") ?? dummyTower.GetNodeOrNull<Area2D>("detectionRange");
            var collision = area?.GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
            if (collision != null && collision.Shape is CircleShape2D circle) 
            {
                rangeX = circle.Radius * collision.Scale.X * area.Scale.X;
                rangeY = circle.Radius * collision.Scale.Y * area.Scale.Y;
            }
            if (ghostInstance.HasMethod("UpdateRangeVisual"))
                ghostInstance.Call("UpdateRangeVisual", rangeX, rangeY);
            dummyTower.QueueFree();
        }
    }

    public override void _Process(double delta)
    {
        if (ghostInstance == null || !ghostInstance.Visible || allLayers.Count == 0) return;

        TileMapLayer currentLayer = GetLayerUnderMouse() ?? allLayers[0];
        Vector2I tilePos = GetTileUnderMouse(currentLayer);
        Vector2 localPos = currentLayer.MapToLocal(tilePos);
        
        ghostInstance.GlobalPosition = currentLayer.ToGlobal(localPos) + GhostOffset;

        if (isDeconstructing)
        {
            // Cambia el color del fantasma a rojo si detecta una torre en esa posición
            ghostInstance.Modulate = occupiedTiles.ContainsKey(tilePos) 
                ? new Color(1, 0, 0, 0.8f) 
                : new Color(1, 1, 1, 0.4f);
        }
        else
        {
            ghostInstance.Modulate = CanBuildOnTile(tilePos) && BuildTime.CanBuild 
                ? new Color(0, 1, 0, 0.6f) 
                : new Color(1, 0, 0, 0.6f);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mbe && mbe.Pressed)
        {
            if (mbe.ButtonIndex == MouseButton.Left && !string.IsNullOrEmpty(currentTowerName))
            {
                if (IsMouseOverUI()) return;
                AttemptBuild();
            }
            else if (mbe.ButtonIndex == MouseButton.Right)
            {
                CancelSelection();
            }
        }
    }

    private bool IsMouseOverUI()
    {
        var guiObj = GetViewport().GuiGetHoveredControl();
        return guiObj != null && guiObj.MouseFilter == Control.MouseFilterEnum.Stop;
    }

    private void AttemptBuild()
    {
        TileMapLayer targetLayer = GetLayerUnderMouse();
        if (targetLayer == null) return;
        Vector2I tilePos = GetTileUnderMouse(targetLayer);

        if (isDeconstructing)
        {
            if (occupiedTiles.TryGetValue(tilePos, out Node2D towerToDestroy))
            {
                GD.Print("Borrando torre en " + tilePos);
                towerToDestroy.QueueFree();
                Recursos.Instance.DevolverRecuros();
                occupiedTiles.Remove(tilePos);
            }
            return;
        }

        if (!BuildTime.CanBuild || !CanBuildOnTile(tilePos)) return;

        if (TowersScenes.TryGetValue(currentTowerName, out PackedScene scene))
        {
            Node2D towerInstance = scene.Instantiate<Node2D>();
            if (towerInstance is BaseTower tower)
            {
                tower.Build();
                if (!tower.CanBuild) { tower.QueueFree(); return; }
            }

            Vector2 localPos = targetLayer.MapToLocal(tilePos);
            towerInstance.GlobalPosition = targetLayer.ToGlobal(localPos) + GhostOffset;

            TowersParent.AddChild(towerInstance);
            occupiedTiles[tilePos] = towerInstance;
        }
    }

    private void CancelSelection()
    {
        currentTowerName = "";
        isDeconstructing = false;
        if (ghostInstance != null) ghostInstance.Visible = false;
    }

    private TileMapLayer GetLayerUnderMouse()
    {
        for (int i = allLayers.Count - 1; i >= 0; i--)
        {
            Vector2I tp = GetTileUnderMouse(allLayers[i]);
            if (allLayers[i].GetCellTileData(tp) != null) return allLayers[i];
        }
        return null;
    }

    private Vector2I GetTileUnderMouse(TileMapLayer layer)
    {
        Vector2 mousePos = GetGlobalMousePosition();
        Vector2 localPos = layer.ToLocal(mousePos);
        return layer.LocalToMap(localPos);
    }

    private bool CanBuildOnTile(Vector2I tilePos)
    {
        if (occupiedTiles.ContainsKey(tilePos)) return false;

        foreach (TileMapLayer layer in allLayers)
        {
            TileData data = layer.GetCellTileData(tilePos);
            if (data == null) continue;

            string prop = currentTowerName switch { 
                "Ship" => "can_build_boat", 
                "Atun" => "can_build_atun", 
                _ => "can_build" 
            };

            Variant buildData = data.GetCustomData(prop);
            if (buildData.VariantType != Variant.Type.Nil && buildData.AsBool()) return true;
        }
        return false;
    }
}
