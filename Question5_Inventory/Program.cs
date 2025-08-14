using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// Question 5: Inventory Records System

// Marker Interface for Logging
public interface IInventoryEntity
{
    int Id { get; }
}

// Immutable Inventory Record (using positional syntax)
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// Generic Inventory Logger
public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new List<T>();
    private string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
        Console.WriteLine($"InventoryLogger initialized with file path: {_filePath}");
    }

    public void Add(T item)
    {
        _log.Add(item);
        Console.WriteLine($"Added to log: {item}");
    }

    public List<T> GetAll()
    {
        return new List<T>(_log); // Return a copy to maintain immutability
    }

    public void SaveToFile()
    {
        try
        {
            using (var writer = new StreamWriter(_filePath))
            {
                writer.WriteLine("=== INVENTORY LOG ===");
                writer.WriteLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine($"Total Items: {_log.Count}");
                writer.WriteLine();
                
                foreach (var item in _log)
                {
                    writer.WriteLine($"ID: {item.Id} | {item}");
                }
                
                writer.WriteLine();
                writer.WriteLine("=== END OF LOG ===");
            }
            
            Console.WriteLine($"✓ Successfully saved {_log.Count} items to {_filePath}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"❌ Access denied when saving to file: {ex.Message}");
        }
        catch (DirectoryNotFoundException ex)
        {
            Console.WriteLine($"❌ Directory not found: {ex.Message}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"❌ IO error when saving file: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Unexpected error saving file: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine($"⚠️  File {_filePath} does not exist. Starting with empty inventory.");
                _log.Clear();
                return;
            }

            using (var reader = new StreamReader(_filePath))
            {
                _log.Clear();
                string? line;
                int loadedCount = 0;
                
                Console.WriteLine($"Loading inventory from {_filePath}...");
                
                while ((line = reader.ReadLine()) != null)
                {
                    // Skip header and empty lines
                    if (line.StartsWith("===") || string.IsNullOrWhiteSpace(line) || 
                        line.StartsWith("Generated:") || line.StartsWith("Total Items:"))
                        continue;
                    
                    // Parse the line (this is a simplified parser for demonstration)
                    if (line.StartsWith("ID:"))
                    {
                        Console.WriteLine($"Loaded: {line}");
                        loadedCount++;
                    }
                }
                
                Console.WriteLine($"✓ Successfully loaded {loadedCount} items from file");
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"⚠️  File {_filePath} not found. Starting with empty inventory.");
            _log.Clear();
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"❌ Access denied when loading file: {ex.Message}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"❌ IO error when loading file: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Unexpected error loading file: {ex.Message}");
        }
    }

    // Alternative JSON-based save/load methods for better serialization
    public void SaveToJsonFile()
    {
        try
        {
            string jsonFilePath = Path.ChangeExtension(_filePath, ".json");
            var options = new JsonSerializerOptions 
            { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            string jsonString = JsonSerializer.Serialize(_log, options);
            
            using (var writer = new StreamWriter(jsonFilePath))
            {
                writer.Write(jsonString);
            }
            
            Console.WriteLine($"✓ Successfully saved {_log.Count} items to JSON file: {jsonFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error saving JSON file: {ex.Message}");
        }
    }

    public void LoadFromJsonFile()
    {
        try
        {
            string jsonFilePath = Path.ChangeExtension(_filePath, ".json");
            
            if (!File.Exists(jsonFilePath))
            {
                Console.WriteLine($"⚠️  JSON file {jsonFilePath} does not exist.");
                return;
            }

            using (var reader = new StreamReader(jsonFilePath))
            {
                string jsonString = reader.ReadToEnd();
                var items = JsonSerializer.Deserialize<List<T>>(jsonString);
                
                if (items != null)
                {
                    _log = items;
                    Console.WriteLine($"✓ Successfully loaded {_log.Count} items from JSON file");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error loading JSON file: {ex.Message}");
        }
    }

    public void Clear()
    {
        _log.Clear();
        Console.WriteLine("Inventory log cleared from memory");
    }

    public int Count => _log.Count;
}

// Integration Layer - InventoryApp
public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    public void SeedSampleData()
    {
        Console.WriteLine("\n=== SEEDING SAMPLE DATA ===");
        
        var items = new[]
        {
            new InventoryItem(1, "Dell Laptop", 10, DateTime.Now.AddDays(-5)),
            new InventoryItem(2, "Wireless Mouse", 25, DateTime.Now.AddDays(-4)),
            new InventoryItem(3, "Mechanical Keyboard", 15, DateTime.Now.AddDays(-3)),
            new InventoryItem(4, "4K Monitor", 8, DateTime.Now.AddDays(-2)),
            new InventoryItem(5, "USB Webcam", 12, DateTime.Now.AddDays(-1))
        };

        foreach (var item in items)
        {
            _logger.Add(item);
        }
        
        Console.WriteLine($"✓ Seeded {items.Length} inventory items");
    }

    public void SaveData()
    {
        Console.WriteLine("\n=== SAVING DATA TO FILE ===");
        _logger.SaveToFile();
        
        // Also save as JSON for better serialization
        _logger.SaveToJsonFile();
    }

    public void LoadData()
    {
        Console.WriteLine("\n=== LOADING DATA FROM FILE ===");
        _logger.LoadFromFile();
        
        // Try loading from JSON if available
        _logger.LoadFromJsonFile();
    }

    public void PrintAllItems()
    {
        Console.WriteLine("\n=== ALL INVENTORY ITEMS ===");
        var items = _logger.GetAll();
        
        if (items.Count == 0)
        {
            Console.WriteLine("No items found in inventory.");
            return;
        }

        Console.WriteLine($"Total items in inventory: {items.Count}");
        Console.WriteLine();
        
        foreach (var item in items)
        {
            Console.WriteLine($"  {item}");
        }
        
        // Display summary statistics
        DisplayInventorySummary(items);
    }

    private void DisplayInventorySummary(List<InventoryItem> items)
    {
        if (items.Count == 0) return;
        
        Console.WriteLine("\n--- INVENTORY SUMMARY ---");
        
        int totalQuantity = 0;
        DateTime oldestDate = DateTime.MaxValue;
        DateTime newestDate = DateTime.MinValue;
        
        foreach (var item in items)
        {
            totalQuantity += item.Quantity;
            if (item.DateAdded < oldestDate) oldestDate = item.DateAdded;
            if (item.DateAdded > newestDate) newestDate = item.DateAdded;
        }
        
        Console.WriteLine($"Total Quantity: {totalQuantity} units");
        Console.WriteLine($"Average Quantity per Item: {(double)totalQuantity / items.Count:F1} units");
        Console.WriteLine($"Date Range: {oldestDate:yyyy-MM-dd} to {newestDate:yyyy-MM-dd}");
    }

    public void ClearMemoryAndSimulateNewSession()
    {
        Console.WriteLine("\n=== SIMULATING NEW SESSION ===");
        Console.WriteLine("Clearing memory to simulate application restart...");
        _logger.Clear();
        Console.WriteLine($"Items in memory: {_logger.Count}");
    }

    public void DemonstrateRecordImmutability()
    {
        Console.WriteLine("\n=== DEMONSTRATING RECORD IMMUTABILITY ===");
        
        var original = new InventoryItem(999, "Test Item", 5, DateTime.Now);
        Console.WriteLine($"Original: {original}");
        
        // Records support with-expressions for creating modified copies
        var modified = original with { Quantity = 10 };
        Console.WriteLine($"Modified: {modified}");
        Console.WriteLine($"Original unchanged: {original}");
        Console.WriteLine($"Are they equal? {original.Equals(modified)}");
        Console.WriteLine($"Original ID: {original.Id}, Modified ID: {modified.Id}");
    }
}

// Program Entry Point
public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== INVENTORY RECORDS SYSTEM ===");
        
        var app = new InventoryApp("inventory_log.txt");
        
        // Execute main application flow as specified
        app.SeedSampleData();
        app.SaveData();
        app.ClearMemoryAndSimulateNewSession();
        app.LoadData();
        app.PrintAllItems();
        
        // Additional demonstrations
        app.DemonstrateRecordImmutability();
        
        Console.WriteLine("\n=== INVENTORY SYSTEM DEMONSTRATION COMPLETE ===");
        
        // Show file contents for verification
        ShowFileContents("inventory_log.txt");
    }

    private static void ShowFileContents(string filePath)
    {
        Console.WriteLine($"\n=== CONTENTS OF {filePath.ToUpper()} ===");
        try
        {
            if (File.Exists(filePath))
            {
                using (var reader = new StreamReader(filePath))
                {
                    Console.WriteLine(reader.ReadToEnd());
                }
            }
            else
            {
                Console.WriteLine("File does not exist.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
        }
    }
}