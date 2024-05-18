using System;
using System.Linq;
using System.Collections.Generic;

public class PossibilitiesExpectation : Expectation {
    string default_;
    List<string> options;

    public PossibilitiesExpectation(params string[] options) {
        default_ = options[0];
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
        return "<"+String.Join(" | ", options)+">";
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
