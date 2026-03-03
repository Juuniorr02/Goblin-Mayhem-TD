using Godot;

public partial class menu_pausa : CanvasLayer
{
    private Button btnGuardar;
    private Button btnGuardarSalir;
    private Button btnVolver;
    private Button btnCargar;

    private bool isPaused = false;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        btnGuardar = GetNodeOrNull<Button>("CenterContainer/VBoxContainer/guardar");
        btnGuardarSalir = GetNodeOrNull<Button>("CenterContainer/VBoxContainer/guardarsalir");
        btnVolver = GetNodeOrNull<Button>("CenterContainer/VBoxContainer/volver");
        btnCargar = GetNodeOrNull<Button>("CenterContainer/VBoxContainer/cargar");

        ConfigurarBoton(btnGuardar);
        ConfigurarBoton(btnGuardarSalir);
        ConfigurarBoton(btnVolver);
        ConfigurarBoton(btnCargar);

        if (btnGuardar != null)
            btnGuardar.Pressed += OnGuardar;

        if (btnGuardarSalir != null)
            btnGuardarSalir.Pressed += OnGuardarSalir;

        if (btnCargar != null)
            btnCargar.Pressed += OnCargar;

        if (btnVolver != null)
            btnVolver.Pressed += Salir;

        Visible = false;
    }

    private void ConfigurarBoton(Button b)
    {
        if (b == null) return;

        b.ProcessMode = ProcessModeEnum.Always;
        b.MouseFilter = Control.MouseFilterEnum.Stop;
    }

    public override void _Input(InputEvent e)
    {
        if (e.IsActionPressed("pausa"))
        {
            if (isPaused) QuitarPausa();
            else Pausar();
        }
    }

    private void Pausar()
    {
        isPaused = true;
        GetTree().Paused = true;
        Visible = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

	private void Salir()
    {
        QuitarPausa();
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GetTree().ChangeSceneToFile("res://scenes/ui/Menu.tscn");
    }

    private void QuitarPausa()
    {
        isPaused = false;
        GetTree().Paused = false;
        Visible = false;
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    private void OnGuardar()
	{
    	GD.Print("Guardar partida");

    	var save = GetNode<SaveSystem>("/root/SaveSystem");
    	save.SaveGame();
	}

	private void OnGuardarSalir()
	{
    	GD.Print("Guardar y salir");

		var save = GetNode<SaveSystem>("/root/SaveSystem");
    	save.SaveGame();

    	QuitarPausa();
    	Input.MouseMode = Input.MouseModeEnum.Visible;
    	GetTree().ChangeSceneToFile("res://scenes/ui/Menu.tscn");
	}

	private async void OnCargar()
	{
    	GD.Print("Cargar partida");

    	var save = GetNode<SaveSystem>("/root/SaveSystem");
    	await save.LoadGame();
	}
}