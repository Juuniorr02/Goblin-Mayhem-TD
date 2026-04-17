using Godot;

public partial class Aldea : Node2D
{
    public Button btnAyuntamiento;
    public Button btnCantera;
    public Button btnAserradero;
    public Button btnHerreria;
    public Button btnMina;
    public Button btnBarrio;
	public Button btnMapa;

    private menu_ayuntamiento menu_ayuntamiento;
    private menu_cantera menu_cantera;
    private menu_aserradero menu_aserradero;
    private menu_herreria menu_herreria;
    private menu_mina menu_mina;
    private menu_barrio menu_barrio;

    private bool menuAbierto = false;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        menu_ayuntamiento = GetNodeOrNull<menu_ayuntamiento>("Botones/BotonAyuntamiento/menu_ayuntamiento");
        menu_cantera = GetNodeOrNull<menu_cantera>("Botones/BotonCantera/menu_cantera");
        menu_aserradero = GetNodeOrNull<menu_aserradero>("Botones/BotonAserradero/menu_aserradero");
        menu_herreria = GetNodeOrNull<menu_herreria>("Botones/BotonHerreria/menu_herreria");
        menu_mina = GetNodeOrNull<menu_mina>("Botones/BotonMina/menu_mina");
        menu_barrio = GetNodeOrNull<menu_barrio>("Botones/BotonBarrio/menu_barrio");

		menu_ayuntamiento.aldea = this;
		menu_cantera.aldea = this;
		menu_aserradero.aldea = this;
		menu_herreria.aldea = this;
		menu_mina.aldea = this;
		menu_barrio.aldea = this;

        btnAyuntamiento = GetNodeOrNull<Button>("Botones/BotonAyuntamiento");
        btnCantera = GetNodeOrNull<Button>("Botones/BotonCantera");
        btnAserradero = GetNodeOrNull<Button>("Botones/BotonAserradero");
        btnHerreria = GetNodeOrNull<Button>("Botones/BotonHerreria");
        btnMina = GetNodeOrNull<Button>("Botones/BotonMina");
        btnBarrio = GetNodeOrNull<Button>("Botones/BotonBarrio");
		btnMapa = GetNodeOrNull<Button>("Botones/BotonMapa");

        ConfigurarBoton(btnAyuntamiento);
        ConfigurarBoton(btnCantera);
        ConfigurarBoton(btnAserradero);
        ConfigurarBoton(btnHerreria);
        ConfigurarBoton(btnMina);
        ConfigurarBoton(btnBarrio);
		ConfigurarBoton(btnMapa);

        if (btnAyuntamiento != null)
            btnAyuntamiento.Pressed += OnAyuntamiento;

        if (btnCantera != null)
            btnCantera.Pressed += OnCantera;

        if (btnAserradero != null)
            btnAserradero.Pressed += OnAserradero;

        if (btnHerreria != null)
            btnHerreria.Pressed += OnHerreria;

        if (btnMina != null)
            btnMina.Pressed += OnMina;

        if (btnBarrio != null)
            btnBarrio.Pressed += OnBarrio;
		
		if (btnMapa != null)
			btnMapa.Pressed += OnMapa;
    }

    private void ConfigurarBoton(Button b)
    {
        if (b == null) return;

        b.ProcessMode = ProcessModeEnum.Pausable;
        b.MouseFilter = Control.MouseFilterEnum.Stop;
		b.FocusMode = Control.FocusModeEnum.None;
		b.ToggleMode = false;

    	b.Pressed += () => b.ButtonPressed = false;
    }

    private void CerrarTodosLosMenus()
    {
        menu_ayuntamiento?.OnVolver();
        menu_cantera?.OnVolver();
        menu_aserradero?.OnVolver();
        menu_herreria?.OnVolver();
        menu_mina?.OnVolver();
        menu_barrio?.OnVolver();

        menuAbierto = false;
    }

    private void SetBotonesActivos(bool estado)
    {
        btnAyuntamiento.Disabled = !estado;
        btnCantera.Disabled = !estado;
        btnAserradero.Disabled = !estado;
        btnHerreria.Disabled = !estado;
        btnMina.Disabled = !estado;
        btnBarrio.Disabled = !estado;
    }

    public void MenuCerrado()
    {
        menuAbierto = false;
        SetBotonesActivos(true);
    }

    private void OnAyuntamiento()
    {
        if (menuAbierto) return;

        CerrarTodosLosMenus();
        menu_ayuntamiento?.Abrir();

        menuAbierto = true;
        SetBotonesActivos(false);
    }

    private void OnCantera()
    {
        if (menuAbierto) return;

        CerrarTodosLosMenus();
        menu_cantera?.Abrir();

        menuAbierto = true;
        SetBotonesActivos(false);
    }

    private void OnAserradero()
    {
        if (menuAbierto) return;

        CerrarTodosLosMenus();
        menu_aserradero?.Abrir();

        menuAbierto = true;
        SetBotonesActivos(false);
    }

    private void OnHerreria()
    {
        if (menuAbierto) return;

        CerrarTodosLosMenus();
        menu_herreria?.Abrir();

        menuAbierto = true;
        SetBotonesActivos(false);
    }

    private void OnMina()
    {
        if (menuAbierto) return;

        CerrarTodosLosMenus();
        menu_mina?.Abrir();

        menuAbierto = true;
        SetBotonesActivos(false);
    }

    private void OnBarrio()
    {
        if (menuAbierto) return;

        CerrarTodosLosMenus();
        menu_barrio?.Abrir();

        menuAbierto = true;
        SetBotonesActivos(false);
    }

	private void OnMapa()
	{
		if (menuAbierto) return;

		CerrarTodosLosMenus();
		GetTree().ChangeSceneToFile("res://scenes/level/aldea/mapa_mundi.tscn");
	}
}