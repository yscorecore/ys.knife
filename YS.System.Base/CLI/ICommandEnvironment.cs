namespace System.CLI
{
    public interface ICommandEnvironment
    {
        string CommandLine { get; }

        string[] GetCommandLineArgs();

        string Program { get; }
    }
}