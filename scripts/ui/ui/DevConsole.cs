using Godot;
using System;
using System.Reflection;
using System.Linq;

public partial class DevConsole : CanvasLayer
{
    [Export] private Control consolePanel;
    [Export] private RichTextLabel historyLabel;
    [Export] private LineEdit inputField;
    [Export] private ConsoleCommands commandsNode;

    public override void _Ready()
    {
		GD.Print(">>> ¡AQUÍ ESTOY! <<<");
        // Ocultar al empezar
        if (consolePanel != null) 
        {
            consolePanel.Visible = false;
        }

        // Conectar la señal de texto enviado por código para que no falle
        inputField.TextSubmitted += OnCommandSubmitted;
        
        Log("Sistema de consola listo. Pulsa F10 para abrir.");
    }

    public override void _Input(InputEvent @event)
    {
        // Usamos F10 como tecla de emergencia/fácil
        if (@event is InputEventKey k && k.Pressed && k.Keycode == Key.F10)
        {
            ToggleConsole();
            // Evita que la tecla F10 se escriba o haga otras cosas
            GetViewport().SetInputAsHandled();
        }
    }
	public override void _UnhandledInput(InputEvent @event)
{
    if (@event is InputEventKey k && k.Pressed && k.Keycode == Key.F10)
    {
        GD.Print("¡Tecla F10 detectada!"); // Esto DEBE salir en el Output
        ToggleConsole();
        GetViewport().SetInputAsHandled();
    }
}

    private void ToggleConsole()
    {
        if (consolePanel == null) return;

        consolePanel.Visible = !consolePanel.Visible;

        if (consolePanel.Visible)
        {
            inputField.GrabFocus();
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
        else
        {
            inputField.ReleaseFocus();
        }
    }

    private void OnCommandSubmitted(string fullText)
    {
        if (string.IsNullOrWhiteSpace(fullText)) return;

        inputField.Clear();
        Log($"[User]: {fullText}");

        string[] parts = fullText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string commandName = parts[0].ToLower();
        string[] args = parts.Skip(1).ToArray();

        ExecuteCommand(commandName, args);
    }

    private void ExecuteCommand(string name, string[] args)
    {
        // Buscamos la función en el script de comandos
        MethodInfo method = typeof(ConsoleCommands).GetMethod(name);

        if (method != null)
        {
            try
            {
                ParameterInfo[] paramInfo = method.GetParameters();
                object[] typedArgs = new object[paramInfo.Length];

                // Solo intentamos convertir si hay argumentos suficientes
                for (int i = 0; i < paramInfo.Length; i++)
                {
                    if (i < args.Length)
                        typedArgs[i] = Convert.ChangeType(args[i], paramInfo[i].ParameterType);
                    else
                        typedArgs[i] = Type.Missing; // O dejar nulo si el método lo permite
                }

                object result = method.Invoke(commandsNode, typedArgs);
                if (result != null) Log($"> {result}");
            }
            catch (Exception ex)
            {
                Log($"[Error] Argumentos incorrectos: {ex.Message}");
            }
        }
        else
        {
            Log($"[Error] El comando '{name}' no existe.");
        }
    }

    private void Log(string message)
    {
        if (historyLabel == null) return;
        historyLabel.AppendText(message + "\n");
        // Auto-scroll al final
        historyLabel.ScrollToLine(historyLabel.GetLineCount());
    }
}
