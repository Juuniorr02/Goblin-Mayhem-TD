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

	private AnimatedSprite2D spriteTutorial;
	private AnimatedSprite2D spriteMontana1;
	private AnimatedSprite2D spritePantano1;
	private AnimatedSprite2D spritePantano2;
	private AnimatedSprite2D spriteIslas1;
	private AnimatedSprite2D spriteCastilloMalvado;

	private bool menuAbierto = false;

	public override void _Process(double delta)
	{
    	ActualizarMapa();
	}
	
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		MusicManager music = GetNode<MusicManager>("/root/MusicManager");

        music.PlayMusic("res://assets/music/aldea.wav");

		SaveSystem.Instance.SaveGame();

		BuildTime.CanBuild = true;

		entrar_nivel = GetNodeOrNull<entrar_nivel>("entrar_nivel");

		entrar_nivel.mapa_mundi = this;

		btnTutorial = GetNodeOrNull<Button>("Botones/Tutorial");
		btnMontana1 = GetNodeOrNull<Button>("Botones/Montana1");
		btnPantano1 = GetNodeOrNull<Button>("Botones/Pantano1");
		btnPantano2 = GetNodeOrNull<Button>("Botones/Pantano2");
		btnIslas1 = GetNodeOrNull<Button>("Botones/Islas1");
		btnCastilloMalvado = GetNodeOrNull<Button>("Botones/Castillo Malvado");

		spriteTutorial = GetNodeOrNull<AnimatedSprite2D>("Botones/Tutorial/Tutorial");
		spriteMontana1 = GetNodeOrNull<AnimatedSprite2D>("Botones/Montana1/Montana1");
		spritePantano1 = GetNodeOrNull<AnimatedSprite2D>("Botones/Pantano1/Pantano1");
		spritePantano2 = GetNodeOrNull<AnimatedSprite2D>("Botones/Pantano2/Pantano2");
		spriteIslas1 = GetNodeOrNull<AnimatedSprite2D>("Botones/Islas1/Islas1");
		spriteCastilloMalvado = GetNodeOrNull<AnimatedSprite2D>("Botones/Castillo Malvado/Castillo Malvado");

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
		GameData.MapaMundiVisitado = true;
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

	private void ActualizarMapa()
	{
    	if (GameData.Level1)
		{
    		spriteTutorial?.Play("completado");
		}
		else
		{
    		spriteTutorial?.Play("desbloqueado");
		}

    	spriteMontana1?.Play(GameData.Level2 ? (GameData.Level3 ? "completado" : "desbloqueado") : "bloqueado");

    	spritePantano1?.Play(GameData.Level3 ? (GameData.Level4 ? "completado" : "desbloqueado") : "bloqueado");

    	/*spritePantano2?.Play(GameData.Level3 ? (GameData.Level4 ? "completado" : "desbloqueado") : "bloqueado");*/

    	spriteIslas1?.Play(GameData.Level4 ? (GameData.Level5 ? "completado" : "desbloqueado") : "bloqueado");

    	spriteCastilloMalvado?.Play(GameData.Level5 ? "desbloqueado" : "bloqueado");
}
}
