using Godot;

public partial class MenuDerrota : CanvasLayer
{
    private Button btnCargar;
    private Button btnVolver;
    private Button btnReiniciar;
	private int healthActual;
    private bool isPaused = false;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        btnCargar = GetNodeOrNull<Button>("PanelContainer/VBoxContainer/botones/cargar");
        btnVolver = GetNodeOrNull<Button>("PanelContainer/VBoxContainer/botones/volver");
        btnReiniciar = GetNodeOrNull<Button>("PanelContainer/VBoxContainer/botones/reiniciar");

        ConfigurarBoton(btnCargar);
        ConfigurarBoton(btnVolver);
        ConfigurarBoton(btnReiniciar);

        if (btnCargar != null)
            btnCargar.Pressed += OnCargar;

        if (btnReiniciar != null)
            btnReiniciar.Pressed += OnReiniciar;

        if (btnVolver != null)
            btnVolver.Pressed += OnGuardarSalir;

        Visible = false;
    }

	public override void _Process(double delta)
    {
        UpdateMenuDerrota();
    }

    private void ConfigurarBoton(Button b)
    {
        if (b == null) return;

        b.ProcessMode = ProcessModeEnum.Always;
        b.MouseFilter = Control.MouseFilterEnum.Stop;
    }

    public void UpdateMenuDerrota()
	{
		healthActual = Base.Instance.Health;

		if (Base.Instance.Health <= 0)
		{
			Pausar();
		}
	}

    private void Pausar()
    {
        isPaused = true;
        GetTree().Paused = true;
        Visible = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    private void QuitarPausa()
    {
        isPaused = false;
        GetTree().Paused = false;
        Visible = false;
    }

    private void OnReiniciar()
	{
		QuitarPausa();
        Base.Instance.RepairBase();
        Wave.Instance.ResetWaves();
    	GD.Print("Reiniciar partida");
    	GetTree().ReloadCurrentScene();
	}

	private void OnGuardarSalir()
	{
		QuitarPausa();
    	GD.Print("Guardar y salir");

		var save = GetNode<SaveSystem>("/root/SaveSystem");
    	save.SaveGame();

    	QuitarPausa();
    	Input.MouseMode = Input.MouseModeEnum.Visible;
    	GetTree().ChangeSceneToFile("res://scenes/ui/menus/menu.tscn");
	}

	private async void OnCargar()
	{
		QuitarPausa();
    	GD.Print("Cargar partida");

    	var save = GetNode<SaveSystem>("/root/SaveSystem");
    	await save.LoadGame();
	}
}