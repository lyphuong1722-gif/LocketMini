namespace LocketMini.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception inner) : base(message, inner) { }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key)
        : base($"Không tìm thấy '{entityName}' với key = {key}.") { }
}

public class ConflictException : DomainException
{
    public ConflictException(string message) : base(message) { }
}
