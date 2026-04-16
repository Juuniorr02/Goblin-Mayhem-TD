using Godot;
using System;

public partial class menu_herreria : CanvasLayer
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
