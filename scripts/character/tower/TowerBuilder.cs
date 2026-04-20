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
    private string lastLayerName = "";
    private Node2D ghostInstance;
    private Dictionary<Vector2I, Node2D> occupiedTiles = new();

    public override void _Ready()
    {
        string[] mapNames = { "mapa2", "islas1", "pantano" };
        foreach (string name in mapNames)
        {
            Node mapRoot = GetTree().Root.FindChild(name, true, false);
            if (mapRoot != null)
            {
                foreach (Node child in mapRoot.GetChildren())
                {
                    if (child is TileMapLayer layer)
                    {
                        allLayers.Add(layer);
                        GD.Print($"Capa detectada en {name}: {layer.Name}");
                    }
                }
            }
        }

        var botones = GetTree().GetNodesInGroup("botones_torres");
        foreach (Node nodo in botones)
        {
            if (nodo is TextureButton btn)
            {
                string towerId = btn.Name;
                btn.Pressed += () => SelectTower(towerId);
            }
        }

        if (GhostTowerScene != null)
        {
            ghostInstance = GhostTowerScene.Instantiate<Node2D>();
            AddChild(ghostInstance);
            
            // --- CAMBIO CLAVE PARA EL GHOST ---
            // Le decimos al ghost que no debe disparar
            if (ghostInstance.HasMethod("set_IsGhost")) // Si usas [Export] automático
                ghostInstance.Set("IsGhost", true);
            else if (ghostInstance.GetScript().As<CSharpScript>() != null)
                ghostInstance.Set("IsGhost", true);

            ghostInstance.Visible = false;
        }
    }

    public override void _Process(double delta)
    {
        if (ghostInstance == null || !ghostInstance.Visible || allLayers.Count == 0) return;

        Vector2I tilePos = GetTileUnderMouse(allLayers[0]);
        Vector2 localPos = allLayers[0].MapToLocal(tilePos);
        ghostInstance.GlobalPosition = allLayers[0].ToGlobal(localPos) + GhostOffset;

        ghostInstance.Modulate = CanBuildOnTile(tilePos) ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mbe && mbe.Pressed)
        {
            if (mbe.ButtonIndex == MouseButton.Left && !string.IsNullOrEmpty(currentTowerName))
                AttemptBuild();
            else if (mbe.ButtonIndex == MouseButton.Right)
                CancelSelection();
        }
        else if (@event is InputEventKey ek && ek.Pressed && ek.Keycode == Key.Escape)
        {
            CancelSelection();
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
            
            // --- ASEGURAR QUE LA TORRE REAL SÍ DISPARE ---
            towerInstance.Set("IsGhost", false);

            Vector2 localPos = allLayers[0].MapToLocal(tilePos);
            towerInstance.GlobalPosition = allLayers[0].ToGlobal(localPos) + GhostOffset;

            TowersParent.AddChild(towerInstance);
            occupiedTiles[tilePos] = towerInstance;

            if (towerInstance.HasMethod("SetEnemiesContainer"))
                towerInstance.Call("SetEnemiesContainer", EnemiesContainer);
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
    // 1. Si ya hay algo construido, fuera.
    if (occupiedTiles.ContainsKey(tilePos)) return false;

    // 2. Recorremos las capas (de la superior a la inferior)
    for (int i = allLayers.Count - 1; i >= 0; i--)
    {
        TileMapLayer layer = allLayers[i];
        TileData data = layer.GetCellTileData(tilePos);
        
        // Si en esta capa no hay tile, pasamos a la siguiente capa más abajo
        if (data == null) continue;

        // 3. Decidimos qué "permiso" necesitamos
        string propertyRequired = (currentTowerName == "Ship") ? "can_build_boat" : "can_build";

        // 4. Verificamos si el tile tiene esa propiedad específica
        // Importante: GetCustomData devuelve un valor nulo si la propiedad no existe en el TileSet
        Variant buildData = data.GetCustomData(propertyRequired);

        if (buildData.VariantType != Variant.Type.Nil && buildData.AsBool())
        {
            // ¡Éxito! El tile tiene la propiedad correcta y es 'true'
            return true;
        }
        else
        {
            // Si encontramos un tile pero no tiene la propiedad que buscamos, 
            // dejamos de buscar en capas inferiores para evitar construir en "el vacío"
            // o en capas que no corresponden.
            return false; 
        }
    }
    return false; 
}


    public void SelectTower(string towerName)
    {
        currentTowerName = towerName;
        if (ghostInstance != null)
            ghostInstance.Visible = !string.IsNullOrEmpty(towerName);
    }
}
