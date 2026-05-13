using Godot;
using System;

public partial class menu_aserradero : CanvasLayer
{
	public Aldea aldea;
	public Label labelInfo;
	public Label labelNombre;
	public Button btnMejorar;
	public Button btnVolver;
	public int Madera;
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
		if (Recursos.Instance.ProdWood == 50)
		{
			labelNombre.Text = "  Aserradero Nivel 1  ";
			labelInfo.Text = "  Producción: 50 de madera por ronda  " + "\n  Coste mejora: 500 de oro  ";
			btnMejorar.Disabled = false;
		}
		else if (Recursos.Instance.ProdWood == 100)
		{
			labelNombre.Text = "  Aserradero Nivel 2  ";
			labelInfo.Text = "  Producción: 100 de madera por ronda  " + "\n  Coste mejora: 650 de oro, 60 de madera  ";
			btnMejorar.Disabled = false;
		}
		else if (Recursos.Instance.ProdWood == 150)
		{
			labelNombre.Text = "  Aserradero Nivel 3  ";
			labelInfo.Text = "  Producción: 150 de madera por ronda  " + "\n  Coste mejora: 750 de oro, 120 de madera, 40 de piedra  ";
			btnMejorar.Disabled = false;
		}
		else if (Recursos.Instance.ProdWood == 200)
		{
			labelNombre.Text = "  Aserradero Nivel 4  ";
			labelInfo.Text = "  Producción: 200 de madera por ronda  " + "\n  Coste mejora: 1000 de oro, 220 de madera, 120 de piedra, 40 de hierro  ";
			btnMejorar.Disabled = false;
		}
		else if (Recursos.Instance.ProdWood == 300)
		{
			labelNombre.Text = "  Aserradero Nivel 5  ";
			labelInfo.Text = "  Producción: 300 de madera por ronda  ";
			btnMejorar.Disabled = true;
		}
        Visible = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
	}

	public void OnVolver()
	{
		aldea.MenuCerrado();
        Visible = false;
	}

	public void OnMejorar()
	{
		int amountGold, amountWood, amountStone, amountIron;

		if(labelNombre.Text == "  Aserradero Nivel 1  ")
		{
			amountGold = 500; amountWood = 0; amountStone = 0; amountIron = 0;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "  Aserradero Nivel 2  ";
				Madera = 100;
				Recursos.Instance.ProdWood = Madera;
				Recursos.Instance.SaveData();
				SaveSystem.Instance.SaveGame();
				labelInfo.Text = "  Producción: 100 de madera por ronda  " + "\n  Coste mejora: 650 de oro, 60 de madera  ";
			}
			else
			{
				GD.Print(Recursos.Instance.TotalGold, " ", Recursos.Instance.TotalWood, " ", Recursos.Instance.TotalStone, " ", Recursos.Instance.TotalIron);
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}

		}
		else if(labelNombre.Text == "  Aserradero Nivel 2  ")
		{
			amountGold = 650; amountWood = 60; amountStone = 0; amountIron = 0;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "  Aserradero Nivel 3  ";
				Madera = 150;
				Recursos.Instance.ProdWood = Madera;
				Recursos.Instance.SaveData();
				SaveSystem.Instance.SaveGame();
				labelInfo.Text = "  Producción: 150 de madera por ronda  " + "\n  Coste mejora: 750 de oro, 120 de madera, 40 de piedra  ";
			}
			else
			{
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}
		}
		else if(labelNombre.Text == "  Aserradero Nivel 3  ")
		{
			amountGold = 750; amountWood = 120; amountStone = 40; amountIron = 0;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "  Aserradero Nivel 4  ";
				Madera = 200;
				Recursos.Instance.ProdWood = Madera;
				Recursos.Instance.SaveData();
				SaveSystem.Instance.SaveGame();
				labelInfo.Text = "  Producción: 200 de madera por ronda  " + "\n  Coste mejora: 1000 de oro, 220 de madera, 120 de piedra, 40 de hierro  ";
			}
			else
			{
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}
		}
		else if(labelNombre.Text == "  Aserradero Nivel 4  ")
		{
			amountGold = 1000; amountWood = 220; amountStone = 120; amountIron = 40;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "  Aserradero Nivel 5  ";
				Madera = 300;
				Recursos.Instance.ProdWood = Madera;
				Recursos.Instance.SaveData();
				SaveSystem.Instance.SaveGame();
				labelInfo.Text = "  Producción: 300 de madera por ronda  ";
				btnMejorar.Disabled = true;
			}
			else
			{
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}
		}
	}
}