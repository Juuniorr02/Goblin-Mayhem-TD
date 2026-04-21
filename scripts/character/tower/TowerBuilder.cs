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

    public override void _Ready()
    {
        // Tu lógica de detección de mapas
        string[] mapNames = { "tutorial, montana1", "islas1", "pantano1" };
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
            if (nodo is TextureButton btn) btn.Pressed += () => SelectTower(btn.Name);
        }

        // Creamos el fantasma (Martillo) inicialmente oculto
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
            // --- EL TRUCO: Multiplicamos el radio por la escala del nodo ---
            // Esto detectará si el rango está estirado o es muy grande
            rangeX = circle.Radius * collision.Scale.X * area.Scale.X;
            rangeY = circle.Radius * collision.Scale.Y * area.Scale.Y;
        }

        // Le pasamos AMBOS radios al martillo para que dibuje el óvalo perfecto
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

        // Color verde si puede construir, rojo si no
        ghostInstance.Modulate = CanBuildOnTile(tilePos) ? new Color(0, 1, 0, 0.6f) : new Color(1, 0, 0, 0.6f);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mbe && mbe.Pressed)
        {
            if (mbe.ButtonIndex == MouseButton.Left && !string.IsNullOrEmpty(currentTowerName))
            {
                AttemptBuild();
            }
            else if (mbe.ButtonIndex == MouseButton.Right)
            {
                CancelSelection();
            }
        }
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
            
            CancelSelection();
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

            // Lógica de exclusividad de tiles
            string prop = currentTowerName switch { 
                "Ship" => "can_build_boat", 
                "AtunHatchery" => "can_build_atun", 
                _ => "can_build" 
            };

            Variant buildData = data.GetCustomData(prop);
            if (buildData.VariantType != Variant.Type.Nil && buildData.AsBool()) return true;
            return false;
        }
        return false;
    }
}
