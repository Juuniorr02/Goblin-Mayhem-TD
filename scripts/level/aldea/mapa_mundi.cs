using Godot;
using System;

public partial class mapa_mundi : Node2D
{
	public String nombreNivel;

	private entrar_nivel entrar_nivel;

	private Button btnTutorial;
	private Button btnMontana1;
	private Button btnPantano1;
	private Button btnPantano2;
	private Button btnIslas1;
	private Button btnCastilloMalvado;

	private bool menuAbierto = false;
	
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;

		entrar_nivel = GetNodeOrNull<entrar_nivel>("entrar_nivel");

		entrar_nivel.mapa_mundi = this;

		btnTutorial = GetNodeOrNull<Button>("Botones/Tutorial");
		btnMontana1 = GetNodeOrNull<Button>("Botones/Montana1");
		btnPantano1 = GetNodeOrNull<Button>("Botones/Pantano1");
		btnPantano2 = GetNodeOrNull<Button>("Botones/Pantano2");
		btnIslas1 = GetNodeOrNull<Button>("Botones/Islas1");
		btnCastilloMalvado = GetNodeOrNull<Button>("Botones/Castillo Malvado");

		ConfigurarBoton(btnTutorial);
		ConfigurarBoton(btnMontana1);
		ConfigurarBoton(btnPantano1);
		ConfigurarBoton(btnPantano2);
		ConfigurarBoton(btnIslas1);
		ConfigurarBoton(btnCastilloMalvado);

		if (btnTutorial != null)
			btnTutorial.Pressed += OnTutorial;
		
		if (btnMontana1 != null)
			btnMontana1.Pressed += OnMontana1;

		if (btnPantano1 != null)
			btnPantano1.Pressed += OnPantano1;

		if (btnPantano2 != null)
			btnPantano2.Pressed += OnPantano2;

		if (btnIslas1 != null)
			btnIslas1.Pressed += OnIslas1;

		if (btnCastilloMalvado != null)
			btnCastilloMalvado.Pressed += OnCastilloMalvado;
	}

	private void ConfigurarBoton(Button b)
    {
        if (b == null) return;

        b.ProcessMode = ProcessModeEnum.Pausable;
        b.MouseFilter = Control.MouseFilterEnum.Stop;
    }

	public void OnTutorial()
	{
		if (menuAbierto) return;
		nombreNivel = "Tutorial";
        entrar_nivel?.Abrir();

        menuAbierto = true;
	}

	public void OnMontana1()
	{
		if (menuAbierto) return;
		nombreNivel = "Montana1";
		entrar_nivel?.Abrir();

		menuAbierto = true;
	}

	public void OnPantano1()
	{
		if (menuAbierto) return;
		nombreNivel = "Pantano1";
		entrar_nivel?.Abrir();

		menuAbierto = true;
	}

	public void OnPantano2()
	{
		if (menuAbierto) return;
		nombreNivel = "Pantano2";
		entrar_nivel?.Abrir();

		menuAbierto = true;
	}

	public void OnIslas1()
	{
		if (menuAbierto) return;
		nombreNivel = "Islas1";
		entrar_nivel?.Abrir();

		menuAbierto = true;
	}

	public void OnCastilloMalvado()
	{
		if (menuAbierto) return;
		nombreNivel = "Castillo Malvado";
		entrar_nivel?.Abrir();

		menuAbierto = true;
	}

	public void CerrarMenu()
	{
		menuAbierto = false;
	}
}
