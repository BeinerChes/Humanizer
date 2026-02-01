using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Humanizer.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NamespaceMigrationAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "HUMANIZER001";
    private const string Category = "Usage";

    private static readonly LocalizableString Title = "Old Humanizer namespace usage";
    private static readonly LocalizableString MessageFormat = "The namespace '{0}' has been consolidated into 'Humanizer' in v3. Update your using directive.";
    private static readonly LocalizableString Description = "Humanizer v3 consolidates sub-namespaces into the root Humanizer namespace. This using directive should be updated to 'using Humanizer;'.";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description);

    private static readonly ImmutableHashSet<string> OldNamespaces = ImmutableHashSet.Create(
        StringComparer.Ordinal,
        "Humanizer.Bytes",
        "Humanizer.Localisation",
        "Humanizer.Localisation.Formatters",
        "Humanizer.Localisation.NumberToWords",
        "Humanizer.DateTimeHumanizeStrategy",
        "Humanizer.Configuration",
        "Humanizer.Localisation.DateToOrdinalWords",
        "Humanizer.Localisation.Ordinalizers",
        "Humanizer.Inflections",
        "Humanizer.Localisation.CollectionFormatters",
        "Humanizer.Localisation.TimeToClockNotation");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeUsingDirective, SyntaxKind.UsingDirective);
        context.RegisterSyntaxNodeAction(AnalyzeQualifiedName, SyntaxKind.QualifiedName);
    }

    private static void AnalyzeUsingDirective(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not UsingDirectiveSyntax usingDirective)
            return;

        if (usingDirective.Name is null)
            return;

        var namespaceName = usingDirective.Name.ToString();

        if (OldNamespaces.Contains(namespaceName))
        {
            var diagnostic = Diagnostic.Create(Rule, usingDirective.Name.GetLocation(), namespaceName);
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static void AnalyzeQualifiedName(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not QualifiedNameSyntax qualifiedName)
            return;

        if (qualifiedName.Ancestors().OfType<UsingDirectiveSyntax>().Any())
            return;

        if (qualifiedName.Parent is QualifiedNameSyntax parentQualified)
        {
            var parentName = parentQualified.ToString();
            if (HasMatchingNamespace(parentName))
                return;
        }

        var fullName = qualifiedName.ToString();

        var matchingNamespace = FindMatchingNamespace(fullName);
        if (matchingNamespace != null)
        {
            var diagnostic = Diagnostic.Create(Rule, qualifiedName.GetLocation(), matchingNamespace);
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static string? FindMatchingNamespace(string fullName)
    {
        foreach (var ns in OldNamespaces)
        {
            if (IsNamespaceMatch(fullName, ns))
                return ns;
        }
        return null;
    }

    private static bool IsNamespaceMatch(string fullName, string oldNamespace)
    {
        if (fullName.Length == oldNamespace.Length)
            return string.Equals(fullName, oldNamespace, StringComparison.Ordinal);

        return fullName.Length > oldNamespace.Length
            && fullName[oldNamespace.Length] == '.'
            && fullName.StartsWith(oldNamespace, StringComparison.Ordinal);
    }

    private static bool HasMatchingNamespace(string namespaceName)
    {
        foreach (var ns in OldNamespaces)
        {
            if (IsNamespaceMatch(namespaceName, ns))
                return true;
        }
        return false;
    }
}
