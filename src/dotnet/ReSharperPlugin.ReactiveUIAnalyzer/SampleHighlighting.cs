using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using ReSharperPlugin.ReactiveUIAnalyzer;

namespace ReSharperPlugin.ReactiveUIAnalyzer
{
    [RegisterConfigurableSeverity(
        SampleHighlighting.SeverityId,
        CompoundItemName: null,
        Group: HighlightingGroupIds.CodeSmell,
        Title: Message,
        Description: Description,
        DefaultSeverity: Severity.WARNING)]
    [ConfigurableSeverityHighlighting(
        SeverityId,
        CSharpLanguage.Name,
        OverlapResolve = OverlapResolveKind.ERROR,
        OverloadResolvePriority = 0,
        ToolTipFormatString = Message)]
    public class SampleHighlighting : IHighlighting
    {
        public ILambdaExpression LambdaExpression { get; }
        public const string SeverityId = nameof(SampleHighlighting);
        public const string Message = "Sample highlighting message";
        public const string Description = "Sample highlighting description";
        
        public SampleHighlighting(ILambdaExpression lambdaExpression)
        {
            LambdaExpression = lambdaExpression;
        }

        public bool IsValid()
        {
            return LambdaExpression.IsValid();
        }

        public DocumentRange CalculateRange()
        {
            return LambdaExpression.BodyExpression.GetNavigationRange();
        }

        public string ToolTip => Message;
        
        public string ErrorStripeToolTip
            => $"Sample highlighting error";
    }
}
