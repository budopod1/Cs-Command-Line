using System;

public class InputExpectation : Expectation {
    bool optional;
    string help;

    public InputExpectation(string help, bool optional=false) {
        this.optional = optional;
        this.help = help;
    }
    
    public override bool Matches(string word) {return true;}
    public override bool IsOptional() {return optional;}
    public override bool IsEmpty() {return false;}
    protected override string _GetHelp() {return $"<{help}>";}
}
