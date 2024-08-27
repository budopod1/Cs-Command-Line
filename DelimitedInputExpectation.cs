public class DelimitedInputExpectation : Expectation {
    public List<string> MatchedSegments = [];
    public override string Matched {
        set => MatchedSegments.Add(value);
        get => MatchedSegments.Last();
    }
    readonly ArgumentParser parser;
    readonly bool isOptional;
    bool needsNext;
    readonly string help;
    readonly string delimiter;

    public DelimitedInputExpectation(ArgumentParser parser, string help, string delimiter, bool needsOne=false) {
        this.parser = parser;
        isOptional = !needsOne;
        needsNext = needsOne;
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
