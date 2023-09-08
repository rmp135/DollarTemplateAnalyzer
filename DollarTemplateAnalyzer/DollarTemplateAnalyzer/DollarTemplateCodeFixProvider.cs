using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxFactory = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using SyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

namespace DollarTemplateAnalyzer;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DollarTemplateCodeFixProvider)), Shared]
public class DollarTemplateCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(DollarTemplateAnalyzer.DiagnosticId);

    public override FixAllProvider? GetFixAllProvider() => null;

    public sealed override async Task RegisterCodeFixesAsync(
        CodeFixContext context
    )
    {
        var diagnostic = context.Diagnostics.Single();
        var diagnosticSpan = diagnostic.Location.SourceSpan;
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnosticNode = root?.FindNode(diagnosticSpan);

        if (diagnosticNode is not InterpolatedStringContentSyntax declaration)
            return;

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Remove dollar sign from template",
                createChangedDocument: c => CreateChangedDocument(context.Document, declaration, c),
                equivalenceKey: "Remove dollar sign from template"
            ),
            diagnostic
        );
    }

    private async Task<Document> CreateChangedDocument(
        Document contextDocument,
        CSharpSyntaxNode diagnosticNode,
        CancellationToken arg
    )
    {
        var oldRoot = await contextDocument.GetSyntaxRootAsync(arg).ConfigureAwait(false);
        var token = diagnosticNode.GetFirstToken();
        var newToken = SyntaxFactory.Token(
            token.LeadingTrivia,
            SyntaxKind.InterpolatedStringTextToken, 
            token.ValueText.TrimEnd('$'),
            token.ValueText,
            token.TrailingTrivia
        );
        var newRoot =  oldRoot!.ReplaceToken(token, newToken);
        return contextDocument.WithSyntaxRoot(newRoot);
    }
}