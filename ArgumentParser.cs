using System;
using System.Linq;
using System.Collections.Generic;

public class ArgumentParser {
    string cmdName;
    string description;
    LinkedList<Expectation> expected = new LinkedList<Expectation>();
    Dictionary<string, Action> options = new Dictionary<string, Action>();
    List<OptionHelp> optionHelp = new List<OptionHelp>();
    List<IEnumerable<WithCmdUsage>> usages = new List<IEnumerable<WithCmdUsage>>();
    bool exitOnProblem = true;

    public bool CurrentlyParseOptions = true;

    public int OptionUsagePadding = 45;

    public ArgumentParser(string cmdName, string description) {
        this.cmdName = cmdName;
        this.description = description;
        AddOption(ShowHelp, "Show this text and exit", null, "h", "help");
    }

    public static void DisplayProblem(string problem) {
        Console.WriteLine("Epsilon: "+problem);
        Console.WriteLine("Use '--help' to view usage");
        Environment.Exit(1);
    }

    void parsingProblem(string problem) {
        if (exitOnProblem) {
            DisplayProblem(problem);
        } else {
            throw new ParseProblemException(problem);
        }
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

    public T ExpectFirst<T>(T expectation) where T : Expectation {
        expected.AddFirst(expectation);
        return expectation;
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
        AddOption(() => ExpectFirst(expectation), help, expectation.GetHelp(), names);
        return expectation;
    }

    public void AddOption(Action action, string help, string expected, params string[] names) {
        optionHelp.Add(new OptionHelp(help, expected ?? "", names));
        foreach (string name in names) {
            options[name] = action;
        }
    }

    public void UseOption(string option) {
        if (!options.ContainsKey(option)) {
            parsingProblem($"Unknown option {JSONTools.ToLiteral(option)}");
        }
        options[option]();
    }

    public void Parse(string[] args) {
        bool positionalOnly = false;
        foreach (string arg in args) {
            if (arg == "--") {
                positionalOnly = true;
                continue;
            }

            Expectation expectation = null;
            if (expected.Count > 0) {
                expectation = expected.First.Value;
                expectation.Before();
            }
            
            if (!positionalOnly && CurrentlyParseOptions
                && arg.Length >= 2 && arg[0] == '-') {
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
                expectation = expected.First.Value;
                if (expected.Count == 0) {
                    parsingProblem($"To many parameters: {JSONTools.ToLiteral(arg)}");
                }
                expected.RemoveFirst();
                expectation.IsPresent = true;
                if (expectation.IsEmpty()) {
                    expectation.RunThens();
                    continue;
                }
                bool matches = expectation.Matches(arg);
                if (!matches) {
                    if (expectation.IsOptional()) continue;
                    parsingProblem($"Expected {expectation.GetHelp()}, found {arg}");
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
                continue;
            }
            parsingProblem("Expected another parameter: " + expectation.GetHelp());
        }
    }

    public void ParseAdditionalOptions(IEnumerable<string> options) {
        expected = new LinkedList<Expectation>();
        exitOnProblem = false;
        CurrentlyParseOptions = true;
        
        Parse(options.ToArray());
    }
}
