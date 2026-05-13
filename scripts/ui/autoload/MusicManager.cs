using Godot;
using System;

public partial class MusicManager : Node
{
    private AudioStreamPlayer musicPlayer;

    private string currentMusicPath = "";

    public override void _Ready()
    {
        musicPlayer = new AudioStreamPlayer();
        AddChild(musicPlayer);

        musicPlayer.Bus = "Music";

        // 🔥 LOOP UNIVERSAL (funciona para cualquier formato)
        musicPlayer.Finished += OnMusicFinished;
    }

    public void PlayMusic(string musicPath)
    {
        // Evita reiniciar la misma música
        if (currentMusicPath == musicPath)
            return;

        currentMusicPath = musicPath;

        var music = GD.Load<AudioStream>(musicPath);

        if (music == null)
        {
            GD.PrintErr("No se pudo cargar la música: " + musicPath);
            return;
        }

        musicPlayer.Stop();
        musicPlayer.Stream = music;
        musicPlayer.Play();
    }

    public void StopMusic()
    {
        musicPlayer.Stop();
        currentMusicPath = "";
    }

    // 🔥 LOOP MANUAL (sirve para OGG, WAV y MP3)
    private void OnMusicFinished()
    {
        if (musicPlayer.Stream != null)
        {
            musicPlayer.Play();
        }
    }
}