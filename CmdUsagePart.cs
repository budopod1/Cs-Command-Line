namespace CsCommandLine;
public class CmdUsagePart(string usage) : WithCmdUsage {
    readonly string usage = usage;

    public override string GetHelp() {return usage;}
}
