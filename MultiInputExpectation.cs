using System;
using System.Linq;
using System.Collections.Generic;

public class MultiInputExpectation : Expectation {
    public List<string> MatchedSegments = new List<string>();
    public override string Matched { 
        set => MatchedSegments.Add(value); 
        get => MatchedSegments.Last();
    }
    bool isOptional;
    bool needsNext;
    string help;

    public MultiInputExpectation(ArgumentParser parser, string help, bool needsOne=false) {
        this.isOptional = !needsOne;
        this.needsNext = needsOne;
        this.help = help;
        Then(() => {
            needsNext = false;
            parser.ExpectFirst(this);
        });
    }

    public override bool Matches(string word) {return true;}
    public override bool IsOptional() {return !needsNext;}
    public override bool IsEmpty() {return false;}
    public override string GetHelp() {
        return $"{(isOptional ? $"[{help}]" : help)}...";
    }
}
