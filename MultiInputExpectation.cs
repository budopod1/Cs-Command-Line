namespace CsCommandLine;
public class MultiInputExpectation : Expectation {
    public List<string> MatchedSegments = [];
    public override string Matched {
        set => MatchedSegments.Add(value);
        get => MatchedSegments.Last();
    }
    readonly bool isOptional;
    bool needsNext;
    readonly string help;

    public MultiInputExpectation(ArgumentParser parser, string help, bool needsOne=false) {
        isOptional = !needsOne;
        needsNext = needsOne;
        this.help = help;
        Then(() => {
            needsNext = false;
            parser.ExpectFirst(this);
        });
    }

    public override bool ShouldParseOption() {return false;}
    public override bool Matches(string word) {return true;}
    public override bool IsOptional() {return !needsNext;}
    public override bool IsEmpty() {return false;}

    public override string GetHelp() {
        string wrapped = $"<{help}>";
        if (isOptional) wrapped = $"[{wrapped}]";
        return $"{wrapped}...";
    }
}
