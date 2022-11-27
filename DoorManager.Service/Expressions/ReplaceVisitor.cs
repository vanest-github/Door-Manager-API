using System.Linq.Expressions;

namespace DoorManager.Service.Expressions;

internal class ReplaceVisitor : ExpressionVisitor
{
    private readonly Expression from;
    private readonly Expression to;

    public ReplaceVisitor(Expression from, Expression to)
    {
        this.from = from;
        this.to = to;
    }

    public override Expression Visit(Expression node)
    {
        return node == this.from ? this.to : base.Visit(node);
    }
}
