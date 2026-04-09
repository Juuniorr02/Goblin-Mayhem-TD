using Godot;
using System.Collections.Generic;

public partial class TowerBuilder : Node2D
{
    // Lista para almacenar todas las capas que encuentre YATI
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
        // 1. Buscar el nodo raíz del mapa generado por YATI
        // AJUSTE: Cambia "Mapa" por el nombre del nodo principal de tu .tmx en la escena
        Node mapRoot = GetTree().Root.FindChild("mapa2", true, false);
        Node mapRoot2 = GetTree().Root.FindChild("islas1", true, false);
        Node mapRoot3 = GetTree().Root.FindChild("pantano", true, false);

        if (mapRoot != null)
        {
            // Buscamos todas las capas (TileMapLayer) que sean hijas del mapa
            foreach (Node child in mapRoot.GetChildren())
            {
                if (child is TileMapLayer layer)
                {
                    allLayers.Add(layer);
                    GD.Print($"Capa detectada: {layer.Name}");
                }
            }
        }
        else
        {
            GD.PrintErr("ERROR: No se encontró el nodo raíz del mapa. Revisa el nombre en FindChild.(mapa2)");
        }

        if (mapRoot2 != null)
        {
            // Buscamos todas las capas (TileMapLayer) que sean hijas del mapa
            foreach (Node child in mapRoot2.GetChildren())
            {
                if (child is TileMapLayer layer)
                {
                    allLayers.Add(layer);
                    GD.Print($"Capa detectada: {layer.Name}");
                }
            }
        }
        else
        {
            GD.PrintErr("ERROR: No se encontró el nodo raíz del mapa. Revisa el nombre en FindChild.(islas1)");
        }
        if (mapRoot3 != null)
        {
            // Buscamos todas las capas (TileMapLayer) que sean hijas del mapa
            foreach (Node child in mapRoot3.GetChildren())
            {
                if (child is TileMapLayer layer)
                {
                    allLayers.Add(layer);
                    GD.Print($"Capa detectada: {layer.Name}");
                }
            }
        }
        else
        {
            GD.PrintErr("ERROR: No se encontró el nodo raíz del mapa. Revisa el nombre en FindChild.(pantano)");
        }

        // 2. Buscar el Botón por grupo
        var botones = GetTree().GetNodesInGroup("botones_torres");
        foreach (Node nodo in botones)
        {
            if (nodo is TextureButton btn)
            {
                // Conectamos CUALQUIER botón del grupo usando su nombre como referencia
                string towerId = btn.Name; // Asume que el botón se llama "Cannon", "Archer", etc.
                btn.Pressed += () => SelectTower(towerId);
                
                GD.Print($"Botón {towerId} configurado automáticamente.");
            }
        }

        // 3. Configurar Ghost
        if (GhostTowerScene != null)
        {
            ghostInstance = GhostTowerScene.Instantiate<Node2D>();
            AddChild(ghostInstance);
            ghostInstance.Visible = false;
        }
    }

    public override void _Process(double delta)
    {
        if (ghostInstance == null || !ghostInstance.Visible || allLayers.Count == 0) return;

        // Usamos la primera capa solo para calcular la posición de la rejilla (suelen ser iguales)
        Vector2I tilePos = GetTileUnderMouse(allLayers[0]);
        Vector2 localPos = allLayers[0].MapToLocal(tilePos);
        ghostInstance.GlobalPosition = allLayers[0].ToGlobal(localPos) + GhostOffset;

        ghostInstance.Modulate = CanBuildOnTile(tilePos) ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mbe && mbe.Pressed && mbe.ButtonIndex == MouseButton.Left)
        {
            if (!string.IsNullOrEmpty(currentTowerName))
            {
                AttemptBuild();
            }
        }
    }

    private void AttemptBuild()
    {
        if (allLayers.Count == 0) return;

        Vector2I tilePos = GetTileUnderMouse(allLayers[0]);

        if (!CanBuildOnTile(tilePos))
        {
            GD.Print("Construcción rechazada: No permitido o casilla ocupada.");
            return;
        }

        if (TowersScenes.TryGetValue(currentTowerName, out PackedScene scene))
        {
            Node2D towerInstance = scene.Instantiate<Node2D>();
            Vector2 localPos = allLayers[0].MapToLocal(tilePos);
            towerInstance.GlobalPosition = allLayers[0].ToGlobal(localPos) + GhostOffset;

            TowersParent.AddChild(towerInstance);
            occupiedTiles[tilePos] = towerInstance;

            if (towerInstance.HasMethod("SetEnemiesContainer"))
                towerInstance.Call("SetEnemiesContainer", EnemiesContainer);
            
            GD.Print($"¡Torre {currentTowerName} construida!");
        }
    }

    private Vector2I GetTileUnderMouse(TileMapLayer layer)
    {
        Vector2 mousePos = GetGlobalMousePosition();
        Vector2 localPos = layer.ToLocal(mousePos);
        return layer.LocalToMap(localPos);
    }

 private bool CanBuildOnTile(Vector2I tilePos)
{
    // Recorremos de arriba (capa 5) hacia abajo (Suelo)
    for (int i = allLayers.Count - 1; i >= 0; i--)
    {
        TileMapLayer layer = allLayers[i];
        TileData data = layer.GetCellTileData(tilePos);

        // Si no hay nada en esta capa (aire), bajamos a la siguiente
        if (data == null) continue;

        // --- LÓGICA DE PRINT (Solo si cambia la capa) ---
        if (layer.Name != lastLayerName)
        {
            lastLayerName = layer.Name;
            GD.Print($"[Mouse] Objeto detectado en Capa: {lastLayerName}");
        }

        // Buscamos la propiedad en el TileSet (sin prefijo data_)
        int layerIndex = layer.TileSet.GetCustomDataLayerByName("can_build");
        
        if (layerIndex != -1)
        {
            Variant buildData = data.GetCustomData("can_build");
            bool canBuild = buildData.VariantType != Variant.Type.Nil && buildData.AsBool();
            
            // SI EL OBJETO MÁS ALTO ES CONSTRUIBLE:
            if (canBuild)
            {
                // Solo permitimos si no hay otra torre ya puesta
                return !occupiedTiles.ContainsKey(tilePos);
            }
        }

        // Si el código llega aquí, significa que ha encontrado un Tile (como la roca) 
        // pero NO tiene permiso para construir. 
        // RETORNAMOS FALSE INMEDIATAMENTE: No dejamos que el código mire las capas de abajo.
        return false; 
    }

    return false; // No hay nada en ninguna capa
}


    public void OnCannonPressed() => SelectTower("Cannon");
    public void OnArcherPressed() => SelectTower("Archer");

    public void SelectTower(string towerName)
    {
        currentTowerName = towerName;
        if (ghostInstance != null)
            ghostInstance.Visible = !string.IsNullOrEmpty(towerName);
    }

    

}
