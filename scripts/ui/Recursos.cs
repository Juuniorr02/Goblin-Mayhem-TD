using Godot;
using System.Text.Json;

public partial class Recursos : Node
{
    public static Recursos Instance;

    private const string PATH = "user://recursos.json";

	private Timer autosaveTimer;

    // =========================
    // 🎮 RECURSOS PARTIDA (RAM)
    // =========================
    public int Gold;
    public int Wood;
    public int Stone;
    public int Iron;

    // =========================
    // 🏠 RECURSOS BASE NIVEL
    // =========================
    public int BaseGold = 500;
    public int BaseWood = 200;
    public int BaseStone = 0;
    public int BaseIron = 0;

    // =========================
    // 🏗️ PRODUCCIÓN ALDEA
    // =========================
    public int ProdGold { get; set; }
    public int ProdWood { get; set; }
    public int ProdStone { get; set; }
    public int ProdIron { get; set; }

    // =========================
    // 💰 RECURSOS TOTALES (SAVE)
    // =========================
    public int TotalGold { get; set; } = 5000;
    public int TotalWood { get; set; } = 5000;
    public int TotalStone { get; set; } = 5000;
    public int TotalIron { get; set; } = 5000;

    // =========================
    // 📦 STRUCT DE GUARDADO
    // =========================
    private class SaveDataStruct
    {
        public int TotalGold { get; set; }
        public int TotalWood { get; set; }
        public int TotalStone { get; set; }
        public int TotalIron { get; set; }

        public int ProdGold { get; set; }
        public int ProdWood { get; set; }
        public int ProdStone { get; set; }
        public int ProdIron { get; set; }
    }

    // =========================
    // 🚀 INIT
    // =========================
    public override void _Ready()
    {

		autosaveTimer = new Timer();
    	autosaveTimer.WaitTime = 10.0f;
    	autosaveTimer.Autostart = true;
    	autosaveTimer.OneShot = false;

    	autosaveTimer.Timeout += OnAutosaveTimeout;

        Instance = this;
        LoadData();

		AddChild(autosaveTimer);
    }

    // =========================
    // 🎮 INICIO DE PARTIDA
    // =========================
    public void StartLevel()
    {
        Gold = BaseGold + ProdGold;
        Wood = BaseWood + ProdWood;
        Stone = BaseStone + ProdStone;
        Iron = BaseIron + ProdIron;
    }

    // =========================
    // 🔁 PRODUCCIÓN POR RONDA
    // =========================
    public void AddProduction()
    {
        Gold += ProdGold;
        Wood += ProdWood;
        Stone += ProdStone;
        Iron += ProdIron;
    }

    // =========================
    // 🏁 FIN DE PARTIDA (GUARDAR)
    // =========================
    public void EndLevel()
    {
        TotalGold += Gold;
        TotalWood += Wood;
        TotalStone += Stone;
        TotalIron += Iron;

        SaveData();
    }

	private void OnAutosaveTimeout()
	{
	GD.Print("AUTOSAVE");
    SaveData();
	}

    // =========================
    // 💾 GUARDAR
    // =========================
    public void SaveData()
    {
        var data = new SaveDataStruct
        {
            TotalGold = TotalGold,
            TotalWood = TotalWood,
            TotalStone = TotalStone,
            TotalIron = TotalIron,

            ProdGold = ProdGold,
            ProdWood = ProdWood,
            ProdStone = ProdStone,
            ProdIron = ProdIron
        };

        string json = JsonSerializer.Serialize(data);

        using var file = FileAccess.Open(PATH, FileAccess.ModeFlags.Write);
        file.StoreString(json);
    }

    // =========================
    // 📂 CARGAR
    // =========================
    public void LoadData()
	{
    	if (!FileAccess.FileExists(PATH))
    	{
        	SaveData();
       		return;
    	}

    	using var file = FileAccess.Open(PATH, FileAccess.ModeFlags.Read);
    	string json = file.GetAsText();

    	// 🔥 Evitar JSON vacío o inválido
    	if (string.IsNullOrEmpty(json) || json == "{}")
    	{
        	GD.Print("JSON vacío, usando valores por defecto");
        	SaveData();
        	return;
    	}

    	var data = JsonSerializer.Deserialize<SaveDataStruct>(json);

    	// 🔥 Validar que no sea null
    	if (data == null)
    	{
        	GD.Print("Error al deserializar, usando valores por defecto");
        	SaveData();
        	return;
    	}

    	TotalGold = data.TotalGold;
    	TotalWood = data.TotalWood;
    	TotalStone = data.TotalStone;
    	TotalIron = data.TotalIron;

    	ProdGold = data.ProdGold;
    	ProdWood = data.ProdWood;
    	ProdStone = data.ProdStone;
    	ProdIron = data.ProdIron;
	}
}