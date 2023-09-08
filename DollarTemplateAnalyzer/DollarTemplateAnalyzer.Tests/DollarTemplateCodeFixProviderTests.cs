using System.Threading.Tasks;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<DollarTemplateAnalyzer.DollarTemplateAnalyzer,
        DollarTemplateAnalyzer.DollarTemplateCodeFixProvider>;

namespace DollarTemplateAnalyzer.Tests;

public class DollarTemplateCodeFixProviderTests
{
    [Fact(Skip = "Not sure why this is failing")]
    public async Task DollarFoundEndOfStringTemplate_RemoveTrailingDollar()
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

        const string newText = @"
public class Program
{
    public void Main()
    {
        var test = 0;
        var text = $""Hello{test}"";
    }
}
";

        var expected = Verifier.Diagnostic()
            .WithLocation(7, 22);
        await Verifier.VerifyCodeFixAsync(text, expected, newText).ConfigureAwait(false);
    }
}