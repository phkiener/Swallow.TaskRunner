using Swallow.TaskRunner.Commands;
using Swallow.TaskRunner.Serialization;

namespace Swallow.TaskRunner.Test.Commands;

public sealed class CreateManifestTest
{
    private readonly CreateManifest command = new();

    [Test]
    public async Task CreatesNewManifest()
    {
        using var context = TestCommandContext.Create();
        await command.RunAsync(context, []);

        var expectedFile = Path.Combine(context.CurrentDirectory, ".config", "dotnet-tasks.json");
        await Assert.That(context.WrittenOutput).Contains(expectedFile);
        await Assert.That(context.WrittenOutput).Contains("Created new task manifest");

        await using var fileStream = File.OpenRead(expectedFile);
        var manifest = ManifestReader.ReadAsync(fileStream, context.CancellationToken);

        Assert.NotNull(manifest);
    }

    [Test]
    public async Task RunningTwice_WorksWithoutExceptions()
    {
        using var context = TestCommandContext.Create();
        await command.RunAsync(context, []);
        await command.RunAsync(context, []);

        await Assert.That(context.WrittenOutput).Contains("Task manifest already exists");
    }
}
