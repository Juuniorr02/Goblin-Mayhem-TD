using Godot;
using System;

public partial class menu_ayuntamiento : CanvasLayer
{
	public Aldea aldea;
	public Label labelInfo;
	public Label labelNombre;
	public Button btnMejorar;
	public Button btnVolver;
	public int Oro;

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
		b.FocusMode = Control.FocusModeEnum.None;
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
		if (Recursos.Instance.ProdGold == 100)
		{
			labelNombre.Text = "  Ayuntamiento Nivel 1  ";
			labelInfo.Text = "  Producción: 100 de oro por ronda  " + "\n  Coste mejora: 400 de oro  ";
			btnMejorar.Disabled = false;
		}
		else if (Recursos.Instance.ProdGold == 200)
		{
			labelNombre.Text = "  Ayuntamiento Nivel 2  ";
			labelInfo.Text = "  Producción: 200 de oro por ronda  " + "\n  Coste mejora: 500 de oro, 50 de madera  ";
			btnMejorar.Disabled = false;
		}
		else if (Recursos.Instance.ProdGold == 300)
		{
			labelInfo.Text = "  Producción: 300 de oro por ronda  " + "\n  Coste mejora: 700 de oro, 100 de madera, 25 de piedra  ";
			btnMejorar.Disabled = false;
		}
		else if (Recursos.Instance.ProdGold == 400)
		{
			labelNombre.Text = "  Ayuntamiento Nivel 4  ";
			labelInfo.Text = "  Producción: 400 de oro por ronda  " + "\n  Coste mejora: 1000 de oro, 200 de madera, 100 de piedra, 25 de hierro  ";
			btnMejorar.Disabled = false;
		}
		else if (Recursos.Instance.ProdGold == 500)
		{
			labelNombre.Text = "  Ayuntamiento Nivel 5  ";
			labelInfo.Text = "  Producción: 500 de oro por ronda  ";
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

		if(labelNombre.Text == "  Ayuntamiento Nivel 1  ")
		{
			amountGold = 400; amountWood = 0; amountStone = 0; amountIron = 0;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "  Ayuntamiento Nivel 2  ";
				Oro = 200;
				Recursos.Instance.ProdGold = Oro;
				Recursos.Instance.SaveData();
				SaveSystem.Instance.SaveGame();
				labelInfo.Text = "  Producción: 200 de oro por ronda  " + "\n  Coste mejora: 500 de oro, 50 de madera  ";
			}
			else
			{
				GD.Print(Recursos.Instance.TotalGold, " ", Recursos.Instance.TotalWood, " ", Recursos.Instance.TotalStone, " ", Recursos.Instance.TotalIron);
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}

		}
		else if(labelNombre.Text == "  Ayuntamiento Nivel 2  ")
		{
			amountGold = 500; amountWood = 50; amountStone = 0; amountIron = 0;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "  Ayuntamiento Nivel 3  ";
				Oro = 300;
				Recursos.Instance.ProdGold = Oro;
				Recursos.Instance.SaveData();
				SaveSystem.Instance.SaveGame();
				labelInfo.Text = "  Producción: 300 de oro por ronda  " + "\n  Coste mejora: 700 de oro, 100 de madera, 25 de piedra  ";
			}
			else
			{
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}
		}
		else if(labelNombre.Text == "  Ayuntamiento Nivel 3  ")
		{
			amountGold = 700; amountWood = 100; amountStone = 25; amountIron = 0;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "  Ayuntamiento Nivel 4  ";
				Oro = 400;
				Recursos.Instance.ProdGold = Oro;
				Recursos.Instance.SaveData();
				SaveSystem.Instance.SaveGame();
				labelInfo.Text = "  Producción: 400 de oro por ronda  " + "\n  Coste mejora: 1000 de oro, 200 de madera, 100 de piedra, 25 de hierro  ";
			}
			else
			{
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}
		}
		else if(labelNombre.Text == "  Ayuntamiento Nivel 4  ")
		{
			amountGold = 1000; amountWood = 200; amountStone = 100; amountIron = 25;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "  Ayuntamiento Nivel 5  ";
				Oro = 500;
				Recursos.Instance.ProdGold = Oro;
				Recursos.Instance.SaveData();
				SaveSystem.Instance.SaveGame();
				labelInfo.Text = "  Producción: 500 de oro por ronda  ";
				btnMejorar.Disabled = true;
			}
			else
			{
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}
		}
	}
}
