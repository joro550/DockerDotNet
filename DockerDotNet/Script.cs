using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DockerDotNet;

public record Image(string Name)
{
    public async Task<Container> TryRunAsContainer()
    {
        return new Container("1234");
    }
}

public record Container(string Id);

public abstract class CommandLine
{
    public abstract Task<string> Accept(IVisitor visitor);
}

public class RunImage : CommandLine
{
    public override async Task<string> Accept(IVisitor visitor) 
        => await visitor.Visit(this);
}

public interface IVisitor
{
    Task<string> Visit(CommandLine commandLine);
}

public class DockerVisitor : IVisitor
{
    public Task<string> Visit(CommandLine commandLine)
    {
        throw new NotImplementedException();
    }
}

internal static class Command
{
    public static async Task RunAsync(string val, CancellationToken cancellationToken = default)
    {
        if (OperatingSystem.IsWindows()) 
            await val.BatAsync(cancellationToken);
        
        
        if (OperatingSystem.IsMacOS()) 
            await val.BashAsync(cancellationToken);
    }
}

internal static class OperatingSystem
{
    public static bool IsWindows() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    public static bool IsMacOS() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

    public static bool IsLinux() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
}

internal static class Shell
{
    public static async Task<string> BashAsync(this string cmd, CancellationToken cancellationToken = default)
    {
        var escapedArgs = cmd.Replace("\"", "\\\"");
        return await RunAsync("/bin/bash", $"-c \"{escapedArgs}\"", cancellationToken);
    }

    public static async Task<string> BatAsync(this string cmd, CancellationToken cancellationToken = default)
    {
        var escapedArgs = cmd.Replace("\"", "\\\"");
        return await RunAsync("cmd.exe", $"/c \"{escapedArgs}\"", cancellationToken);
    }

    private static async Task<string> RunAsync (string filename, string arguments, CancellationToken cancellationToken = default)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = filename,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = false,
            }
        };
        
        process.Start();
        var result = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync(cancellationToken);
        return result;
    }
}