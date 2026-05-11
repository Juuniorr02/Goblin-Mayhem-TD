using Godot;
using System;

public partial class menu_barrio : CanvasLayer
{
	public Aldea aldea;
	public Label labelInfo;
	public Label labelNombre;
	public Button btnMejorar;
	public Button btnVolver;
	public int Vida;
	private bool isPaused = false;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;

		labelInfo = GetNodeOrNull<Label>("CenterContainer/PanelContainer/MarginContainer/PanelContainer/VBoxContainer/VBoxContainer/Info");
		labelNombre = GetNodeOrNull<Label>("CenterContainer/PanelContainer/MarginContainer/PanelContainer/VBoxContainer/VBoxContainer/Nombre");
		btnMejorar = GetNodeOrNull<Button>("CenterContainer/PanelContainer/MarginContainer/PanelContainer/VBoxContainer/VBoxContainer/Mejorar");
		btnVolver = GetNodeOrNull<Button>("CenterContainer/PanelContainer/MarginContainer/PanelContainer/VBoxContainer/VBoxContainer/Volver");

		ConfigurarBoton(btnMejorar);
		ConfigurarBoton(btnVolver);

		if (btnMejorar != null)
			btnMejorar.Pressed += OnMejorar;

		if (btnVolver != null)
			btnVolver.Pressed += OnVolver;

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
            if (isPaused) OnVolver();
        }
    }

	public void Abrir()
	{
		if (Recursos.Instance.ProdVida == 0)
		{
			labelNombre.Text = "  Barrio Nivel 1  ";
			labelInfo.Text = "  Vida Maxima: 100  " + "\n  Coste mejora: 500 de oro  ";
		}
		else if (Recursos.Instance.ProdVida == 25)
		{
			labelNombre.Text = "  Barrio Nivel 2  ";
			labelInfo.Text = "  Vida Maxima: 125  " + "\n  Coste mejora: 600 de oro, 50 de madera  ";
		}
		else if (Recursos.Instance.ProdVida == 50)
		{
			labelNombre.Text = "  Barrio Nivel 3  ";
			labelInfo.Text = "  Vida Maxima: 150  " + "\n  Coste mejora: 800 de oro, 100 de madera, 50 de piedra  ";
		}
		else if (Recursos.Instance.ProdVida == 75)
		{
			labelNombre.Text = "  Barrio Nivel 4  ";
			labelInfo.Text = "  Vida Maxima: 175  " + "\n  Coste mejora: 1000 de oro, 150 de madera, 100 de piedra, 25 de hierro  ";
		}
		else if (Recursos.Instance.ProdVida == 100)
		{
			labelNombre.Text = "  Barrio Nivel 5  ";
			labelInfo.Text = "  Vida Maxima: 200  ";
			btnMejorar.Disabled = true;
		}

        isPaused = true;
        GetTree().Paused = true;
        Visible = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
	}

	public void OnVolver()
	{
		isPaused = false;
        GetTree().Paused = false;
		aldea.MenuCerrado();
        Visible = false;
	}

	public void OnMejorar()
	{
		int amountGold, amountWood, amountStone, amountIron;

		if(labelNombre.Text == "Barrio Nivel 1")
		{
			amountGold = 500; amountWood = 0; amountStone = 0; amountIron = 0;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "Barrio Nivel 2";
				Vida = 25;
				Recursos.Instance.ProdVida = Vida;
				Recursos.Instance.SaveData();
				SaveSystem.Instance.SaveGame();
				labelInfo.Text = "  Vida Maxima: 125  " + "\n  Coste mejora: 600 de oro, 50 de madera  ";
			}
			else
			{
				GD.Print(Recursos.Instance.TotalGold, " ", Recursos.Instance.TotalWood, " ", Recursos.Instance.TotalStone, " ", Recursos.Instance.TotalIron);
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}

		}
		else if(labelNombre.Text == "Barrio Nivel 2")
		{
			amountGold = 600; amountWood = 50; amountStone = 0; amountIron = 0;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "Barrio Nivel 3";
				Vida = 50;
				Recursos.Instance.ProdVida = Vida;
				Recursos.Instance.SaveData();
				SaveSystem.Instance.SaveGame();
				labelInfo.Text = "  Vida Maxima: 150  " + "\n  Coste mejora: 800 de oro, 100 de madera, 50 de piedra  ";
			}
			else
			{
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}
		}
		else if(labelNombre.Text == "Barrio Nivel 3")
		{
			amountGold = 800; amountWood = 100; amountStone = 50; amountIron = 0;

			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "Barrio Nivel 4";
				Vida = 75;
				Recursos.Instance.ProdVida = Vida;
				Recursos.Instance.SaveData();
				SaveSystem.Instance.SaveGame();
				labelInfo.Text = "  Vida Maxima: 175  " + "\n  Coste mejora: 1000 de oro, 150 de madera, 100 de piedra, 25 de hierro  ";
			}
			else
			{
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}
		}
		else if(labelNombre.Text == "Barrio Nivel 4")
		{
			amountGold = 1000; amountWood = 150; amountStone = 100; amountIron = 25;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "Barrio Nivel 5";
				Vida = 100;
				Recursos.Instance.ProdVida = Vida;
				Recursos.Instance.SaveData();
				SaveSystem.Instance.SaveGame();
				labelInfo.Text = "  Vida Maxima: 200  ";
				btnMejorar.Disabled = true;
			}
			else
			{
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}
		}
	}
}