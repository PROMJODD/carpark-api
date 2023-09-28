using Moq;
using Xunit;
using Prom.LPR.Api.ExternalServices.ObjectStorage;

namespace Prom.LPR.Test.Api.ExternalServices.ObjectStorage;

public class GoogleCloudStorageTest
{
    [Theory]
    [InlineData("abc.jpg", "default", "bucket", "abcde/0001", "gs://bucket/default/abcde/0001/abc.jpg")]
    [InlineData("xxxxxabc.jpg", "default", "bucket", "abcde/0001", "gs://bucket/default/abcde/0001/xxxxxabc.jpg")]
    [InlineData("xxxxxabc.jpg", "global1", "bucket", "xxx/abcde/0001", "gs://bucket/global1/xxx/abcde/0001/xxxxxabc.jpg")]
    public void UploadFileNoSignerTest(string localPath, string org, string bucket, string folder, string expectedResult)
    {
        File.Create(localPath).Dispose();
    
        var sc = new Mock<IGcsClient>();
        var dat = new Google.Apis.Storage.v1.Data.Object();
        sc.Setup(x => x.UploadObject(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>())).Returns(dat);

        var gcs = new GoogleCloudStorage();
        gcs.SetStorageClient(sc.Object);

        var m = gcs.UploadFile(localPath, org, bucket, folder);

        File.Delete(localPath);

        Assert.Equal(expectedResult, m.StoragePath);
    }

    [Theory]
    [InlineData("abc.jpg", "default", "bucket", "abcde/0001", "gs://bucket/default/abcde/0001/abc.jpg")]
    [InlineData("xxxxxabc.jpg", "default", "bucket", "abcde/0001", "gs://bucket/default/abcde/0001/xxxxxabc.jpg")]
    [InlineData("xxxxxabc.jpg", "global1", "bucket", "xxx/abcde/0001", "gs://bucket/global1/xxx/abcde/0001/xxxxxabc.jpg")]
    public void UploadFileWithSignerTest(string localPath, string org, string bucket, string folder, string expectedResult)
    {
        File.Create(localPath).Dispose();
    
        var sc = new Mock<IGcsClient>();
        var dat = new Google.Apis.Storage.v1.Data.Object();
        sc.Setup(x => x.UploadObject(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>())).Returns(dat);

        var mckSn = new Mock<IGcsSigner>();
        mckSn.Setup(x => x.Sign(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<HttpMethod>())).Returns("presigned_url");

        var gcs = new GoogleCloudStorage();
        gcs.SetStorageClient(sc.Object);
        gcs.SetUrlSigner(mckSn.Object);

        var m = gcs.UploadFile(localPath, org, bucket, folder);

        File.Delete(localPath);

        Assert.Equal(expectedResult, m.StoragePath);
        Assert.Equal("presigned_url", m.PreSignedUrl);
    }
}
