using Godot;

public partial class MenuDerrotaTutorial : CanvasLayer
{
    private Button btnSiguiente;
	private int healthActual;
    private bool isPaused = false;


    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        btnSiguiente = GetNodeOrNull<Button>("PanelContainer/VBoxContainer/botones/siguiente");

        ConfigurarBoton(btnSiguiente);

        btnSiguiente.Pressed += OnSiguiente;

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

    private void OnSiguiente()
	{
		QuitarPausa();
        Recursos.Instance.FirstLevelEnd();
        Base.Instance.RepairBase();
        Wave.Instance.ResetWaves();
    	GetTree().ChangeSceneToFile("res://scenes/level/aldea/mapa_mundi.tscn");
	}
}