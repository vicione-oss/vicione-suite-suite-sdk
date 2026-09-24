using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Testing", Scope = "namespaceanddescendants", Target = "~N:TestModule.Backend")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Contract members mirror the reference database's column names", Scope = "namespaceanddescendants", Target = "~N:TestModule.Backend.Contracts")]
[assembly: SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Contract members mirror the reference database's column names", Scope = "namespaceanddescendants", Target = "~N:TestModule.Backend.Contracts")]
