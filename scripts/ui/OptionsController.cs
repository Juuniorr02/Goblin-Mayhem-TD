using Godot;
using System;
using System.IO;

public partial class OptionsController : Control
{
    private OptionButton resolutionOption;
    private OptionButton screenModeOption;
    private HSlider volumeSlider;

    private const string ConfigPath = "user://config.cfg";

    public override void _Ready()
    {
        resolutionOption = GetNode<OptionButton>("CanvasLayer/CenterContainer/VBoxContainer/VBoxResolucion/ResolutionOption");
        screenModeOption = GetNode<OptionButton>("CanvasLayer/CenterContainer/VBoxContainer/VBoxModo/ScreenModeOption");
        volumeSlider = GetNode<HSlider>("CanvasLayer/CenterContainer/VBoxContainer/HBoxVolumen/VolumeSlider");

        // Añadir resoluciones disponibles
        resolutionOption.AddItem("800x600");
        resolutionOption.AddItem("1280x720");
        resolutionOption.AddItem("1920x1080");
        resolutionOption.AddItem("2560x1440");
        resolutionOption.AddItem("3840x2160");

        // Añadir modos de pantalla
        screenModeOption.AddItem("Ventana");
        screenModeOption.AddItem("Pantalla completa");
        screenModeOption.AddItem("Sin bordes");

        // Conectar botones
        GetNode<Button>("CanvasLayer/CenterContainer/VBoxContainer/HBoxBotones/Aplicar").Pressed += OnApply;
        GetNode<Button>("CanvasLayer/CenterContainer/VBoxContainer/HBoxBotones/Volver").Pressed += OnBack;

        // Cargar configuración si existe
        LoadConfig();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (keyEvent.Keycode == Key.Escape)
            {
                GetTree().ChangeSceneToFile("res://scenes/ui/Menu.tscn");
            }
        }
    }

    private void OnApply()
    {
        // Cambiar resolución
        string res = resolutionOption.GetItemText(resolutionOption.Selected);
        string[] parts = res.Split('x');
        int width = int.Parse(parts[0]);
        int height = int.Parse(parts[1]);

        // Cambiar modo de pantalla
        string mode = screenModeOption.GetItemText(screenModeOption.Selected);
        DisplayServer.WindowMode windowMode = DisplayServer.WindowMode.Windowed;

        if (mode == "Pantalla completa")
            windowMode = DisplayServer.WindowMode.Fullscreen;
        else if (mode == "Sin bordes")
            windowMode = DisplayServer.WindowMode.ExclusiveFullscreen;

        DisplayServer.WindowSetMode(windowMode);
        DisplayServer.WindowSetSize(new Vector2I(width, height));

        // Cambiar volumen general
        float volume = (float)volumeSlider.Value;
        int masterBus = AudioServer.GetBusIndex("Master");
        AudioServer.SetBusVolumeDb(masterBus, Linear2Db(volume));

        // Guardar configuración
        SaveConfig(width, height, mode, volume);
    }

    private void OnBack()
    {
        GetTree().ChangeSceneToFile("res://scenes/ui/Menu.tscn");
    }

    // Función auxiliar para convertir de 0..1 a decibelios
    private float Linear2Db(float linear)
    {
        if (linear <= 0) return -80;
        return 20f * (Mathf.Log(linear) / Mathf.Log(10f));
    }

    // Guarda la configuración en un archivo
	private void SaveConfig(int width, int height, string mode, float volume)
	{
    	var config = new ConfigFile();

    	// Usar ruta user://
    	const string path = "user://config.cfg";

    	// Cargar si existe
    	Error err = config.Load(path);
    	if (err != Error.Ok)
        	config = new ConfigFile(); // archivo nuevo

    	// Guardar valores
    	config.SetValue("display", "width", width);
    	config.SetValue("display", "height", height);
    	config.SetValue("display", "mode", mode);
    	config.SetValue("audio", "volume", volume);

    	// Guardar archivo
    	config.Save(path);
	}

    // Carga la configuración desde el archivo si existe
    private void LoadConfig()
    {
        var config = new ConfigFile();
        if (config.Load(ConfigPath) != Error.Ok)
            return; // no existe el archivo, usar valores por defecto

        int width = (int)config.GetValue("display", "width", 1920);
        int height = (int)config.GetValue("display", "height", 1080);
        string mode = (string)config.GetValue("display", "mode", "Ventana");
        float volume = (float)config.GetValue("audio", "volume", 1f);

        // Aplicar configuración
        DisplayServer.WindowSetSize(new Vector2I(width, height));
        DisplayServer.WindowSetMode(mode switch
        {
            "Pantalla completa" => DisplayServer.WindowMode.Fullscreen,
            "Sin bordes" => DisplayServer.WindowMode.ExclusiveFullscreen,
            _ => DisplayServer.WindowMode.Windowed
        });

        int masterBus = AudioServer.GetBusIndex("Master");
        AudioServer.SetBusVolumeDb(masterBus, Linear2Db(volume));

        // Reflejar en UI
        string resText = $"{width}x{height}";
        for (int i = 0; i < resolutionOption.GetItemCount(); i++)
        {
            if (resolutionOption.GetItemText(i) == resText)
            {
                resolutionOption.Selected = i;
                break;
            }
        }

        for (int i = 0; i < screenModeOption.GetItemCount(); i++)
        {
            if (screenModeOption.GetItemText(i) == mode)
            {
                screenModeOption.Selected = i;
                break;
            }
        }

        volumeSlider.Value = volume;
    }
}