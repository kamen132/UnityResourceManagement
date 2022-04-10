using System.Diagnostics;
using System.Text;

public static class CommandLineTools
{
    public static bool MacCommand(string shellPath, string args = "", string WorkingDirectory = "")
    {
        ProcessStartInfo start = null;
        if(string.IsNullOrEmpty(args))
        {
            start = new ProcessStartInfo("/bin/bash", shellPath);
        }else
        {
            start = new ProcessStartInfo("/bin/bash", shellPath + " " + args);
        }
        start.CreateNoWindow = false;
        start.ErrorDialog = true;
        start.UseShellExecute = false;
        start.WorkingDirectory = WorkingDirectory;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;
        start.RedirectStandardInput = true;
        start.StandardOutputEncoding = Encoding.UTF8;
        start.StandardErrorEncoding = Encoding.UTF8;

        Process p = Process.Start(start);

        UnityEngine.Debug.Log("common print: " + p.StandardOutput.ReadToEnd());
        string __err = p.StandardError.ReadToEnd();
        if (string.IsNullOrEmpty(__err) == false)
        {

            UnityEngine.Debug.LogError(__err);
            //EditorUtility.DisplayDialog("command error", "command error，请勿清空控制台！", "确定");
            //return false;
        }

        p.WaitForExit();
        p.Close();

        return true;
    }
    public static bool WinCommand(string exePath, string arguments = "", string WorkingDirectory = "")
    {
        ProcessStartInfo start = null;
        if(string.IsNullOrEmpty(arguments))
        {
            start = new ProcessStartInfo(exePath);
        }else
        {
            start = new ProcessStartInfo(exePath, arguments);
        }
        start.WorkingDirectory = WorkingDirectory;
        start.CreateNoWindow = false;
        start.ErrorDialog = true;
        start.UseShellExecute = false;

        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;
        start.RedirectStandardInput = true;
        start.StandardOutputEncoding = Encoding.UTF8;
        start.StandardErrorEncoding = Encoding.UTF8;

        Process p = Process.Start(start);
        UnityEngine.Debug.Log("command print: " + p.StandardOutput.ReadToEnd());
        string __err = p.StandardError.ReadToEnd();
        if (string.IsNullOrEmpty(__err) == false)
        {
            //UnityEngine.Debug.LogError(__err);
            //EditorUtility.DisplayDialog("command error", "command error，请勿清空控制台！", "确定");
            //return false;
        }

        p.WaitForExit();
        p.Close();

        return true;
    }
}

