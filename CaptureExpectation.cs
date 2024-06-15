using System;

public class CaptureExpectation : Expectation {
    Action<string> action;
    bool optional;
    string help;
    ArgumentParser parser;
    
    public override string Matched { 
        set => action(value); 
        get => null;
    }

    public CaptureExpectation(ArgumentParser parser, Action<string> action, string help, bool optional=false) {
        this.parser = parser;
        this.action = action;
        this.optional = optional;
        this.help = help;
        Then(() => {
            parser.CurrentlyParseOptions = true;
        });
    }

    public override void Before() {
        parser.CurrentlyParseOptions = false;
    }
    public override bool Matches(string word) {return true;}
    public override bool IsOptional() {return optional;}
    public override bool IsEmpty() {return false;}
    protected override string _GetHelp() {return $"<{help}>";}
}
