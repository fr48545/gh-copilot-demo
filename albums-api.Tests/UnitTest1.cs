using System.Text;
using UnsecureApp.Controllers;

namespace albums_api.Tests;

public sealed class MockFileStreamFactory : IFileStreamFactory
{
    private readonly byte[] content;

    public string? OpenedPath { get; private set; }
    public int OpenReadCallCount { get; private set; }

    public MockFileStreamFactory(string content)
    {
        this.content = Encoding.UTF8.GetBytes(content);
    }

    public Stream OpenRead(string path)
    {
        OpenedPath = path;
        OpenReadCallCount++;

        return new MemoryStream(content, writable: false);
    }
}

[TestFixture]
public class MyControllerTests
{
    [Test]
    public void ReadFile_ReturnsMockedFileContents()
    {
        var fileStreamFactory = new MockFileStreamFactory("album content");
        var controller = new MyController(fileStreamFactory);

        string result = controller.ReadFile("mocked-path");

        Assert.That(result, Is.EqualTo("album content"));
        Assert.That(fileStreamFactory.OpenedPath, Is.EqualTo("mocked-path"));
        Assert.That(fileStreamFactory.OpenReadCallCount, Is.EqualTo(1));
    }

    [Test]
    public void GetObject_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => new MyController().GetObject());
    }
}