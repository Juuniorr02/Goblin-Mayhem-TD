using Godot;
using System.Threading.Tasks;

public partial class SaveSystem : Node
{
    private const string SavePath = "user://save.json";

    public void SaveGame()
    {
        var data = new Godot.Collections.Dictionary();

        data["scene"] = GetTree().CurrentScene.SceneFilePath;

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

        var towersData = new Godot.Collections.Array();

        var towersNode = GetTree().CurrentScene.GetNodeOrNull<Node>("Towers");

        if (towersNode != null)
        {
            foreach (Node child in towersNode.GetChildren())
            {
                if (child is Node3D tower)
                {
                    var towerInfo = new Godot.Collections.Dictionary
                    {
                        { "scene", tower.SceneFilePath },
                        { "x", tower.GlobalPosition.X },
                        { "y", tower.GlobalPosition.Y },
                        { "z", tower.GlobalPosition.Z }
                    };

                    towersData.Add(towerInfo);
                }
            }
        }

        data["towers"] = towersData;

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

        if (data.ContainsKey("towers"))
        {
            var towersNode = GetTree().CurrentScene.GetNodeOrNull<Node>("Towers");
            var towers = data["towers"].AsGodotArray();

            foreach (Godot.Collections.Dictionary tower in towers)
            {
                string scenePath = tower["scene"].AsString();

                var packed = GD.Load<PackedScene>(scenePath);
                var instance = packed.Instantiate<Node3D>();

                instance.GlobalPosition = new Vector3(
                    (float)tower["x"],
                    (float)tower["y"],
                    (float)tower["z"]
                );

                towersNode.AddChild(instance);
            }
        }

        GD.Print("✅ partida cargada");
    }
}