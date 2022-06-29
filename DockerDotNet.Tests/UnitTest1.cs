using Amazon.S3;
using Amazon.S3.Model;

namespace DockerDotNet.Tests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        var localstack = await LocalStack.StartAsync();
        
        var client = new AmazonS3Client(new AmazonS3Config
        {
            UseHttp = true,
            ForcePathStyle = true,
            ServiceURL = "http://localhost:4566",
        });
        
        var response = await client.PutBucketAsync(new PutBucketRequest { BucketName = "test" });
        var bucketList = await client.ListBucketsAsync();
        

        await localstack.StopAsync();
    }

    private static class LocalStack
    {
        public static async Task<Container> StartAsync()
        {
            var image = new Image("localstack/localstack", "4566");
            return await image.TryRunAsContainer();
        }
    }
}