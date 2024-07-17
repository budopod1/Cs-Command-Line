using System;

public class OptionHelp {
    public string Help;
    public string Expected;
    public string[] Names;

    public OptionHelp(string help, string expected, string[] names) {
        Help = help;
        Expected = expected;
        Names = names;
    }
}
