public class KeywordExpectation(string keyword, bool isOptional = false) : Expectation {
    readonly string keyword = keyword;
    readonly bool isOptional = isOptional;

    public override bool Matches(string word) {return keyword == word;}
    public override bool IsOptional() {return isOptional;}
    public override bool IsEmpty() {return false;}
    protected override string _GetHelp() {return keyword;}
}
