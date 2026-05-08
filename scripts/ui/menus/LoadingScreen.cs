using Godot;
using System.Threading.Tasks;

public partial class LoadingScreen : Control
{
    private string sceneToLoad;
    private ProgressBar progressBar;

    public static string NextScenePath;
    public static bool IsLoadingFromSave = false;

    private float visualProgress = 0f;
    private float realProgress = 0f;

    private SaveSystem save;

    public override void _Ready()
    {
        progressBar = GetNode<ProgressBar>("%ProgressBar");
        save = GetNode<SaveSystem>("/root/SaveSystem");

        if (IsLoadingFromSave)
        {
            LoadFromSave();
        }
        else
        {
            LoadScene(NextScenePath);
        }
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

        // 🔥 suavizado
        visualProgress = Mathf.Lerp(visualProgress, realProgress, 1f * (float)delta);

        progressBar.Value = visualProgress;

        // cuando termina realmente
        if (status == ResourceLoader.ThreadLoadStatus.Loaded)
        {
        // 🔥 esperar a que llegue a 100 visualmente
            if (visualProgress >= 99f)
            {
                var scene = ResourceLoader.LoadThreadedGet(sceneToLoad);
                GetTree().ChangeSceneToPacked(scene as PackedScene);
            }
        }
    }

    private async void LoadFromSave()
    {
        await save.LoadGame();
    }
}