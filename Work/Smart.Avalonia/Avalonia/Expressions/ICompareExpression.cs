namespace Smart.Avalonia.Expressions;

public interface ICompareExpression
{
    bool Eval(object? left, object? right);
}
