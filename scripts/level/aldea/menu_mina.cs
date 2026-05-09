using Godot;
using System;

public partial class menu_mina : CanvasLayer
{
	public Aldea aldea;
	public Label labelInfo;
	public Label labelNombre;
	public Button btnMejorar;
	public Button btnVolver;
	public int Hierro;
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
		if (labelNombre.Text == "Mina Nivel 0")
		{
			labelInfo.Text = "  Producción: 0 de hierro por ronda  " + "\n  Coste desbloqueo: 200 de oro  ";
		}
		else if (labelNombre.Text == "Mina Nivel 1")
		{
			labelInfo.Text = "  Producción: 25 de hierro por ronda  " + "\n  Coste mejora: 400 de oro  ";
		}
		else if (labelNombre.Text == "Mina Nivel 2")
		{
			labelInfo.Text = "  Producción: 50 de hierro por ronda  " + "\n  Coste mejora: 700 de oro, 100 de madera  ";
		}
		else if (labelNombre.Text == "Mina Nivel 3")
		{
			labelInfo.Text = "  Producción: 75 de hierro por ronda  " + "\n  Coste mejora: 900 de oro, 180 de madera, 70 de piedra  ";
		}
		else if (labelNombre.Text == "Mina Nivel 4")
		{
			labelInfo.Text = "  Producción: 100 de hierro por ronda  " + "\n  Coste mejora: 1200 de oro, 250 de madera, 140 de piedra, 70 de hierro  ";
		}
		else if (labelNombre.Text == "Mina Nivel 5")
		{
			labelInfo.Text = "  Producción: 150 de hierro por ronda  ";
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

		if(labelNombre.Text == "Mina Nivel 0")
		{
			amountGold = 200; amountWood = 0; amountStone = 0; amountIron = 0;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "Mina Nivel 1";
				Hierro = 25;
				Recursos.Instance.ProdIron = Hierro;
				Recursos.Instance.SaveData();
				SaveSystem.Instance.SaveGame();
				labelInfo.Text = "  Producción: 25 de hierro por ronda  " + "\n  Coste mejora: 400 de oro  ";
			}
			else
			{
				GD.Print(Recursos.Instance.TotalGold, " ", Recursos.Instance.TotalWood, " ", Recursos.Instance.TotalStone, " ", Recursos.Instance.TotalIron);
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}

		}
		if(labelNombre.Text == "Mina Nivel 1")
		{
			amountGold = 400; amountWood = 0; amountStone = 0; amountIron = 0;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "Mina Nivel 2";
				Hierro = 50;
				Recursos.Instance.ProdIron = Hierro;
				Recursos.Instance.SaveData();
				SaveSystem.Instance.SaveGame();
				labelInfo.Text = "  Producción: 50 de hierro por ronda  " + "\n  Coste mejora: 700 de oro, 100 de madera  ";
			}
			else
			{
				GD.Print(Recursos.Instance.TotalGold, " ", Recursos.Instance.TotalWood, " ", Recursos.Instance.TotalStone, " ", Recursos.Instance.TotalIron);
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}

		}
		else if(labelNombre.Text == "Mina Nivel 2")
		{
			amountGold = 700; amountWood = 100; amountStone = 0; amountIron = 0;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "Mina Nivel 3";
				Hierro = 75;
				Recursos.Instance.ProdIron = Hierro;
				Recursos.Instance.SaveData();
				SaveSystem.Instance.SaveGame();
				labelInfo.Text = "  Producción: 75 de hierro por ronda  " + "\n  Coste mejora: 900 de oro, 180 de madera, 70 de piedra  ";
			}
			else
			{
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}
		}
		else if(labelNombre.Text == "Mina Nivel 3")
		{
			amountGold = 900; amountWood = 180; amountStone = 70; amountIron = 0;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "Mina Nivel 4";
				Hierro = 100;
				Recursos.Instance.ProdIron = Hierro;
				Recursos.Instance.SaveData();
				SaveSystem.Instance.SaveGame();
				labelInfo.Text = "  Producción: 100 de hierro por ronda  " + "\n  Coste mejora: 1200 de oro, 250 de madera, 140 de piedra, 70 de hierro  ";
			}
			else
			{
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}
		}
		else if(labelNombre.Text == "Mina Nivel 4")
		{
			amountGold = 1200; amountWood = 250; amountStone = 140; amountIron = 70;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "Mina Nivel 5";
				Hierro = 150;
				Recursos.Instance.ProdIron = Hierro;
				Recursos.Instance.SaveData();
				SaveSystem.Instance.SaveGame();
				labelInfo.Text = "  Producción: 150 de hierro por ronda  ";
				btnMejorar.Disabled = true;
			}
			else
			{
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}
		}
	}
}