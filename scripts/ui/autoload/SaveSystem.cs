using Godot;
using System.Threading.Tasks;

public partial class SaveSystem : Node
{
    public static SaveSystem Instance;
    private const string SavePath = "user://save.json";

    public void SaveGame()
    {
        var data = new Godot.Collections.Dictionary();

        data["scene"] = GetTree().CurrentScene.SceneFilePath;

        // Cámara
        var cam = GetTree().GetFirstNodeInGroup("camera") as Camera3D;
        if (cam != null)
        {
            data["camera_pos"] = new Godot.Collections.Dictionary
            {
                { "x", cam.GlobalPosition.X },
                { "y", cam.GlobalPosition.Y },
                { "z", cam.GlobalPosition.Z }
            };

            data["camera_zoom"] = cam.Size;
        }

        // Progreso (booleanos)
        data["level1"] = GameData.Level1;
        data["level2"] = GameData.Level2;
        data["level3"] = GameData.Level3;
        data["level4"] = GameData.Level4;
        data["level5"] = GameData.Level5;
        data["aldeaVisitada"] = GameData.AldeaVisitada;
        data["mapaMundiVisitado"] = GameData.MapaMundiVisitado;

        var json = Json.Stringify(data);

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        file.StoreString(json);

        Recursos.Instance.SaveData();

        GD.Print("✅ partida guardada");
    }

    public async Task LoadGame()
    {
        Recursos.Instance.StartLevel();
        Recursos.Instance.LoadData();
        Wave.Instance.ResetWaves();
        if (!FileAccess.FileExists(SavePath))
        {
            GD.Print("❌ no hay save");
            return;
        }

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
        var json = file.GetAsText();

        var parser = new Json();
        if (parser.Parse(json) != Error.Ok)
        {
            GD.PrintErr("❌ save corrupto");
            return;
        }

        var data = parser.Data.AsGodotDictionary();

        // Cambiar escena
        string scene = data["scene"].AsString();

        if (GetTree().ChangeSceneToFile(scene) != Error.Ok)
        {
            GD.PrintErr("❌ error cambiando escena");
            return;
        }

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        // Restaurar cámara
        var cam = GetTree().GetFirstNodeInGroup("camera") as Camera3D;

        if (cam != null && data.ContainsKey("camera_pos"))
        {
            var pos = data["camera_pos"].AsGodotDictionary();

            cam.GlobalPosition = new Vector3(
                (float)pos["x"],
                (float)pos["y"],
                (float)pos["z"]
            );
        }

        if (cam != null && data.ContainsKey("camera_zoom"))
        {
            cam.Size = (float)data["camera_zoom"];
        }

        // 🔥 Restaurar progreso (SOBREESCRIBE valores)
        if (data.ContainsKey("level1"))
            GameData.Level1 = (bool)data["level1"];

        if (data.ContainsKey("level2"))
            GameData.Level2 = (bool)data["level2"];

        if (data.ContainsKey("level3"))
            GameData.Level3 = (bool)data["level3"];

        if (data.ContainsKey("level4"))
            GameData.Level4 = (bool)data["level4"];

        if (data.ContainsKey("level5"))
            GameData.Level5 = (bool)data["level5"];

        if(data.ContainsKey("aldeaVisitada"))
            GameData.AldeaVisitada = (bool)data["aldeaVisitada"];
            
        if(data.ContainsKey("mapaMundiVisitado"))
            GameData.MapaMundiVisitado = (bool)data["mapaMundiVisitado"];

        GD.Print("✅ partida cargada");
    }
        public override void _Ready()
    {
        Instance = this;
    }
}