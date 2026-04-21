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
    private Node2D ghostInstance;
    private Dictionary<Vector2I, Node2D> occupiedTiles = new();
    private List<TextureButton> towerButtons = new(); // Para rastrear los botones

    public override void _Ready()
    {
        allLayers.Clear();
        towerButtons.Clear();

        string[] mapNames = { "mapa2", "islas1", "pantano" };
        foreach (string name in mapNames)
        {
            Node mapRoot = GetTree().Root.FindChild(name, true, false);
            if (mapRoot != null)
            {
                foreach (Node child in mapRoot.GetChildren())
                {
                    if (child is TileMapLayer layer) allLayers.Add(layer);
                }
            }
        }
        
        var botones = GetTree().GetNodesInGroup("botones_torres");
        foreach (Node nodo in botones)
        {
            if (nodo is TextureButton btn)
            {
                btn.Pressed += () => SelectTower(btn.Name);
                btn.MouseFilter = Control.MouseFilterEnum.Stop; // La UI bloquea el clic hacia abajo
                towerButtons.Add(btn);
            }
        }

        if (GhostTowerScene != null)
        {
            ghostInstance = GhostTowerScene.Instantiate<Node2D>();
            AddChild(ghostInstance);
            ghostInstance.Visible = false;
        }
    }

    public void SelectTower(string towerName)
    {
        currentTowerName = towerName;
        if (ghostInstance == null) return;

        if (string.IsNullOrEmpty(towerName))
        {
            ghostInstance.Visible = false;
            return;
        }

        ghostInstance.Visible = true;
        
        if (TowersScenes.TryGetValue(towerName, out PackedScene towerScene))
        {
            var dummyTower = towerScene.Instantiate<Node2D>();
            float rangeX = 0;
            float rangeY = 0;

            var area = dummyTower.GetNodeOrNull<Area2D>("DetectionRange") ?? dummyTower.GetNodeOrNull<Area2D>("detectionRange");
            var collision = area?.GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
            
            if (collision != null && collision.Shape is CircleShape2D circle) 
            {
                rangeX = circle.Radius * collision.Scale.X * area.Scale.X;
                rangeY = circle.Radius * collision.Scale.Y * area.Scale.Y;
            }

            if (ghostInstance.HasMethod("UpdateRangeVisual"))
            {
                ghostInstance.Call("UpdateRangeVisual", rangeX, rangeY);
            }
            
            dummyTower.QueueFree();
        }
    }

    public override void _Process(double delta)
    {
        if (ghostInstance == null || !ghostInstance.Visible || allLayers.Count == 0) return;

        Vector2I tilePos = GetTileUnderMouse(allLayers[0]);
        Vector2 localPos = allLayers[0].MapToLocal(tilePos);
        ghostInstance.GlobalPosition = allLayers[0].ToGlobal(localPos) + GhostOffset;

        ghostInstance.Modulate = CanBuildOnTile(tilePos) ? new Color(0, 1, 0, 0.6f) : new Color(1, 0, 0, 0.6f);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mbe && mbe.Pressed)
        {
            if (mbe.ButtonIndex == MouseButton.Left && !string.IsNullOrEmpty(currentTowerName))
            {
                // PROTECCIÓN: Si el ratón está sobre un botón de la UI, NO intentes construir
                if (IsMouseOverUI()) return;

                AttemptBuild();
            }
            else if (mbe.ButtonIndex == MouseButton.Right)
            {
                CancelSelection();
            }
        }
    }

    // Comprueba si el ratón está encima de algún botón para evitar construir debajo
private bool IsMouseOverUI()
{
    // 1. Preguntamos si hay algún nodo de la interfaz bajo el ratón ahora mismo
    // Control.MouseButton es el filtro estándar para esto.
    var guiObj = GetViewport().GuiGetHoveredControl();
    
    // 2. Si el ratón está sobre algo de la interfaz (guiObj != null)
    // y ese algo tiene el MouseFilter en 'Stop', bloqueamos la construcción.
    if (guiObj != null && guiObj.MouseFilter == Control.MouseFilterEnum.Stop)
    {
        return true;
    }
    
    return false;
}

    private void AttemptBuild()
    {
        if (allLayers.Count == 0) return;
        Vector2I tilePos = GetTileUnderMouse(allLayers[0]);

        if (!CanBuildOnTile(tilePos)) return;

        if (TowersScenes.TryGetValue(currentTowerName, out PackedScene scene))
        {
            Node2D towerInstance = scene.Instantiate<Node2D>();
            Vector2 localPos = allLayers[0].MapToLocal(tilePos);
            towerInstance.GlobalPosition = allLayers[0].ToGlobal(localPos) + GhostOffset;

            TowersParent.AddChild(towerInstance);
            occupiedTiles[tilePos] = towerInstance;
        }
    }

    private void CancelSelection()
    {
        currentTowerName = "";
        if (ghostInstance != null) ghostInstance.Visible = false;
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
        for (int i = allLayers.Count - 1; i >= 0; i--)
        {
            TileMapLayer layer = allLayers[i];
            TileData data = layer.GetCellTileData(tilePos);
            if (data == null) continue;

            string prop = currentTowerName switch { 
                "Ship" => "can_build_boat", 
                "AtunHatchery" => "can_build_atun", 
                _ => "can_build" 
            };

            Variant buildData = data.GetCustomData(prop);
            if (buildData.VariantType != Variant.Type.Nil && buildData.AsBool()) return true;
        }
        return false;
    }
}
