using Godot;

public partial class menu_pausa_level1 : menu_pausa
{
    private Button btnVolver;
    private Button btnOpciones;
    private Button btnSalir;

    private float lastPressTime = -1f;
    private float doublePressThreshold = 0.3f;

    private bool isPaused = false;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        btnVolver = GetNodeOrNull<Button>("CenterContainer/VBoxContainer/volver");
        btnOpciones = GetNodeOrNull<Button>("CenterContainer/VBoxContainer/opciones");
        btnSalir = GetNodeOrNull<Button>("CenterContainer/VBoxContainer/salir");

        ConfigurarBoton(btnVolver);
        ConfigurarBoton(btnOpciones);
        ConfigurarBoton(btnSalir);
        
        if (btnOpciones != null)
            btnOpciones.Pressed += OnOpciones;
            
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

    public override void _UnhandledInput(InputEvent @event)
    {
        if (GameData.MenuAbierto)
            return;

        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (keyEvent.Keycode == Key.Escape)
            {
                if (isPaused)
                    QuitarPausa();
                else
                    Pausar();
            }
            
        }
    }

    public new void Pausar()
    {
        isPaused = true;
        GetTree().Paused = true;
        Visible = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

	private void Salir()
    {
        Recursos.Instance.StartLevel();
        Wave.Instance.ResetWaves();
        QuitarPausa();
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GetTree().ChangeSceneToFile("res://scenes/ui/menus/Menu.tscn");
    }

    public new void QuitarPausa()
    {
        isPaused = false;
        GetTree().Paused = false;
        Visible = false;
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