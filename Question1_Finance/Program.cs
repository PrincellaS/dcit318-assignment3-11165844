using System;
using System.Collections.Generic;

namespace FinanceManagementSystem
{
    // Define Transaction record
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // Define ITransactionProcessor interface
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // Concrete processor implementations
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Bank Transfer: Processing {transaction.Amount:C} for {transaction.Category}");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Mobile Money: Processing {transaction.Amount:C} for {transaction.Category}");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Crypto Wallet: Processing {transaction.Amount:C} for {transaction.Category}");
        }
    }

    // Base Account class
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
        }
    }

    // Sealed SavingsAccount class
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance) 
            : base(accountNumber, initialBalance)
        {
        }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
            }
            else
            {
                base.ApplyTransaction(transaction);
                Console.WriteLine($"Transaction applied. Updated balance: {Balance:C}");
            }
        }
    }

    // FinanceApp class
    public class FinanceApp
    {
        private List<Transaction> _transactions = new List<Transaction>();

        public void Run()
        {
            // Instantiate SavingsAccount
            var savingsAccount = new SavingsAccount("SAV001", 1000m);
            Console.WriteLine($"Initial balance: {savingsAccount.Balance:C}");
            Console.WriteLine();

            // Create three Transaction records
            var transaction1 = new Transaction(1, DateTime.Now, 150m, "Groceries");
            var transaction2 = new Transaction(2, DateTime.Now, 200m, "Utilities");
            var transaction3 = new Transaction(3, DateTime.Now, 50m, "Entertainment");

            // Create processors
            var mobileProcessor = new MobileMoneyProcessor();
            var bankProcessor = new BankTransferProcessor();
            var cryptoProcessor = new CryptoWalletProcessor();

            // Process transactions
            Console.WriteLine("Processing transactions:");
            
            mobileProcessor.Process(transaction1);
            savingsAccount.ApplyTransaction(transaction1);
            _transactions.Add(transaction1);
            Console.WriteLine();

            bankProcessor.Process(transaction2);
            savingsAccount.ApplyTransaction(transaction2);
            _transactions.Add(transaction2);
            Console.WriteLine();

            cryptoProcessor.Process(transaction3);
            savingsAccount.ApplyTransaction(transaction3);
            _transactions.Add(transaction3);
            Console.WriteLine();

            Console.WriteLine($"Final balance: {savingsAccount.Balance:C}");
            Console.WriteLine($"Total transactions processed: {_transactions.Count}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Finance Management System ===");
            Console.WriteLine();
            
            var app = new FinanceApp();
            app.Run();
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}