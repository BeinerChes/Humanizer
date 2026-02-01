using System.Collections.Immutable;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Humanizer.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NamespaceMigrationCodeFixProvider)), Shared]
public class NamespaceMigrationCodeFixProvider : CodeFixProvider
{
    private const string Title = "Update to Humanizer namespace";

    private static readonly ImmutableHashSet<string> OldNamespaces = ImmutableHashSet.Create(
        StringComparer.Ordinal,
        "Humanizer.Localisation.CollectionFormatters",
        "Humanizer.Localisation.TimeToClockNotation",
        "Humanizer.Localisation.DateToOrdinalWords",
        "Humanizer.Localisation.NumberToWords",
        "Humanizer.Localisation.Formatters",
        "Humanizer.Localisation.Ordinalizers",
        "Humanizer.DateTimeHumanizeStrategy",
        "Humanizer.Configuration",
        "Humanizer.Localisation",
        "Humanizer.Inflections",
        "Humanizer.Bytes");

    public sealed override ImmutableArray<string> FixableDiagnosticIds => [NamespaceMigrationAnalyzer.DiagnosticId];

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
            return;

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;
        var node = root.FindNode(diagnosticSpan);

        var usingDirective = node.FirstAncestorOrSelf<UsingDirectiveSyntax>();
        if (usingDirective is not null)
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => ReplaceUsingDirectiveAsync(context.Document, usingDirective, c),
                    equivalenceKey: Title),
                diagnostic);
            return;
        }

        if (node is QualifiedNameSyntax qualifiedName)
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => ReplaceQualifiedNameAsync(context.Document, qualifiedName, c),
                    equivalenceKey: Title),
                diagnostic);
        }
    }

    private static async Task<Document> ReplaceUsingDirectiveAsync(
        Document document,
        UsingDirectiveSyntax usingDirective,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
            return document;

        var newName = SyntaxFactory.IdentifierName("Humanizer");
        var newUsingDirective = usingDirective.WithName(newName);

        var updatedRoot = root.ReplaceNode(usingDirective, newUsingDirective);
        return document.WithSyntaxRoot(updatedRoot);
    }

    private static async Task<Document> ReplaceQualifiedNameAsync(Document document, QualifiedNameSyntax qualifiedName, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
            return document;

        var fullName = qualifiedName.ToString();
        string? matchedNamespace = null;

        foreach (var ns in OldNamespaces)
        {
            if (IsNamespaceMatch(fullName, ns))
            {
                matchedNamespace = ns;
                break;
            }
        }

        if (matchedNamespace == null)
            return document;

        var newNameText = GetReplacementName(fullName, matchedNamespace);

        var newQualifiedName = SyntaxFactory.ParseName(newNameText)
            .WithLeadingTrivia(qualifiedName.GetLeadingTrivia())
            .WithTrailingTrivia(qualifiedName.GetTrailingTrivia());

        var updatedRoot = root.ReplaceNode(qualifiedName, newQualifiedName);
        return document.WithSyntaxRoot(updatedRoot);
    }

    private static bool IsNamespaceMatch(string fullName, string oldNamespace)
    {
        if (fullName.Length == oldNamespace.Length)
            return string.Equals(fullName, oldNamespace, StringComparison.Ordinal);

        return fullName.Length > oldNamespace.Length
            && fullName[oldNamespace.Length] == '.'
            && fullName.StartsWith(oldNamespace, StringComparison.Ordinal);
    }

    private static string GetReplacementName(string fullName, string matchedNamespace)
    {
        if (fullName.Length == matchedNamespace.Length)
            return "Humanizer";

        var startIndex = matchedNamespace.Length + 1;

        if (startIndex >= fullName.Length)
            return "Humanizer";

        var remainder = fullName.Substring(startIndex);
        return "Humanizer." + remainder;
    }
}
