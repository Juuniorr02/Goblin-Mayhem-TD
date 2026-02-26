using Godot;

public partial class GameSettings : Node
{
    private const string ConfigPath = "user://config.cfg";

    public int Width { get; private set; } = 1920;
    public int Height { get; private set; } = 1080;
    public string Mode { get; private set; } = "Ventana";
    public float Volume { get; private set; } = 1f;

    public void LoadConfig()
    {
        var config = new ConfigFile();
        if (config.Load(ConfigPath) != Error.Ok)
            return;

        Width = (int)config.GetValue("display", "width", 1920);
        Height = (int)config.GetValue("display", "height", 1080);
        Mode = (string)config.GetValue("display", "mode", "Ventana");
        Volume = (float)config.GetValue("audio", "volume", 1f);

        GD.Print($"Configuración cargada: {Width}x{Height} {Mode} Vol={Volume}");
    }

    public void SaveConfig()
    {
        var config = new ConfigFile();
        config.SetValue("display", "width", Width);
        config.SetValue("display", "height", Height);
        config.SetValue("display", "mode", Mode);
        config.SetValue("audio", "volume", Volume);
        config.Save(ConfigPath);
    }

    public void ApplySettings()
    {
        DisplayServer.WindowSetSize(new Vector2I(Width, Height));
        DisplayServer.WindowSetMode(Mode switch
        {
            "Pantalla completa" => DisplayServer.WindowMode.Fullscreen,
            "Sin bordes" => DisplayServer.WindowMode.ExclusiveFullscreen,
            _ => DisplayServer.WindowMode.Windowed
        });

        int masterBus = AudioServer.GetBusIndex("Master");
        AudioServer.SetBusVolumeDb(masterBus, Linear2Db(Volume));
    }

    private float Linear2Db(float linear)
    {
        if (linear <= 0) return -80;
        return 20f * Mathf.Log(linear) / Mathf.Log(10f);
    }
}