using System;

public class CmdUsagePart : WithCmdUsage {
    string usage;

    public CmdUsagePart(string usage) {
        this.usage = usage;
    }

    public override string GetHelp() {return usage;}
}
