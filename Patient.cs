namespace MedicalClinicREST;

public class Patient
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string Pesel { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public Address Address { get; set; } = new Address();
}

public class Address
{
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string HouseNumber { get; set; } = string.Empty;
    public string? ApartmentNumber { get; set; }
    public string PostalCode { get; set; } = string.Empty;
}