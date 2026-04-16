using Godot;
using System;

public partial class menu_cantera : CanvasLayer
{
	public override void _Ready()
	{
	}

	public void Abrir()
	{
		Visible = true;
	}

	public void Cerrar()
	{
		Visible = false;
	}
}
