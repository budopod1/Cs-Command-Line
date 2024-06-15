using System;
using System.Linq;
using System.Collections.Generic;

public class DelimitedInputExpectation : Expectation {
    public List<string> MatchedSegments = new List<string>();
    public override string Matched { 
        set => MatchedSegments.Add(value); 
        get => MatchedSegments.Last();
    }
    ArgumentParser parser;
    bool isOptional;
    bool needsNext;
    string help;
    string delimiter;

    public DelimitedInputExpectation(ArgumentParser parser, string help, string delimiter, bool needsOne=false) {
        this.parser = parser;
        this.isOptional = !needsOne;
        this.needsNext = needsOne;
        this.help = help;
        this.delimiter = delimiter;
        Then(() => {
            needsNext = false;
            if (MatchedSegments.Last() == delimiter) {
                MatchedSegments.RemoveAt(MatchedSegments.Count - 1);
            } else {
                parser.ExpectFirst(this);
            }
        });
    }

    public override bool ShouldParseOption() {return false;}
    public override bool Matches(string word) {return true;}
    public override bool IsOptional() {return !needsNext;}
    public override bool IsEmpty() {return false;}
    
    public override string GetHelp() {
        string wrapped = $"<{help}>";
        if (isOptional) wrapped = $"[{wrapped}]";
        return $"{wrapped}... {delimiter}";
    }
}
