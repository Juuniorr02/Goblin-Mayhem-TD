using Godot;
using System;

public partial class menu_mina : CanvasLayer
{
	public Aldea aldea;
	public Label labelInfo;
	public Label labelNombre;
	public Button btnMejorar;
	public Button btnVolver;
	public int Hierro = 100;
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
		if (labelNombre.Text == "Mina Nivel 1")
		{
			labelInfo.Text = "  Producción: 100 de hierro por ronda  " + "\n  Coste mejora: 1000 de oro  ";
		}
		else if (labelNombre.Text == "Mina Nivel 2")
		{
			labelInfo.Text = "  Producción: 200 de hierro por ronda  " + "\n  Coste mejora: 2000 de oro, 100 de madera  ";
		}
		else if (labelNombre.Text == "Mina Nivel 3")
		{
			labelInfo.Text = "  Producción: 300 de hierro por ronda  " + "\n  Coste mejora: 3000 de oro, 200 de madera, 100 de piedra  ";
		}
		else if (labelNombre.Text == "Mina Nivel 4")
		{
			labelInfo.Text = "  Producción: 400 de hierro por ronda  " + "\n  Coste mejora: 4000 de oro, 400 de madera, 200 de piedra, 100 de hierro  ";
		}
		else if (labelNombre.Text == "Mina Nivel 5")
		{
			labelInfo.Text = "  Producción: 500 de hierro por ronda  ";
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

		if(labelNombre.Text == "Mina Nivel 1")
		{
			amountGold = 1000; amountWood = 0; amountStone = 0; amountIron = 0;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "Mina Nivel 2";
				Hierro = 200;
				Recursos.Instance.ProdIron = Hierro;
				Recursos.Instance.SaveData();
				labelInfo.Text = "  Producción: 200 de hierro por ronda  " + "\n  Coste mejora: 2000 de oro, 100 de madera  ";
			}
			else
			{
				GD.Print(Recursos.Instance.TotalGold, " ", Recursos.Instance.TotalWood, " ", Recursos.Instance.TotalStone, " ", Recursos.Instance.TotalIron);
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}

		}
		else if(labelNombre.Text == "Mina Nivel 2")
		{
			amountGold = 2000; amountWood = 100; amountStone = 0; amountIron = 0;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "Mina Nivel 3";
				Hierro = 300;
				Recursos.Instance.ProdIron = Hierro;
				Recursos.Instance.SaveData();
				labelInfo.Text = "  Producción: 300 de hierro por ronda  " + "\n  Coste mejora: 3000 de oro, 200 de madera, 100 de piedra  ";
			}
			else
			{
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}
		}
		else if(labelNombre.Text == "Mina Nivel 3")
		{
			amountGold = 3000; amountWood = 200; amountStone = 100; amountIron = 0;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "Mina Nivel 4";
				Hierro = 400;
				Recursos.Instance.ProdIron = Hierro;
				Recursos.Instance.SaveData();
				labelInfo.Text = "  Producción: 400 de hierro por ronda  " + "\n  Coste mejora: 4000 de oro, 400 de madera, 200 de piedra, 100 de hierro  ";
			}
			else
			{
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}
		}
		else if(labelNombre.Text == "Mina Nivel 4")
		{
			amountGold = 4000; amountWood = 400; amountStone = 200; amountIron = 100;
			if(Recursos.Instance.TotalGold >= amountGold && Recursos.Instance.TotalWood >= amountWood && Recursos.Instance.TotalStone >= amountStone && Recursos.Instance.TotalIron >= amountIron)
			{
				Recursos.Instance.TotalGold -= amountGold;
				Recursos.Instance.TotalWood -= amountWood;
				Recursos.Instance.TotalStone -= amountStone;
				Recursos.Instance.TotalIron -= amountIron;
				labelNombre.Text = "Mina Nivel 5";
				Hierro = 500;
				Recursos.Instance.ProdIron = Hierro;
				Recursos.Instance.SaveData();
				labelInfo.Text = "  Producción: 500 de hierro por ronda  ";
				btnMejorar.Disabled = true;
			}
			else
			{
				labelInfo.Text = "  No tienes suficientes recursos.  ";
			}
		}
	}
}