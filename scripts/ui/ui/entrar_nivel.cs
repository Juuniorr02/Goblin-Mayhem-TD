using Godot;
using System;

public partial class entrar_nivel : CanvasLayer
{
	public mapa_mundi mapa_mundi;

	private Label Nombre;
	private Button btnEntrar;
	private Button btnVolver;

	private bool isPaused = false;
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;

		Nombre = GetNodeOrNull<Label>("%Nombre");
		btnEntrar = GetNodeOrNull<Button>("%Entrar");
		btnVolver = GetNodeOrNull<Button>("%Volver");
		Visible = false;
	}
}
