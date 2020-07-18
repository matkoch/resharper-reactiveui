using System;
using System.Linq;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace ReSharperPlugin.ReactiveUIAnalyzer
{
    // Types mentioned in this attribute are used for performance optimizations
    [ElementProblemAnalyzer(
        typeof (IInvocationExpression),
        HighlightingTypes = new [] {typeof (SampleHighlighting)})]
    public class ReactiveUIClosureAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
    {
        protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            /*
                this.WhenAnyValue(
                        x => x.Toggle,
                        x => _settingsProvider.Settings.IsAdmin,
                        (toggle, admin) => toggle && admin)
                    .ToProperty(this, nameof(ShowAdmin), out _showAdmin);
             */
            var method = element.InvocationExpressionReference.Resolve().DeclaredElement as IMethod;
            if (method?.ShortName != "WhenAnyValue")
                return;

            foreach (var argument in element.ArgumentsEnumerable)
            {
                var lambdaExpression = argument.Expression as ILambdaExpression;
                if (lambdaExpression == null)
                    continue;

                var referenceExpression = lambdaExpression.BodyExpression as IReferenceExpression;
                if (referenceExpression == null)
                    continue;

                // A.B.C
                // A.B <--
                // A <--
                var qualifierExpression = referenceExpression
                    .TraverseAcross(x => x.QualifierExpression as IReferenceExpression)
                    .Last();
                // var qualifierExpression = referenceExpression.QualifierExpression as IReferenceExpression;
                if (qualifierExpression == null)
                    continue;

                var parameter = lambdaExpression.ParameterDeclarations.FirstOrDefault();
                if (parameter == null)
                    continue;
                
                if (parameter.NameIdentifier.Name == qualifierExpression.NameIdentifier.Name)
                    continue;

                var field = qualifierExpression.Reference.Resolve().DeclaredElement as IField;
                var type = field?.GetContainingType();
                if (type?.GetClrName().FullName != (parameter.DeclaredElement.Type as IDeclaredType)?.GetClrName().FullName)
                    continue;

                consumer.AddHighlighting(new SampleHighlighting(lambdaExpression));
            }
        }
    }
}
