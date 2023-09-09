using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace DollarTemplateAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DollarTemplateAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "RP0001";

    private static readonly LocalizableString Title = "Interpolated string ends with '$'";

    private static readonly LocalizableString MessageFormat = "Interpolated string ends with '$'";

    private const string Category = "Usage";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Rule);

    public override void Initialize(
        AnalysisContext context
    )
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterOperationAction(AnalyzeOperation, OperationKind.InterpolatedString);
    }

    private void AnalyzeOperation(
        OperationAnalysisContext context
    )
    {
        if (context.Operation is not IInterpolatedStringOperation invocationOperation)
            return;

        var children = invocationOperation.ChildOperations.ToArray();

        // Show for all but for the end of the string interpolation. 
        for (var i = 0; i < children.Length-1; i++)
        {
            var child = children[i];
            
            if (child is not IInterpolatedStringTextOperation childOp) continue;

            if (!childOp.Text.ConstantValue.Value!.ToString().EndsWith("$")) continue;
            
            var diagnostic = Diagnostic.Create(
                Rule,
                 childOp.Syntax.GetLocation()
            );
            context.ReportDiagnostic(diagnostic);
        }
    }
}