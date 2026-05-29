using System.Diagnostics;

namespace FinTrackAI;

public class Open_Swagger
{
    public static void Open()
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {

            FileName = "http://localhost:5065/swagger/index.html",
            UseShellExecute = true,
        };
        Process process = Process.Start(psi) ?? throw new Exception("Erro ao inicializar o Swagger no navegador");
        process.WaitForExit();
    }
}
