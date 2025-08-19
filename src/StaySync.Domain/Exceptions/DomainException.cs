namespace StaySync.Domain.Exceptions;

public class DomainException(string message) : Exception(message);

public sealed class NotFoundException(string message) : DomainException(message);
public sealed class ConflictException(string message) : DomainException(message);
