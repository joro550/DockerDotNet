using Amazon.S3;
using Amazon.S3.Model;

namespace DockerDotNet.Tests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        var image = new Image("localstack/localstack");
        var container = await image.TryRunAsContainer();
        
        var client = new AmazonS3Client(new AmazonS3Config
        {
            UseHttp = true,
            ForcePathStyle = true,
            ServiceURL = "http://localhost:4566",
        });
        
        var response = await client.PutBucketAsync(new PutBucketRequest { BucketName = "test" });
        var bucketList = await client.ListBucketsAsync();
        

        await container.StopAsync();
    }
}