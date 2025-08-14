using System;
using System.Collections.Generic;
using System.Linq;

// Question 2: Healthcare Management System

// Generic Repository for Entity Management
public class Repository<T>
{
    private List<T> items = new List<T>();

    public void Add(T item)
    {
        items.Add(item);
    }

    public List<T> GetAll()
    {
        return items;
    }

    public T? GetById(Func<T, bool> predicate)
    {
        return items.FirstOrDefault(predicate);
    }

    public bool Remove(Func<T, bool> predicate)
    {
        var item = items.FirstOrDefault(predicate);
        if (item != null)
        {
            items.Remove(item);
            return true;
        }
        return false;
    }
}

// Patient class
public class Patient
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }

    public override string ToString()
    {
        return $"Patient ID: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender}";
    }
}

// Prescription class
public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }
    public string MedicationName { get; }
    public DateTime DateIssued { get; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }

    public override string ToString()
    {
        return $"Prescription ID: {Id}, Medication: {MedicationName}, Date: {DateIssued:yyyy-MM-dd}";
    }
}

// Health System Application
public class HealthSystemApp
{
    private Repository<Patient> _patientRepo = new Repository<Patient>();
    private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

    public void SeedData()
    {
        Console.WriteLine("Seeding sample data...");
        
        // Add patients
        _patientRepo.Add(new Patient(1, "John Doe", 35, "Male"));
        _patientRepo.Add(new Patient(2, "Jane Smith", 28, "Female"));
        _patientRepo.Add(new Patient(3, "Bob Johnson", 42, "Male"));

        // Add prescriptions with valid PatientIds
        _prescriptionRepo.Add(new Prescription(1, 1, "Aspirin", DateTime.Now.AddDays(-5)));
        _prescriptionRepo.Add(new Prescription(2, 1, "Ibuprofen", DateTime.Now.AddDays(-3)));
        _prescriptionRepo.Add(new Prescription(3, 2, "Paracetamol", DateTime.Now.AddDays(-2)));
        _prescriptionRepo.Add(new Prescription(4, 2, "Vitamin D", DateTime.Now.AddDays(-1)));
        _prescriptionRepo.Add(new Prescription(5, 3, "Metformin", DateTime.Now));
        
        Console.WriteLine($"Added {_patientRepo.GetAll().Count} patients and {_prescriptionRepo.GetAll().Count} prescriptions.");
    }

    public void BuildPrescriptionMap()
    {
        Console.WriteLine("\nBuilding prescription map...");
        
        var prescriptions = _prescriptionRepo.GetAll();
        _prescriptionMap = prescriptions
            .GroupBy(p => p.PatientId)
            .ToDictionary(g => g.Key, g => g.ToList());
        
        Console.WriteLine($"Prescription map built with {_prescriptionMap.Count} patient groups.");
    }

    public List<Prescription> GetPrescriptionsByPatientId(int patientId)
    {
        if (_prescriptionMap.ContainsKey(patientId))
        {
            return _prescriptionMap[patientId];
        }
        return new List<Prescription>();
    }

    public void PrintAllPatients()
    {
        Console.WriteLine("\n=== ALL PATIENTS ===");
        var patients = _patientRepo.GetAll();
        
        if (patients.Count == 0)
        {
            Console.WriteLine("No patients found.");
            return;
        }

        foreach (var patient in patients)
        {
            Console.WriteLine(patient);
        }
    }

    public void PrintPrescriptionsForPatient(int patientId)
    {
        Console.WriteLine($"\n=== PRESCRIPTIONS FOR PATIENT ID {patientId} ===");
        
        // First, get the patient info
        var patient = _patientRepo.GetById(p => p.Id == patientId);
        if (patient == null)
        {
            Console.WriteLine($"Patient with ID {patientId} not found.");
            return;
        }

        Console.WriteLine($"Patient: {patient.Name}");
        Console.WriteLine("Prescriptions:");

        var prescriptions = GetPrescriptionsByPatientId(patientId);
        if (prescriptions.Count == 0)
        {
            Console.WriteLine("No prescriptions found for this patient.");
            return;
        }

        foreach (var prescription in prescriptions)
        {
            Console.WriteLine($"  - {prescription}");
        }
    }

    // Additional method to demonstrate repository functionality
    public void DemonstrateRepositoryOperations()
    {
        Console.WriteLine("\n=== DEMONSTRATING REPOSITORY OPERATIONS ===");
        
        // Search for a specific patient
        var searchPatient = _patientRepo.GetById(p => p.Name.Contains("Jane"));
        if (searchPatient != null)
        {
            Console.WriteLine($"Found patient: {searchPatient}");
        }

        // Count prescriptions for a specific medication
        var aspirinPrescriptions = _prescriptionRepo.GetAll()
            .Where(p => p.MedicationName.Contains("Aspirin"))
            .ToList();
        Console.WriteLine($"Total Aspirin prescriptions: {aspirinPrescriptions.Count}");

        // Show prescription statistics
        var totalPrescriptions = _prescriptionRepo.GetAll().Count;
        Console.WriteLine($"Total prescriptions in system: {totalPrescriptions}");
        Console.WriteLine($"Average prescriptions per patient: {(double)totalPrescriptions / _patientRepo.GetAll().Count:F1}");
    }
}

// Program entry point
public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== HEALTHCARE MANAGEMENT SYSTEM ===");
        
        var healthApp = new HealthSystemApp();
        
        // Execute the required flow
        healthApp.SeedData();
        healthApp.BuildPrescriptionMap();
        healthApp.PrintAllPatients();
        
        // Select Patient ID 1 and display all prescriptions
        healthApp.PrintPrescriptionsForPatient(1);
        
        // Also show prescriptions for other patients
        healthApp.PrintPrescriptionsForPatient(2);
        healthApp.PrintPrescriptionsForPatient(3);
        
        // Demonstrate additional repository capabilities
        healthApp.DemonstrateRepositoryOperations();
        
        Console.WriteLine("\n=== SYSTEM DEMONSTRATION COMPLETE ===");
    }
}