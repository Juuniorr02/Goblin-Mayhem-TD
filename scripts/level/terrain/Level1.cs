using Godot;
using System;

public partial class Level1 : Node2D
{
	public override void _Ready()
	{
		MusicManager music = GetNode<MusicManager>("/root/MusicManager");

        music.PlayMusic("res://assets/music/final.wav");
	}
}
