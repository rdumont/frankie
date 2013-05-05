using RDumont.Frankie.Core;

namespace RDumont.Frankie.CommandLine.Commands
{
    public class RunCommand : RunnableAbstractCommand<BaseOptions>
    {
        public override string Name
        {
            get { return "run"; }
        }

        public RunCommand() : this(new Generator(SiteContext.Current))
        {
        }

        public RunCommand(Generator generator) : base(generator)
        {
        }

        public override void ExecuteCommand(BaseOptions options)
        {
            Logger.Current.Log(LoggingLevel.Minimal, "Running Frankie...");
            Logger.Current.Log(LoggingLevel.Minimal, "Source: {0}", options.LocationPath);
            Logger.Current.Log(LoggingLevel.Minimal, "Target: {0}\n", options.OutputPath);

            var sw = System.Diagnostics.Stopwatch.StartNew();
            
            RunTransformation(options);
            
            sw.Stop();

            Logger.Current.Log(LoggingLevel.Minimal, "\nFINISHED! Took {0}ms", sw.ElapsedMilliseconds);
        }
    }
}
