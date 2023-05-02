namespace BBServer;

public class Result<T> where T : class
{
    public Result()
    {
    }

    public Result(T? value)
    {
        Value = value;
    }

    public Result(string error)
    {
        Error = error;
    }

    public T? Value { get; set; }
    public string? Error { get; set; }
}