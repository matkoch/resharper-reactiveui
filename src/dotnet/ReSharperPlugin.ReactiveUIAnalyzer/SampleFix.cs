using System;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReSharperPlugin.ReactiveUIAnalyzer
{
    public sealed class SampleFix : QuickFixBase
    {
        private readonly ILambdaExpression _lambdaExpression;

        public SampleFix(ILambdaExpression lambdaExpression)
        {
            _lambdaExpression = lambdaExpression;
        }

        public override string Text => "Write all lower-case";

        public override bool IsAvailable(IUserDataHolder cache)
        {
            return true;
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var qualifierExpression = (_lambdaExpression.BodyExpression as IReferenceExpression)
                .TraverseAcross(x => x.QualifierExpression as IReferenceExpression)
                .Last();
            var elementFactory = CSharpElementFactory.GetInstance(qualifierExpression);
            var referenceExpression = elementFactory.CreateReferenceExpression("x");
            qualifierExpression.SetQualifierExpression(referenceExpression);
            
            
            return null;
            var methodDeclaration = (IMethodDeclaration) _lambdaExpression;
            
            // This is greatly simplified, since we're not updating any references
            // You will probably see a small indicator in the lower-right
            // that tells you about an exception being thrown.
            methodDeclaration.SetName(methodDeclaration.DeclaredName.ToLower());
            
            return null;
        }
    }
}
