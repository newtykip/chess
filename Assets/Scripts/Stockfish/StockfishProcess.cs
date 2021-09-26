using System;
using System.Diagnostics;
using System.IO;

class StockfishProcess
{
    private ProcessStartInfo ProcessStartInfo { get; set; }
    private Process Process { get; set; }

    public StockfishProcess(string path)
    {
        this.ProcessStartInfo = new ProcessStartInfo()
        {
            FileName = path,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };
        this.Process = new Process()
        {
            StartInfo = this.ProcessStartInfo
        };
    }

    public void Wait(int millisecond) => this.Process.WaitForExit(millisecond);

    public void WriteLine(string command)
    {
        if (this.Process.StandardInput == null)
            throw new NullReferenceException();
        ((TextWriter) this.Process.StandardInput).WriteLine(command);
        ((TextWriter) this.Process.StandardInput).Flush();
    }

    public string ReadLine() => this.Process.StandardOutput != null
        ? ((TextReader) this.Process.StandardOutput).ReadLine()
        : throw new NullReferenceException();

    public void Start() => this.Process.Start();

    ~StockfishProcess() => this.Process.Close();
}