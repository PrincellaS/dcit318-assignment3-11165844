using System;
using System.Collections.Generic;
using System.Linq;

// Question 3: Warehouse Inventory Management System

// Marker Interface for Inventory Items
public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

// Electronic Item Class
public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }

    public override string ToString()
    {
        return $"Electronic: {Name} (ID: {Id}), Brand: {Brand}, Qty: {Quantity}, Warranty: {WarrantyMonths} months";
    }
}

// Grocery Item Class
public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }

    public override string ToString()
    {
        return $"Grocery: {Name} (ID: {Id}), Qty: {Quantity}, Expires: {ExpiryDate:yyyy-MM-dd}";
    }
}

// Custom Exception Classes
public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message) { }
}

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message) { }
}

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}

// Generic Inventory Repository
public class InventoryRepository<T> where T : IInventoryItem
{
    private Dictionary<int, T> _items = new Dictionary<int, T>();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
            throw new DuplicateItemException($"Item with ID {item.Id} already exists in inventory");
        
        _items[item.Id] = item;
        Console.WriteLine($"Added: {item}");
    }

    public T GetItemById(int id)
    {
        if (!_items.ContainsKey(id))
            throw new ItemNotFoundException($"Item with ID {id} not found in inventory");
        
        return _items[id];
    }

    public void RemoveItem(int id)
    {
        if (!_items.ContainsKey(id))
            throw new ItemNotFoundException($"Cannot remove - Item with ID {id} not found in inventory");
        
        var item = _items[id];
        _items.Remove(id);
        Console.WriteLine($"Removed: {item.Name} (ID: {id})");
    }

    public List<T> GetAllItems()
    {
        return _items.Values.ToList();
    }

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
            throw new InvalidQuantityException("Quantity cannot be negative");
        
        var item = GetItemById(id); // This will throw if item not found
        int oldQuantity = item.Quantity;
        item.Quantity = newQuantity;
        Console.WriteLine($"Updated {item.Name} quantity from {oldQuantity} to {newQuantity}");
    }

    public int GetItemCount()
    {
        return _items.Count;
    }
}

// Warehouse Manager Class
public class WareHouseManager
{
    public InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
    public InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

    public void SeedData()
    {
        Console.WriteLine("=== SEEDING WAREHOUSE DATA ===");
        
        try
        {
            // Add electronic items
            Console.WriteLine("\nAdding Electronic Items:");
            _electronics.AddItem(new ElectronicItem(1, "iPhone 15", 10, "Apple", 12));
            _electronics.AddItem(new ElectronicItem(2, "Samsung TV", 5, "Samsung", 24));
            _electronics.AddItem(new ElectronicItem(3, "Dell Laptop", 8, "Dell", 36));

            // Add grocery items
            Console.WriteLine("\nAdding Grocery Items:");
            _groceries.AddItem(new GroceryItem(1, "Milk", 50, DateTime.Now.AddDays(7)));
            _groceries.AddItem(new GroceryItem(2, "Bread", 30, DateTime.Now.AddDays(3)));
            _groceries.AddItem(new GroceryItem(3, "Apples", 100, DateTime.Now.AddDays(10)));
            
            Console.WriteLine($"\nSeeding complete! Electronics: {_electronics.GetItemCount()}, Groceries: {_groceries.GetItemCount()}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during seeding: {ex.Message}");
        }
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        var items = repo.GetAllItems();
        string itemType = typeof(T).Name.Replace("Item", "");
        
        Console.WriteLine($"\n=== ALL {itemType.ToUpper()} ITEMS ===");
        
        if (items.Count == 0)
        {
            Console.WriteLine("No items found in inventory.");
            return;
        }

        foreach (var item in items)
        {
            Console.WriteLine($"  {item}");
        }
        Console.WriteLine($"Total {itemType} items: {items.Count}");
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            int newQuantity = item.Quantity + quantity;
            repo.UpdateQuantity(id, newQuantity);
            Console.WriteLine($"Successfully increased stock for {item.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error increasing stock: {ex.Message}");
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Successfully removed item with ID {id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing item: {ex.Message}");
        }
    }

    public void TestExceptionHandling()
    {
        Console.WriteLine("\n=== TESTING EXCEPTION HANDLING ===");
        
        Console.WriteLine("\n1. Testing Duplicate Item Exception:");
        try
        {
            _electronics.AddItem(new ElectronicItem(1, "Duplicate iPhone", 1, "Apple", 12));
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine($"✓ Caught expected exception: {ex.Message}");
        }

        Console.WriteLine("\n2. Testing Item Not Found Exception (Remove):");
        try
        {
            _groceries.RemoveItem(999);
        }
        catch (ItemNotFoundException ex)
        {
            Console.WriteLine($"✓ Caught expected exception: {ex.Message}");
        }

        Console.WriteLine("\n3. Testing Item Not Found Exception (Update):");
        try
        {
            _electronics.UpdateQuantity(888, 10);
        }
        catch (ItemNotFoundException ex)
        {
            Console.WriteLine($"✓ Caught expected exception: {ex.Message}");
        }

        Console.WriteLine("\n4. Testing Invalid Quantity Exception:");
        try
        {
            _electronics.UpdateQuantity(1, -5);
        }
        catch (InvalidQuantityException ex)
        {
            Console.WriteLine($"✓ Caught expected exception: {ex.Message}");
        }
    }

    public void DemonstrateOperations()
    {
        Console.WriteLine("\n=== DEMONSTRATING WAREHOUSE OPERATIONS ===");
        
        // Increase stock for an electronic item
        Console.WriteLine("\nIncreasing iPhone stock by 5:");
        IncreaseStock(_electronics, 1, 5);
        
        // Update grocery quantity
        Console.WriteLine("\nUpdating milk quantity to 75:");
        try
        {
            _groceries.UpdateQuantity(1, 75);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        
        // Show updated inventories
        PrintAllItems(_electronics);
        PrintAllItems(_groceries);
    }
}

// Program Entry Point
public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== WAREHOUSE INVENTORY MANAGEMENT SYSTEM ===");
        
        var warehouse = new WareHouseManager();
        
        // Execute main application flow
        warehouse.SeedData();
        
        // Print all grocery items
        warehouse.PrintAllItems(warehouse._groceries);
        
        // Print all electronic items
        warehouse.PrintAllItems(warehouse._electronics);
        
        // Test exception handling scenarios
        warehouse.TestExceptionHandling();
        
        // Demonstrate additional operations
        warehouse.DemonstrateOperations();
        
        Console.WriteLine("\n=== WAREHOUSE SYSTEM DEMONSTRATION COMPLETE ===");
    }
}