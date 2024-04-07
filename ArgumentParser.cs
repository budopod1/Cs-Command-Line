using System;
using System.Linq;
using System.Collections.Generic;

public class ArgumentParser {
    string cmdName;
    string description;
    LinkedList<Expectation> expected = new LinkedList<Expectation>();
    Dictionary<string, Expectation> options = new Dictionary<string, Expectation>();
    List<OptionHelp> optionHelp = new List<OptionHelp>();
    List<IEnumerable<WithCmdUsage>> usages = new List<IEnumerable<WithCmdUsage>>();

    public int OptionUsagePadding = 45;

    public ArgumentParser(string cmdName, string description) {
        this.cmdName = cmdName;
        this.description = description;
        AddOption(ShowHelp, "Show this text and exit", "h", "help");
    }

    public void DisplayProblem(string problem) {
        Console.WriteLine("Epsilon: "+problem);
        Console.WriteLine("Use '--help' to view usage");
        Environment.Exit(1);
    }

    public void AddUsageOption(IEnumerable<WithCmdUsage> usage) {
        usages.Add(usage);
    }

    public void AddUsageOption(params WithCmdUsage[] usage) {
        usages.Add(usage);
    }

    public void AddDefaultUsage() {
        usages.Add(expected);
    }

    void ShowUsageHelp() {
        Console.WriteLine("Usage:");
        foreach (IEnumerable<WithCmdUsage> usage in usages) {
            Console.Write(cmdName + " ");
            if (options.Count > 0) Console.Write("[options] ");
            foreach (WithCmdUsage wusage in usage) {
                string help = wusage.GetHelp();
                if (help.Length == 0) continue;
                Console.Write(help + " ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    void ShowOptionHelp() {
        Console.WriteLine("Options:");
        foreach (OptionHelp help in optionHelp) {
            bool first = true;
            string optionUsage = "";
            foreach (string name in help.Names) {
                if (!first) optionUsage += ", ";
                first = false;
                if (name.Length == 1) {
                    optionUsage += "-"+name;
                } else {
                    optionUsage += "--"+name;
                }
            }
            if (help.Expected != "") optionUsage += " "+help.Expected;
            Console.Write(optionUsage.PadRight(OptionUsagePadding));
            Console.Write(" " + help.Help);
            Console.WriteLine();
        }
    }
    
    public void ShowHelp() {
        Console.WriteLine(description);
        Console.WriteLine();
        if (usages.Count > 0) {
            ShowUsageHelp();
        }
        if (optionHelp.Count > 0) {
            ShowOptionHelp();
        }
        Environment.Exit(0);
    }

    public T Expect<T>(T expectation) where T : Expectation {
        expected.AddLast(expectation);
        return expectation;
    }

    public void Expect(params Expectation[] expectations) {
        foreach (Expectation expectation in expectations) {
            expected.AddLast(expectation);
        }
    }

    public T AddOption<T>(T expectation, string help, params string[] names) where T : Expectation {
        optionHelp.Add(new OptionHelp(help, expectation.GetHelp(), names));
        foreach (string name in names) {
            options[name] = expectation;
        }
        return expectation;
    }

    public ActionExpectation AddOption(Action action, string help, params string[] names) {
        optionHelp.Add(new OptionHelp(help, "", names));
        ActionExpectation expectation = new ActionExpectation(action);
        foreach (string name in names) {
            options[name] = expectation;
        }
        return expectation;
    }

    public void UseOption(string option) {
        if (!options.ContainsKey(option)) {
            DisplayProblem($"Unknown option {JSONTools.ToLiteral(option)}");
        }
        expected.AddFirst(options[option]);
    }

    public void Parse(string[] args) {
        bool positionalOnly = false;
        foreach (string arg in args) {
            if (arg == "--") {
                positionalOnly = true;
                continue;
            }
            
            if (!positionalOnly && arg.Length >= 2 && arg[0] == '-') {
                if (arg[1] == '-') {
                    string option = arg.Substring(2);
                    UseOption(option);
                } else {
                    string sliced = arg.Substring(1);
                    foreach (char chr in sliced) {
                        string option = chr.ToString();
                        UseOption(option);
                    }
                }
                continue;
            }

            while (true) {
                if (expected.Count == 0) {
                    DisplayProblem($"To many parameters: {JSONTools.ToLiteral(arg)}");
                }
                Expectation expectation = expected.First.Value;
                expected.RemoveFirst();
                expectation.IsPresent = true;
                if (expectation.IsEmpty()) {
                    expectation.RunThens();
                    continue;
                }
                bool matches = expectation.Matches(arg);
                if (!matches) {
                    if (expectation.IsOptional()) continue;
                    DisplayProblem("Expected " + expectation.GetHelp());
                }
                expectation.Matched = arg;
                expectation.RunThens();
                break;
            }
        }

        while (expected.Count > 0) {
            Expectation expectation = expected.First.Value;
            expected.RemoveFirst();
            if (expectation.IsEmpty() || expectation.IsOptional()) {
                expectation.IsPresent = true;
                expectation.RunThens();
                continue;
            }
            DisplayProblem("Expected " + expectation.GetHelp());
        }
    }
}
