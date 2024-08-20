using System;

public class InputExpectation(string help, bool optional = false) : Expectation {
    readonly bool optional = optional;
    readonly string help = help;

    public override bool Matches(string word) {return true;}
    public override bool IsOptional() {return optional;}
    public override bool IsEmpty() {return false;}
    protected override string _GetHelp() {return $"<{help}>";}
}
