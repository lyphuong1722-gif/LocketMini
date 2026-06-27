namespace LocketMini.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("Đã xảy ra một hoặc nhiều lỗi validation.")
        => Errors = errors;
}

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message = "Bạn không có quyền thực hiện hành động này.")
        : base(message) { }
}

public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "Truy cập bị từ chối.")
        : base(message) { }
}
