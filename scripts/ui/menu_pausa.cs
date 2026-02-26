using Godot;

public partial class menu_pausa : CanvasLayer
{
    private ColorRect overlay;
    private TextureRect textureRect;

    private Button btnGuardar;
    private Button btnGuardarSalir;
    private Button btnOpciones;
    private Button btnQuitarPausa;
    private Button btnVolver;

	private CanvasLayer menuOpciones;
	private PackedScene menuOpcionesScene = GD.Load<PackedScene>("res://scenes/ui/menu_pausa_opciones.tscn");

    private bool isPaused = false;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        overlay = GetNodeOrNull<ColorRect>("Overlay");
        textureRect = GetNodeOrNull<TextureRect>("TextureRect");

        btnGuardar = GetNodeOrNull<Button>("guardar");
        btnGuardarSalir = GetNodeOrNull<Button>("guardarsalir");
        btnOpciones = GetNodeOrNull<Button>("opciones");
        btnQuitarPausa = GetNodeOrNull<Button>("quitar_pausa");
        btnVolver = GetNodeOrNull<Button>("volver");

        ConfigurarBoton(btnGuardar);
        ConfigurarBoton(btnGuardarSalir);
        ConfigurarBoton(btnOpciones);
        ConfigurarBoton(btnQuitarPausa);
        ConfigurarBoton(btnVolver);

        if (btnGuardar != null)
            btnGuardar.Pressed += OnGuardar;

        if (btnGuardarSalir != null)
            btnGuardarSalir.Pressed += OnGuardarSalir;

        if (btnOpciones != null)
            btnOpciones.Pressed += OnOpciones;

        if (btnQuitarPausa != null)
            btnQuitarPausa.Pressed += QuitarPausa;

        if (btnVolver != null)
        {
            btnVolver.Pressed += () =>
            {
                QuitarPausa();
                GetTree().ChangeSceneToFile("res://scenes/ui/Menu.tscn");
            };
        }

        OcultarMenu();
    }

    private void ConfigurarBoton(Button b)
    {
        if (b == null) return;

        b.ProcessMode = ProcessModeEnum.Always;
        b.MouseFilter = Control.MouseFilterEnum.Stop;
    }

    public override void _UnhandledInput(InputEvent e)
    {
        if (Input.IsActionJustPressed("pausa"))
        {
            if (isPaused) QuitarPausa();
            else Pausar();

            GetViewport().SetInputAsHandled();
        }
    }

    private void Pausar()
    {
        isPaused = true;
        GetTree().Paused = true;
        MostrarMenu();
    }

    private void QuitarPausa()
    {
        isPaused = false;
        GetTree().Paused = false;
        OcultarMenu();
    }

    private void MostrarMenu()
    {
        if (overlay != null) overlay.Visible = true;
        if (textureRect != null) textureRect.Visible = true;

        if (btnGuardar != null) btnGuardar.Visible = true;
        if (btnGuardarSalir != null) btnGuardarSalir.Visible = true;
        if (btnOpciones != null) btnOpciones.Visible = true;
        if (btnQuitarPausa != null) btnQuitarPausa.Visible = true;
        if (btnVolver != null) btnVolver.Visible = true;
    }

    private void OcultarMenu()
    {
        if (overlay != null) overlay.Visible = false;
        if (textureRect != null) textureRect.Visible = false;

        if (btnGuardar != null) btnGuardar.Visible = false;
        if (btnGuardarSalir != null) btnGuardarSalir.Visible = false;
        if (btnOpciones != null) btnOpciones.Visible = false;
        if (btnQuitarPausa != null) btnQuitarPausa.Visible = false;
        if (btnVolver != null) btnVolver.Visible = false;
    }

    private void OnGuardar()
    {
        GD.Print("Guardar partida");
        // aquí tu sistema de guardado
    }

    private void OnGuardarSalir()
    {
        GD.Print("Guardar y salir");
        // guardar
        QuitarPausa();
        GetTree().ChangeSceneToFile("res://scenes/ui/Menu.tscn");
    }

    private void OnOpciones()
    {
        if (menuOpciones == null)
    	{
        	menuOpciones = menuOpcionesScene.Instantiate<CanvasLayer>();
        	GetTree().Root.AddChild(menuOpciones);
    	}

    	OcultarMenu();
    	menuOpciones.Visible = true;
    }
}