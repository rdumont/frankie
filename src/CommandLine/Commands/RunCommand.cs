using RDumont.Frankie.Core;

namespace RDumont.Frankie.CommandLine.Commands
{
    public class RunCommand : RunnableAbstractCommand<BaseOptions>
    {
        public override string Name
        {
            get { return "run"; }
        }

        public override void ExecuteCommand(BaseOptions options)
        {
            Logger.Current.Log(LoggingLevel.Minimal, "Running Frankie...");
            Logger.Current.Log(LoggingLevel.Minimal, "Source: {0}", options.SourcePath);
            Logger.Current.Log(LoggingLevel.Minimal, "Target: {0}\n", options.OutputPath);

            Profile(" ==> Total", () =>
                RunTransformation(options));

            Logger.Current.Log(LoggingLevel.Minimal, "\nFinished!");
        }
    }
}
