using Godot;
using System.Threading.Tasks;

public partial class SaveSystem : Node
{
    private const string SavePath = "user://save.json";

    public void SaveGame()
    {
        var data = new Godot.Collections.Dictionary();

        data["scene"] = GetTree().CurrentScene.SceneFilePath;

        // 📷 guardar cámara
        var cam = GetTree().GetFirstNodeInGroup("camera") as Node3D;
        if (cam != null)
        {
            data["camera_pos"] = new Godot.Collections.Dictionary
            {
                { "x", cam.GlobalPosition.X },
                { "y", cam.GlobalPosition.Y },
                { "z", cam.GlobalPosition.Z }
            };
        }

        var json = Json.Stringify(data);
        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        file.StoreString(json);

        GD.Print("✅ partida guardada");
    }

    public async Task LoadGame()
    {
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

        string scene = data["scene"].AsString();

        if (GetTree().ChangeSceneToFile(scene) != Error.Ok)
        {
            GD.PrintErr("❌ error cambiando escena");
            return;
        }

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        // 📷 restaurar cámara
        var cam = GetTree().GetFirstNodeInGroup("camera") as Node3D;
        if (cam != null && data.ContainsKey("camera_pos"))
        {
            var pos = data["camera_pos"].AsGodotDictionary();

            cam.GlobalPosition = new Vector3(
                (float)pos["x"],
                (float)pos["y"],
                (float)pos["z"]
            );
        }

        GD.Print("✅ partida cargada");
    }
}