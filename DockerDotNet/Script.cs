using Docker.DotNet;
using Docker.DotNet.Models;
using System.Collections.ObjectModel;

namespace DockerDotNet;

public record Image(string Name)
{
    public async Task<Container> TryRunAsContainer()
    {
        var client = new DockerClientConfiguration()
            .CreateClient();
        
        var response = await client.Containers.CreateContainerAsync(new CreateContainerParameters()
        {
            Image = Name,
            ExposedPorts = new Dictionary<string, EmptyStruct>()
            {
                {"4566", new EmptyStruct()}
            },
            HostConfig = new HostConfig
            {
                AutoRemove = true,
                
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    {"4566", new Collection<PortBinding>{ new() {HostPort = "4566"}}}
                }
            }
        });

        var hasStarted = await client.Containers.StartContainerAsync(response.ID, new ContainerStartParameters());
        return new Container(response.ID);
    }
}

public record Container(string Id)
{
    public async Task<bool> StopAsync()
    {
        var client = new DockerClientConfiguration()
            .CreateClient();
        return await client.Containers.StopContainerAsync(Id, new ContainerStopParameters());
    }
}

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