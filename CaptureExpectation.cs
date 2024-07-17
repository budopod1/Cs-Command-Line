using System;

public class CaptureExpectation : Expectation {
    Action<string> action;
    bool optional;
    string help;

    public override string Matched {
        set => action(value);
        get => null;
    }

    public CaptureExpectation(Action<string> action, string help, bool optional=false) {
        this.action = action;
        this.optional = optional;
        this.help = help;
    }

    public override bool ShouldParseOption() {return false;}
    public override bool Matches(string word) {return true;}
    public override bool IsOptional() {return optional;}
    public override bool IsEmpty() {return false;}
    protected override string _GetHelp() {return $"<{help}>";}
}
