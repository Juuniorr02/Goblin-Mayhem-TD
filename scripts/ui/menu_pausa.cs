using Godot;

public partial class menu_pausa : CanvasLayer
{
    private Button btnGuardar;
    private Button btnVolver;
    private Button btnCargar;
    private Button btnReiniciar;
    private Button btnOpciones;
    private Button btnSalir;

    private float lastPressTime = -1f;
    private float doublePressThreshold = 0.3f;

    private bool isPaused = false;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        btnGuardar = GetNodeOrNull<Button>("CenterContainer/VBoxContainer/guardar");
        btnVolver = GetNodeOrNull<Button>("CenterContainer/VBoxContainer/volver");
        btnCargar = GetNodeOrNull<Button>("CenterContainer/VBoxContainer/cargar");
        btnReiniciar = GetNodeOrNull<Button>("CenterContainer/VBoxContainer/reiniciar");
        btnOpciones = GetNodeOrNull<Button>("CenterContainer/VBoxContainer/opciones");
        btnSalir = GetNodeOrNull<Button>("CenterContainer/VBoxContainer/salir");

        ConfigurarBoton(btnGuardar);
        ConfigurarBoton(btnVolver);
        ConfigurarBoton(btnCargar);
        ConfigurarBoton(btnReiniciar);
        ConfigurarBoton(btnOpciones);
        ConfigurarBoton(btnSalir);
        
        if (btnOpciones != null)
            btnOpciones.Pressed += OnOpciones;
            
        if (btnReiniciar != null)
            btnReiniciar.Pressed += OnReiniciar;

        if (btnGuardar != null)
            btnGuardar.Pressed += OnGuardar;

        if (btnCargar != null)
            btnCargar.Pressed += OnCargar;

        if (btnSalir != null)
            btnSalir.Pressed += OnSalir;

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
            float currentTime = Time.GetTicksMsec() / 2000.0f;

            if (lastPressTime > 0 && currentTime - lastPressTime <= doublePressThreshold)
            {
                if (isPaused) QuitarPausa();
                else Pausar();

                lastPressTime = -1f;
            }

            else
            {
            lastPressTime = currentTime;
            }
        }
    }

    public void Pausar()
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

    public void QuitarPausa()
    {
        isPaused = false;
        GetTree().Paused = false;
        Visible = false;
    }

    private void OnGuardar()
	{
		QuitarPausa();
    	GD.Print("Guardar partida");

    	var save = GetNode<SaveSystem>("/root/SaveSystem");
    	save.SaveGame();
	}

	private async void OnCargar()
	{
		QuitarPausa();
    	GD.Print("Cargar partida");

    	var save = GetNode<SaveSystem>("/root/SaveSystem");
    	await save.LoadGame();
	}

    private void OnReiniciar()
	{
		QuitarPausa();
        Input.MouseMode = Input.MouseModeEnum.Visible;
        Base.Instance.RepairBase();
        Wave.Instance.ResetWaves();
    	GD.Print("Reiniciar partida");
    	GetTree().ReloadCurrentScene();
	}

    private void OnOpciones()
    {
        var optionsMenu = GetTree().CurrentScene.GetNodeOrNull<OptionsPausa>("OptionsPausa");

        if (optionsMenu != null)
        {
            GD.Print("Abrir menú de opciones");

            optionsMenu.MostrarOpciones(this);
            Visible = false;
        }
    }

    private void OnSalir()
    {
        QuitarPausa();
    }
}