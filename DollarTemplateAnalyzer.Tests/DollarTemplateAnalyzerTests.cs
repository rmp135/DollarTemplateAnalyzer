using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
        DollarTemplateAnalyzer.DollarTemplateAnalyzer>;

namespace DollarTemplateAnalyzer.Tests;

public class DollarTemplateAnalyzerTests
{
    [Fact]
    public async Task DollarFoundEndOfStringTemplate_AlertDiagnostic()
    {
        const string text = @"
public class Program
{
    public void Main()
    {
        var test = 0;
        var text = $""Hello${test}"";
    }
}
";

        var expected = Verifier.Diagnostic()
            .WithLocation(7, 22);
        await Verifier.VerifyAnalyzerAsync(text, expected);
    }
    
    [Fact]
    public async Task DollarFoundEndOfStringTemplate_MultipleTimes_AlertDiagnostic()
    {
        const string text = @"
public class Program
{
    public void Main()
    {
        var test = 0;
        var text = $""Hello${test}another${test}"";
    }
}
";

        var expected = Verifier.Diagnostic()
            .WithLocation(7, 22);
        var expected2 = Verifier.Diagnostic()
            .WithLocation(7, 34);
        await Verifier.VerifyAnalyzerAsync(text, expected, expected2);
    }
    
    [Fact]
    public async Task DollarFoundAtEndOfString_NoAlertDiagnostic()
    {
        const string text = @"
public class Program
{
    public void Main()
    {
        var test = 0;
        var text = $""Hel$lo{test}ano$ther{test}test$"";
    }
}
";

        await Verifier.VerifyAnalyzerAsync(text, DiagnosticResult.EmptyDiagnosticResults);
    }
}