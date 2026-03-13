using Godot;
using System.Collections.Generic;

public partial class TowerBuilder : Node2D
{
    public TileMapLayer BuildableTilemap; 

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
        // 1. Buscar el Mapa
        if (BuildableTilemap == null)
            BuildableTilemap = GetTree().Root.FindChild("Suelo", true, false) as TileMapLayer;

        // 2. Buscar el Botón por grupo
        var botones = GetTree().GetNodesInGroup("botones_torres");
        foreach (Node nodo in botones)
        {
            if (nodo is TextureButton btn && btn.Name == "Cannon")
            {
                btn.Pressed += OnCannonPressed;
                GD.Print("Botón Cannon conectado por grupo.");
            }
        }

        // 3. Ghost
        if (GhostTowerScene != null)
        {
            ghostInstance = GhostTowerScene.Instantiate<Node2D>();
            AddChild(ghostInstance);
            ghostInstance.Visible = false;
        }
    }

    public override void _Process(double delta)
    {
        if (ghostInstance == null || !ghostInstance.Visible || BuildableTilemap == null) return;

        Vector2I tilePos = GetTileUnderMouse();
        Vector2 localPos = BuildableTilemap.MapToLocal(tilePos);
        ghostInstance.GlobalPosition = BuildableTilemap.ToGlobal(localPos) + GhostOffset;

        ghostInstance.Modulate = CanBuildOnTile(tilePos) ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mbe && mbe.Pressed && mbe.ButtonIndex == MouseButton.Left)
        {
            if (!string.IsNullOrEmpty(currentTowerName))
            {
                GD.Print("--- Intento de construcción detectado ---");
                AttemptBuild();
            }
        }
    }

    private void AttemptBuild()
    {
        if (BuildableTilemap == null) return;

        Vector2I tilePos = GetTileUnderMouse();
        GD.Print($"1. Posición clicada: {tilePos}");

        if (!CanBuildOnTile(tilePos))
        {
            GD.Print("2. RECHAZADO: No se cumplen las condiciones de construcción.");
            return;
        }

        if (TowersScenes.TryGetValue(currentTowerName, out PackedScene scene))
        {
            Node2D towerInstance = scene.Instantiate<Node2D>();
            Vector2 localPos = BuildableTilemap.MapToLocal(tilePos);
            towerInstance.GlobalPosition = BuildableTilemap.ToGlobal(localPos) + GhostOffset;

            TowersParent.AddChild(towerInstance);
            occupiedTiles[tilePos] = towerInstance;

            if (towerInstance.HasMethod("SetEnemiesContainer"))
                towerInstance.Call("SetEnemiesContainer", EnemiesContainer);
            
            GD.Print($"3. ¡ÉXITO! Torre {currentTowerName} construida.");
        }
        else
        {
            GD.Print($"2. ERROR: '{currentTowerName}' no está en el diccionario del Inspector.");
        }
    }

    private Vector2I GetTileUnderMouse()
    {
        Vector2 mousePos = GetGlobalMousePosition();
        Vector2 localPos = BuildableTilemap.ToLocal(mousePos);
        return BuildableTilemap.LocalToMap(localPos);
    }

    private bool CanBuildOnTile(Vector2I tilePos)
    {
        if (BuildableTilemap == null) return false;

        TileData data = BuildableTilemap.GetCellTileData(tilePos);
        if (data == null) 
        {
            GD.Print("   - Fallo: Celda sin TileData (vacía)");
            return false;
        }

        bool canBuild = false;

        // Comprobación de Metadata (Tiled)
        if (data.HasMeta("can_build"))
        {
            canBuild = data.GetMeta("can_build").AsBool();
            GD.Print($"   - Metadata 'can_build' encontrada: {canBuild}");
        }
        else 
        {
            GD.Print("   - Fallo: El tile no tiene Metadata 'can_build'.");
            foreach (var meta in data.GetMetaList()) GD.Print($"     * Meta disponible: {meta}");
        }

        if (occupiedTiles.ContainsKey(tilePos)) GD.Print("   - Fallo: Tile ya ocupado.");

        return canBuild && !occupiedTiles.ContainsKey(tilePos);
    }

    public void OnCannonPressed()
    {
        GD.Print("Acción: Botón Cannon pulsado.");
        SelectTower("Cannon"); 
    }

    public void SelectTower(string towerName)
    {
        currentTowerName = towerName;
        if (ghostInstance != null)
            ghostInstance.Visible = !string.IsNullOrEmpty(towerName);
    }
}
