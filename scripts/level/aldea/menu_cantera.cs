using Godot;
using System;

public partial class menu_cantera : CanvasLayer
{
	public Aldea aldea;
	public Label labelInfo;
	public Label labelNombre;
	public Button btnMejorar;
	public Button btnVolver;
	public int Piedra;
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
		if (labelNombre.Text == "Cantera Nivel 0")
		{
			labelInfo.Text = "  Producción: 0 de piedra por ronda  " + "\n  Coste mejora: 100 de oro  ";
		}
		else if (labelNombre.Text == "Cantera Nivel 1")
		{
			labelInfo.Text = "  Producción: 25 de piedra por ronda  " + "\n  Coste mejora: 300 de oro  ";
		}
		else if (labelNombre.Text == "Cantera Nivel 2")
		{
			labelInfo.Text = "  Producción: 50 de piedra por ronda  " + "\n  Coste mejora: 600 de oro, 75 de madera  ";
		}
		else if (labelNombre.Text == "Cantera Nivel 3")
		{
			labelInfo.Text = "  Producción: 100 de piedra por ronda  " + "\n  Coste mejora: 750 de oro, 150 de madera, 50 de piedra  ";
		}
		else if (labelNombre.Text == "Cantera Nivel 4")
		{
			labelInfo.Text = "  Producción: 150 de piedra por ronda  " + "\n  Coste mejora: 1000 de oro, 230 de madera, 125 de piedra, 50 de hierro  ";
		}
		else if (labelNombre.Text == "Cantera Nivel 5")
		{
			labelInfo.Text = "  Producción: 200 de piedra por ronda  ";
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

		if(labelNombre.Text == "Cantera Nivel 0")
		{
			amountGold = 100; amountWood = 0; amountStone = 0; amountIron = 0;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "Cantera Nivel 1";
				Piedra = 25;
				Recursos.Instance.ProdStone = Piedra;
				Recursos.Instance.SaveData();
				SaveSystem.Instance.SaveGame();
				labelInfo.Text = "  Producción: 25 de piedra por ronda  " + "\n  Coste mejora: 300 de oro  ";
			}
			else
			{
				GD.Print(Recursos.Instance.TotalGold, " ", Recursos.Instance.TotalWood, " ", Recursos.Instance.TotalStone, " ", Recursos.Instance.TotalIron);
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}

		}
		if(labelNombre.Text == "Cantera Nivel 1")
		{
			amountGold = 300; amountWood = 0; amountStone = 0; amountIron = 0;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "Cantera Nivel 2";
				Piedra = 50;
				Recursos.Instance.ProdStone = Piedra;
				Recursos.Instance.SaveData();
				SaveSystem.Instance.SaveGame();
				labelInfo.Text = "  Producción: 50 de piedra por ronda  " + "\n  Coste mejora: 600 de oro, 75 de madera  ";
			}
			else
			{
				GD.Print(Recursos.Instance.TotalGold, " ", Recursos.Instance.TotalWood, " ", Recursos.Instance.TotalStone, " ", Recursos.Instance.TotalIron);
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}

		}
		else if(labelNombre.Text == "Cantera Nivel 2")
		{
			amountGold = 600; amountWood = 75; amountStone = 0; amountIron = 0;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "Cantera Nivel 3";
				Piedra = 100;
				Recursos.Instance.ProdStone = Piedra;
				Recursos.Instance.SaveData();
				SaveSystem.Instance.SaveGame();
				labelInfo.Text = "  Producción: 100 de piedra por ronda  " + "\n  Coste mejora: 750 de oro, 150 de madera, 50 de piedra  ";
			}
			else
			{
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}
		}
		else if(labelNombre.Text == "Cantera Nivel 3")
		{
			amountGold = 750; amountWood = 150; amountStone = 50; amountIron = 0;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "Cantera Nivel 4";
				Piedra = 150;
				Recursos.Instance.ProdStone = Piedra;
				Recursos.Instance.SaveData();
				SaveSystem.Instance.SaveGame();
				labelInfo.Text = "  Producción: 150 de piedra por ronda  " + "\n  Coste mejora: 1000 de oro, 230 de madera, 125 de piedra, 50 de hierro  ";
			}
			else
			{
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}
		}
		else if(labelNombre.Text == "Cantera Nivel 4")
		{
			amountGold = 1000; amountWood = 230; amountStone = 125; amountIron = 50;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "Cantera Nivel 5";
				Piedra = 200;
				Recursos.Instance.ProdStone = Piedra;
				Recursos.Instance.SaveData();
				SaveSystem.Instance.SaveGame();
				labelInfo.Text = "  Producción: 200 de piedra por ronda  ";
				btnMejorar.Disabled = true;
			}
			else
			{
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}
		}
	}
}