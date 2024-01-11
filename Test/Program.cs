public record BankAccount(string AccountNumber, decimal Balance);

public static class BankOperations
{
    public static BankAccount Deposit(BankAccount account, decimal amount)
    {
        return account with { Balance = account.Balance + amount };
    }

    public static (BankAccount, bool) Withdraw(BankAccount account, decimal amount)
    {
        if (amount > account.Balance)
        {
            return (account, false);
        }
        return (account with { Balance = account.Balance - amount }, true);
    }
}

// Usage
BankAccount account = new("123456", 1000);
account = BankOperations.Deposit(account, 500);
var (updatedAccount, success) = BankOperations.Withdraw(account, 200);




public class BankAccount
{
    public string AccountNumber { get; private set; }
    private decimal balance;

    public BankAccount(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        balance = initialBalance;
    }

    public void Deposit(decimal amount)
    {
        balance += amount;
    }

    public bool Withdraw(decimal amount)
    {
        if (amount > balance) return false;

        balance -= amount;
        return true;
    }

    public decimal CheckBalance()
    {
        return balance;
    }
}
