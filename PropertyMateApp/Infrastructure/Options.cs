namespace PropertyMateApp.Infrastructure
{
    using CommandLine;
    using CommandLine.Text;
    using JetBrains.Annotations;
    using Serilog.Events;

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal sealed class Options
    {
        [Option('v', "verbosity", Required = false, DefaultValue = LogEventLevel.Information, HelpText = "Logging verbosity level.")]
        public LogEventLevel LogEventLevel { get; set; }

        [Option('d', "data", Required = false, HelpText = "JSON data file path.")]
        public string DataPath { get; set; }

        [Option('b', "base", Required = false, HelpText = "Base server url.")]
        public string BaseServerUrl { get; set; }

        [Option('s', "seq", Required = false, HelpText = "Logging (seq) server url.")]
        public string LoggingServerUrl { get; set; }

        [Option('t', "target", Required = false, HelpText = "Target (elastic) server url.")]
        public string TargetServerUrl { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage() => HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
    }
}