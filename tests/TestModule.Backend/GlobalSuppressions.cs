// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Testing", Scope = "namespaceanddescendants", Target = "~N:TestModule.Backend")]
[assembly: SuppressMessage("Naming", "CA1707:Bezeichner dürfen keine Unterstriche enthalten", Justification = "<Ausstehend>", Scope = "namespaceanddescendants", Target = "~N:TestModule.Backend.Contracts")]
[assembly: SuppressMessage("Style", "IDE1006:Benennungsstile", Justification = "<Ausstehend>", Scope = "namespaceanddescendants", Target = "~N:TestModule.Backend.Contracts")]
