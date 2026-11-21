namespace EWallet.Domain.Exceptions;

public sealed class InsufficientBalanceException(string message) : Exception(message);
