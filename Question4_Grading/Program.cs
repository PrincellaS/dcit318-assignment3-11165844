using System;
using System.Collections.Generic;
using System.IO;

// Question 4: School Grading System

// Student Class
public class Student
{
    public int Id { get; }
    public string FullName { get; }
    public int Score { get; }

    public Student(int id, string fullName, int score)
    {
        Id = id;
        FullName = fullName;
        Score = score;
    }

    public string GetGrade()
    {
        return Score switch
        {
            >= 80 and <= 100 => "A",
            >= 70 and <= 79 => "B",
            >= 60 and <= 69 => "C",
            >= 50 and <= 59 => "D",
            < 50 => "F",
            _ => "Invalid Score"
        };
    }

    public override string ToString()
    {
        return $"Student ID: {Id}, Name: {FullName}, Score: {Score}, Grade: {GetGrade()}";
    }
}

// Custom Exception Classes
public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

// Student Result Processor Class
public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        var students = new List<Student>();
        int lineNumber = 0;

        using (var reader = new StreamReader(inputFilePath))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                
                // Skip empty lines
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                try
                {
                    var fields = line.Split(',');
                    
                    // Validate number of fields
                    if (fields.Length != 3)
                        throw new MissingFieldException($"Line {lineNumber}: Expected 3 fields but found {fields.Length}. Line content: '{line}'");
                    
                    // Parse and validate ID
                    if (!int.TryParse(fields[0].Trim(), out int id))
                        throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid ID format '{fields[0].Trim()}'. ID must be a number.");
                    
                    // Get full name
                    string fullName = fields[1].Trim();
                    if (string.IsNullOrEmpty(fullName))
                        throw new MissingFieldException($"Line {lineNumber}: Student name cannot be empty.");
                    
                    // Parse and validate score
                    if (!int.TryParse(fields[2].Trim(), out int score))
                        throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid score format '{fields[2].Trim()}'. Score must be a number.");
                    
                    // Additional score validation
                    if (score < 0 || score > 100)
                        throw new InvalidScoreFormatException($"Line {lineNumber}: Score {score} is out of valid range (0-100).");
                    
                    students.Add(new Student(id, fullName, score));
                    Console.WriteLine($"Processed: {fullName} (ID: {id}, Score: {score})");
                }
                catch (InvalidScoreFormatException)
                {
                    throw; // Re-throw custom exceptions
                }
                catch (MissingFieldException)
                {
                    throw; // Re-throw custom exceptions
                }
                catch (Exception ex)
                {
                    throw new Exception($"Line {lineNumber}: Unexpected error processing line '{line}': {ex.Message}");
                }
            }
        }
        
        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using (var writer = new StreamWriter(outputFilePath))
        {
            // Write header
            writer.WriteLine("=" + new string('=', 50));
            writer.WriteLine("         STUDENT GRADE REPORT");
            writer.WriteLine("=" + new string('=', 50));
            writer.WriteLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            writer.WriteLine($"Total Students: {students.Count}");
            writer.WriteLine();

            // Write student results
            foreach (var student in students)
            {
                writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
            }

            // Write statistics
            writer.WriteLine();
            writer.WriteLine("=" + new string('=', 50));
            writer.WriteLine("STATISTICS");
            writer.WriteLine("=" + new string('=', 50));
            
            if (students.Count > 0)
            {
                var gradeDistribution = new Dictionary<string, int>
                {
                    {"A", 0}, {"B", 0}, {"C", 0}, {"D", 0}, {"F", 0}
                };

                double totalScore = 0;
                foreach (var student in students)
                {
                    totalScore += student.Score;
                    gradeDistribution[student.GetGrade()]++;
                }

                writer.WriteLine($"Average Score: {totalScore / students.Count:F1}");
                writer.WriteLine();
                writer.WriteLine("Grade Distribution:");
                foreach (var grade in gradeDistribution)
                {
                    writer.WriteLine($"  Grade {grade.Key}: {grade.Value} students");
                }
            }
        }
    }

    public void ProcessStudents(string inputPath, string outputPath)
    {
        try
        {
            Console.WriteLine("=== STUDENT GRADING SYSTEM ===");
            Console.WriteLine($"Reading from: {inputPath}");
            
            // Create sample input file if it doesn't exist
            if (!File.Exists(inputPath))
            {
                Console.WriteLine("Input file not found. Creating sample file...");
                CreateSampleInputFile(inputPath);
            }
            
            Console.WriteLine("\nProcessing students...");
            var students = ReadStudentsFromFile(inputPath);
            
            Console.WriteLine($"\nWriting report to: {outputPath}");
            WriteReportToFile(students, outputPath);
            
            Console.WriteLine($"\n✓ Successfully processed {students.Count} students.");
            Console.WriteLine($"✓ Report written to: {outputPath}");
            
            // Display summary
            DisplaySummary(students);
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"❌ File Error: The input file '{inputPath}' was not found.");
            Console.WriteLine($"   Details: {ex.Message}");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine($"❌ Score Format Error: {ex.Message}");
            Console.WriteLine("   Please ensure all scores are valid numbers between 0-100.");
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine($"❌ Missing Field Error: {ex.Message}");
            Console.WriteLine("   Expected format: StudentID,FullName,Score");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"❌ Access Error: Unable to access file. {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Unexpected Error: {ex.Message}");
            Console.WriteLine("   Please check the file format and try again.");
        }
    }

    private void CreateSampleInputFile(string filePath)
    {
        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine("101,Alice Smith,84");
            writer.WriteLine("102,Bob Johnson,72");
            writer.WriteLine("103,Carol Davis,91");
            writer.WriteLine("104,David Wilson,58");
            writer.WriteLine("105,Emma Brown,45");
            writer.WriteLine("106,Frank Miller,88");
            writer.WriteLine("107,Grace Lee,67");
        }
        Console.WriteLine("Sample input file created with 7 student records.");
    }

    private void DisplaySummary(List<Student> students)
    {
        Console.WriteLine("\n=== PROCESSING SUMMARY ===");
        
        if (students.Count == 0)
        {
            Console.WriteLine("No students processed.");
            return;
        }

        var gradeCount = new Dictionary<string, int>();
        double totalScore = 0;

        foreach (var student in students)
        {
            string grade = student.GetGrade();
            gradeCount[grade] = gradeCount.GetValueOrDefault(grade, 0) + 1;
            totalScore += student.Score;
        }

        Console.WriteLine($"Total Students: {students.Count}");
        Console.WriteLine($"Average Score: {totalScore / students.Count:F1}");
        Console.WriteLine("\nGrade Distribution:");
        
        foreach (var grade in new[] { "A", "B", "C", "D", "F" })
        {
            int count = gradeCount.GetValueOrDefault(grade, 0);
            if (count > 0)
            {
                Console.WriteLine($"  {grade}: {count} students");
            }
        }
    }

    // Method to test error handling scenarios
    public void TestErrorScenarios()
    {
        Console.WriteLine("\n=== TESTING ERROR SCENARIOS ===");
        
        // Test with invalid file
        Console.WriteLine("\n1. Testing with non-existent file:");
        ProcessStudents("nonexistent.txt", "error_test_output.txt");
        
        // Create file with errors for testing
        Console.WriteLine("\n2. Testing with malformed data:");
        CreateErrorTestFile("error_test_input.txt");
        ProcessStudents("error_test_input.txt", "error_test_output.txt");
    }

    private void CreateErrorTestFile(string filePath)
    {
        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine("201,Valid Student,75");
            writer.WriteLine("202,Missing Score"); // Missing field
            writer.WriteLine("abc,Invalid ID,80"); // Invalid ID format
            writer.WriteLine("203,Invalid Score,xyz"); // Invalid score format
            writer.WriteLine("204,Out of Range,150"); // Score out of range
        }
    }
}

// Program Entry Point
public class Program
{
    public static void Main()
    {
        var processor = new StudentResultProcessor();
        
        // Main application flow
        processor.ProcessStudents("students.txt", "grade_report.txt");
        
        // Demonstrate error handling (optional)
        Console.WriteLine("\n" + new string('-', 60));
        Console.WriteLine("Would you like to test error handling scenarios? (y/n)");
        
        // For demonstration, we'll show error handling
        Console.WriteLine("Demonstrating error handling...");
        processor.TestErrorScenarios();
        
        Console.WriteLine("\n=== GRADING SYSTEM DEMONSTRATION COMPLETE ===");
    }
}