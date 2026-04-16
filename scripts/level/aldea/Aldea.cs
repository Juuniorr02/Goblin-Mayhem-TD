using Godot;

public partial class Aldea : Node2D
{
	public Button btnAyuntamiento;
	public Button btnCantera;
	public Button btnAserradero;
	public Button btnHerreria;
	public Button btnMina;
	public Button btnBarrio;
	private menu_ayuntamiento menu_ayuntamiento;
	private menu_cantera menu_cantera;
	private menu_aserradero menu_aserradero;
	private menu_herreria menu_herreria;
	private menu_mina menu_mina;
	private menu_barrio menu_barrio;

	private bool isPaused = false;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;

		menu_ayuntamiento = GetNodeOrNull<menu_ayuntamiento>("Botones/BotonAyuntamiento/menu_ayuntamiento");
		menu_cantera = GetNodeOrNull<menu_cantera>("Botones/BotonCantera/menu_cantera");
		menu_aserradero = GetNodeOrNull<menu_aserradero>("Botones/BotonAserradero/menu_aserradero");
		menu_herreria = GetNodeOrNull<menu_herreria>("Botones/BotonHerreria/menu_herreria");
		menu_mina = GetNodeOrNull<menu_mina>("Botones/BotonMina/menu_mina");
		menu_barrio = GetNodeOrNull<menu_barrio>("Botones/BotonBarrio/menu_barrio");
		btnAyuntamiento = GetNodeOrNull<Button>("Botones/BotonAyuntamiento");
		btnCantera = GetNodeOrNull<Button>("Botones/BotonCantera");
		btnAserradero = GetNodeOrNull<Button>("Botones/BotonAserradero");
		btnHerreria = GetNodeOrNull<Button>("Botones/BotonHerreria");
		btnMina = GetNodeOrNull<Button>("Botones/BotonMina");
		btnBarrio = GetNodeOrNull<Button>("Botones/BotonBarrio");

		ConfigurarBoton(btnAyuntamiento);
		ConfigurarBoton(btnCantera);
		ConfigurarBoton(btnAserradero);
		ConfigurarBoton(btnHerreria);
		ConfigurarBoton(btnMina);
		ConfigurarBoton(btnBarrio);

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
	}

	private void ConfigurarBoton(Button b)
    {
        if (b == null) return;

        b.ProcessMode = ProcessModeEnum.Always;
        b.MouseFilter = Control.MouseFilterEnum.Stop;
    }

	private void OnAyuntamiento()
	{
		menu_ayuntamiento.Abrir();
	}

	private void OnCantera()
	{
		menu_cantera.Abrir();
	}

	private void OnAserradero()
	{
		menu_aserradero.Abrir();
	}

	private void OnHerreria()
	{
		menu_herreria.Abrir();
	}

	private void OnMina()
	{
		menu_mina.Abrir();
	}

	private void OnBarrio()
	{
		menu_barrio.Abrir();
	}
}
