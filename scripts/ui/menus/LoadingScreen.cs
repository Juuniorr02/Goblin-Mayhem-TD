using Godot;

public partial class LoadingScreen : Control
{
    private string sceneToLoad;
    private ProgressBar progressBar;

    public static string NextScenePath;

    private float visualProgress = 0f;
    private float realProgress = 0f;

    public override void _Ready()
    {
        progressBar = GetNode<ProgressBar>("%ProgressBar");

        // 👉 SOLO NUEVA PARTIDA
        LoadScene(NextScenePath);
    }

    public void LoadScene(string path)
    {
        sceneToLoad = path;
        ResourceLoader.LoadThreadedRequest(sceneToLoad);
    }

    public override void _Process(double delta)
    {
        if (string.IsNullOrEmpty(sceneToLoad))
            return;

        var progress = new Godot.Collections.Array { 0f };
        var status = ResourceLoader.LoadThreadedGetStatus(sceneToLoad, progress);

        realProgress = (float)progress[0] * 100;

        // suavizado (puedes ajustar velocidad si quieres)
        visualProgress = Mathf.Lerp(visualProgress, realProgress, 1f * (float)delta);

        progressBar.Value = visualProgress;

        if (status == ResourceLoader.ThreadLoadStatus.Loaded)
        {
            if (visualProgress >= 99f)
            {
                var scene = ResourceLoader.LoadThreadedGet(sceneToLoad);
                GetTree().ChangeSceneToPacked(scene as PackedScene);
            }
        }
    }
}