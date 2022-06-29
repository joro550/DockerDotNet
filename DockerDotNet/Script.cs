using Docker.DotNet;
using Docker.DotNet.Models;

namespace DockerDotNet;

public record Image(string Name, Dictionary<string, string> PortMap)
{
    public Image(string name, string hostPort, string containerPort)
        :this(name, new Dictionary<string, string> {{hostPort, containerPort}})
    {
        
    }
    
    public Image(string name, string port)
        :this(name, new Dictionary<string, string> {{port, port}})
    {
        
    }
    
    public async Task<Container> TryRunAsContainer()
    {
        var client = new DockerClientConfiguration()
            .CreateClient();

        var createContainerParameters = new CreateContainerParameters()
        {
            Image = Name,
            HostConfig = new HostConfig
            {
                AutoRemove = true,
            }
        };

        foreach (var port in PortMap)
        {
            createContainerParameters.ExposedPorts.Add(port.Key, new EmptyStruct());
            createContainerParameters.HostConfig.PortBindings.Add(port.Key,
                new List<PortBinding>
                {
                    new() { HostPort = port.Value } 
                });
        }
        
        var response = await client.Containers.CreateContainerAsync(createContainerParameters);
        return new Container(response.ID, await 
            client.Containers.StartContainerAsync(response.ID, new ContainerStartParameters()));
    }
}

public record Container(string Id, bool Started)
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