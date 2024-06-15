using System;
using System.Collections.Generic;

public abstract class Expectation : WithCmdUsage {
    public virtual string Matched { get; set; }
    public bool IsPresent = false;
    List<Action> thens = new List<Action>();

    public virtual bool ShouldParseOption() {return true;}
    public virtual bool Matches(string word) {return false;}
    public abstract bool IsOptional();
    public abstract bool IsEmpty();
    
    protected virtual string _GetHelp() {return "";}
    
    public override string GetHelp() {
        string help = _GetHelp();
        if (help == "") return "";
        if (IsOptional()) help = $"[{help}]";
        return help;
    }

    public Expectation Then(Action action) {
        thens.Add(action);
        return this;
    }

    public void RunThens() {
        foreach (Action action in thens) {
            action();
        }
    }
}
