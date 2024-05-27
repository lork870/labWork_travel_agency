using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public interface IPayment
{
    void Pay(double amount);
}

public delegate void TourPackageOrderedEventHandler(string packageName, double price);

public class Country
{
    public string Name { get; set; }
    public string Capital { get; set; }
    public int Population { get; set; }
    public string Language { get; set; }

    public Country(string name, string capital, int population, string language)
    {
        Name = name;
        Capital = capital;
        Population = population;
        Language = language;
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"Country: {Name}");
        Console.WriteLine($"Capital: {Capital}");
        Console.WriteLine($"Population: {Population}");
        Console.WriteLine($"Official language: {Language}");
    }
}

public class TourPackage : IPayment
{
    public string PackageName { get; set; }
    public double Price { get; set; }
    public Country Destination { get; set; }

    public event TourPackageOrderedEventHandler TourPackageOrdered;

    public TourPackage(string packageName, double price, Country destination)
    {
        PackageName = packageName;
        Price = price;
        Destination = destination;
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"Package: {PackageName}");
        Console.WriteLine($"Price: {Price}$");
        Console.WriteLine("Country information:");
        Destination.DisplayInfo();
    }

    public void Pay(double amount)
    {
        Console.WriteLine($"Paid {amount}$ for the package \"{PackageName}\"");
    }

    public void OrderTourPackage()
    {
        TourPackageOrdered?.Invoke(PackageName, Price);
    }
}

public class Tourist
{
    public string Name { get; set; }
    public int Age { get; set; }
    public double Budget { get; set; }

    public Tourist(string name, int age, double budget)
    {
        Name = name;
        Age = age;
        Budget = budget;
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"Tourist: {Name}");
        Console.WriteLine($"Age: {Age}");
        Console.WriteLine($"Budget: {Budget}$");
    }
}

public class TravelAgency
{
    public TourPackage BookTourPackage(string packageName, double price, Country destination)
    {
        return new TourPackage(packageName, price, destination);
    }

    public void PayForTourPackage(TourPackage tourPackage, double amount)
    {
        tourPackage.Pay(amount);
    }
}

class Program
{
    static void Main(string[] args)
    {
        List<Country> countries = new List<Country>
        {
            new Country("France", "Paris", 67000000, "French"),
            new Country("Italy", "Rome", 60000000, "Italian"),
            new Country("Spain", "Madrid", 47000000, "Spanish"),
            new Country("Germany", "Berlin", 83000000, "German"),
            new Country("Portugal", "Lisbon", 10280000, "Portuguese")
        };

        TravelAgency agency = new TravelAgency();

        int choice;
        do
        {
            Console.WriteLine("Select the type of vacation:");
            Console.WriteLine("1. Sightseeing tours");
            Console.WriteLine("2. Beach vacation");
            Console.WriteLine("3. Active vacation");
            Console.WriteLine("4. Cultural vacation");
            Console.WriteLine("5. Gastronomic tourism");
        } while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 5);

        int countryChoice;
        do
        {
            Console.WriteLine("Select the country:");

            for (int i = 0; i < countries.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {countries[i].Name}");
            }
        } while (!int.TryParse(Console.ReadLine(), out countryChoice) || countryChoice < 1 || countryChoice > countries.Count);

        string name;
        do
        {
            Console.WriteLine("Enter your name:");
            name = Console.ReadLine();
        } while (string.IsNullOrWhiteSpace(name) || !Regex.IsMatch(name, @"^[a-zA-Zа-яА-Я\s]+$"));

        int age;
        do
        {
            Console.WriteLine("Enter your age:");
        } while (!int.TryParse(Console.ReadLine(), out age) || age < 1);

        double budget;
        do
        {
            Console.WriteLine("Enter your budget:");
        } while (!double.TryParse(Console.ReadLine(), out budget) || budget < 0);

        Country selectedCountry = countries[countryChoice - 1];

        Tourist tourist = new Tourist(name, age, budget);
        tourist.DisplayInfo();

        double maxPrice = 2000; // Maximum price for a tour package

        TourPackage package = agency.BookTourPackage("Sightseeing", maxPrice, selectedCountry);
        if (package != null)
        {
            if (tourist.Budget >= package.Price)
            {
                package.DisplayInfo();
                agency.PayForTourPackage(package, package.Price);

                // Subscribe to the TourPackageOrdered event and handle it
                package.TourPackageOrdered += (pkgName, pkgPrice) =>
                {
                    Console.WriteLine($"Tour Package Ordered: {pkgName} for {pkgPrice}$");
                };
                package.OrderTourPackage();
            }
            else
            {
                Console.WriteLine("Sorry, this package is not available within your budget.");
            }
        }
        else
        {
            Console.WriteLine("Sorry, a tour package to this country is not available.");
        }
    }
}
