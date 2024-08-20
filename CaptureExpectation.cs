using System;

public class CaptureExpectation(Action<string> action, string help, bool optional = false) : Expectation {
    readonly Action<string> action = action;
    readonly bool optional = optional;
    readonly string help = help;

    public override string Matched {
        set => action(value);
        get => null;
    }

    public override bool ShouldParseOption() {return false;}
    public override bool Matches(string word) {return true;}
    public override bool IsOptional() {return optional;}
    public override bool IsEmpty() {return false;}
    protected override string _GetHelp() {return $"<{help}>";}
}
