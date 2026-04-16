using Godot;
using System;

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

		menu_ayuntamiento = GetNodeOrNull<menu_ayuntamiento>("BotonAyuntamiento/MenuAyuntamiento");
		menu_cantera = GetNodeOrNull<menu_cantera>("BotonCantera/MenuCantera");
		menu_aserradero = GetNodeOrNull<menu_aserradero>("BotonAserradero/MenuAserradero");
		menu_herreria = GetNodeOrNull<menu_herreria>("BotonHerreria/MenuHerreria");
		menu_mina = GetNodeOrNull<menu_mina>("BotonMina/MenuMina");
		menu_barrio = GetNodeOrNull<menu_barrio>("BotonBarrio/MenuBarrio");
		btnAyuntamiento = GetNodeOrNull<Button>("BotonAyuntamiento");
		btnCantera = GetNodeOrNull<Button>("BotonCantera");
		btnAserradero = GetNodeOrNull<Button>("BotonAserradero");
		btnHerreria = GetNodeOrNull<Button>("BotonHerreria");
		btnMina = GetNodeOrNull<Button>("BotonMina");
		btnBarrio = GetNodeOrNull<Button>("BotonBarrio");

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
