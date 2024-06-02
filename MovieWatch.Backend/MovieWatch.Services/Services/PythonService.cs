using System.Diagnostics;
using System.Text;
using CliWrap;
using Microsoft.Extensions.Options;
using MovieWatch.Data.Configurations;

namespace MovieWatch.Services.Services;

public interface IPythonService
{
    Task<(bool, string, string)> RunScript(CancellationToken cancellationToken = default);
}

public class PythonService : IPythonService
{
    private readonly IOptionsMonitor<PyhtonConfiguration> _pythonConfiguration;
    public PythonService(IOptionsMonitor<PyhtonConfiguration> pythonConfiguration)
    {
        _pythonConfiguration = pythonConfiguration;
    }
    
    public async Task<(bool, string, string)> RunScript(CancellationToken cancellationToken = default)
    {
        var stdOutBuffer = new StringBuilder();
        var stdErrBuffer = new StringBuilder();
        
        var scriptsPath = Path.GetFullPath(_pythonConfiguration.CurrentValue.Scripts);
        var scriptPath = Path.Combine(scriptsPath, "content_based_filtering.py");
        var pythonPath = Path.GetFullPath(_pythonConfiguration.CurrentValue.Runtime);
        var storagePath = Path.GetFullPath(_pythonConfiguration.CurrentValue.Storage);
        
        var input = Path.Combine(storagePath, "movies.csv");
        var output = Path.Combine(storagePath, "movies.json");
        

        var pythonExecutable = Path.GetFullPath(pythonPath, Directory.GetCurrentDirectory());
        
        var args = new List<string>
        {
            $"--input \"{input}\"",
            "--col-weights Title:0.2 Description:0.3 Genres:0.5",
            "--max-returned 10",
            $"--output \"{output}\""
        };
        
        

        var cmd = Cli.Wrap(pythonExecutable)
            .WithArguments($"{scriptPath} {string.Join(" ", args)}")
            .WithValidation(CommandResultValidation.None)
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer));
        
        Console.WriteLine(cmd.ToString());

        var result = await cmd.ExecuteAsync(cancellationToken);

        var stdOut = stdOutBuffer.ToString().Replace(Environment.NewLine, " ");
        var stdErr = stdErrBuffer.ToString().Replace(Environment.NewLine, " ");

        return (result.ExitCode == 0, stdOut, stdErr);
    }
    
}