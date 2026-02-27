using Godot;

public partial class menu_pausa : CanvasLayer
{
    private Button btnGuardar;
    private Button btnGuardarSalir;
    private Button btnQuitarPausa;
    private Button btnVolver;

    private bool isPaused = false;

    public override void _Ready()
    {
        // El menú debe procesar aunque el juego esté pausado
        ProcessMode = ProcessModeEnum.Always;

        // Referencias (usa rutas relativas reales de tu escena)
        btnGuardar = GetNodeOrNull<Button>("CenterContainer/VBoxContainer/guardar");
        btnGuardarSalir = GetNodeOrNull<Button>("CenterContainer/VBoxContainer/guardarsalir");
        btnQuitarPausa = GetNodeOrNull<Button>("CenterContainer/VBoxContainer/quitar_pausa");
        btnVolver = GetNodeOrNull<Button>("CenterContainer/VBoxContainer/volver");

        ConfigurarBoton(btnGuardar);
        ConfigurarBoton(btnGuardarSalir);
        ConfigurarBoton(btnQuitarPausa);
        ConfigurarBoton(btnVolver);

        if (btnGuardar != null)
            btnGuardar.Pressed += OnGuardar;

        if (btnGuardarSalir != null)
            btnGuardarSalir.Pressed += OnGuardarSalir;

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

        // 🔥 MUY IMPORTANTE → empezar oculto
        Visible = false;
    }

    private void ConfigurarBoton(Button b)
    {
        if (b == null) return;

        b.ProcessMode = ProcessModeEnum.Always;
        b.MouseFilter = Control.MouseFilterEnum.Stop;
    }

    // Usamos _Input porque es más fiable para pausa
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

    private void QuitarPausa()
    {
		Input.MouseMode = Input.MouseModeEnum.Visible;
        isPaused = false;
        GetTree().Paused = false;
        Visible = false;

        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    private void OnGuardar()
    {
        GD.Print("Guardar partida");
    }

    private void OnGuardarSalir()
    {
        GD.Print("Guardar y salir");

        QuitarPausa();
		Input.MouseMode = Input.MouseModeEnum.Visible;
        GetTree().ChangeSceneToFile("res://scenes/ui/Menu.tscn");
    }
}