namespace CsCommandLine;
public class PossibilitiesExpectation : Expectation {
    readonly string default_;
    readonly List<string> options;

    public PossibilitiesExpectation(string default_, params string[] options) {
        this.default_ = default_;
        this.options = options.ToList();
    }

    public PossibilitiesExpectation(int default_, Type type) {
        options = Enum.GetNames(type).ToList();
        int i = 0;
        foreach (int val in Enum.GetValues(type)) {
            if (val == default_) this.default_ = options[i];
            i++;
        }
    }

    public override bool Matches(string word) {return options.Contains(word);}
    public override bool IsOptional() {return false;}
    public override bool IsEmpty() {return false;}
    protected override string _GetHelp() {
        return "<"+ string.Join(" | ", options)+">";
    }

    public CmdUsagePart Usage(string possibility) {
        return new CmdUsagePart(possibility);
    }

    public string Value() {
        if (Matched == null || Matched == "") return default_;
        return Matched;
    }

    public T ToEnum<T>() where T : struct {
        Enum.TryParse(Value(), out T result);
        return result;
    }
}
