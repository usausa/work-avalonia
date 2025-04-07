namespace Smart.Avalonia.Expressions;

public interface IBinaryExpression
{
    object? Eval(object? left, object? right);
}
