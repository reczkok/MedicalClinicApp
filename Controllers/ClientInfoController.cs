using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MedicalClinicREST.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientInfoController : ControllerBase
{

    private const string ConnectionUri = "mongodb+srv://admin:admin@clientinfo.gjpfecb.mongodb.net/?retryWrites=true&w=majority&appName=ClientInfo";

    private readonly ILogger<ClientInfoController> _logger;

    public ClientInfoController(ILogger<ClientInfoController> logger)
    {
        _logger = logger;
    }

    private static IEnumerable<Patient> _processBsonDocuments(IEnumerable<BsonDocument> documents)
    {
        return documents.Select(document => new Patient
            {
                FirstName = document["FirstName"].AsString,
                LastName = document["LastName"].AsString,
                MiddleName = document.Contains("MiddleName") ? document["MiddleName"].AsString : null,
                Pesel = document["Pesel"].AsString,
                PhoneNumber = document.Contains("PhoneNumber") ? document["PhoneNumber"].AsString : null,
                Email = document.Contains("Email") ? document["Email"].AsString : null,
                Address = new Address
                {
                    City = document["Address"]["City"].AsString,
                    Street = document["Address"]["Street"].AsString,
                    HouseNumber = document["Address"]["HouseNumber"].AsString,
                    ApartmentNumber = document.Contains("ApartmentNumber") ? document["Address"]["ApartmentNumber"].AsString : null,
                    PostalCode = document["Address"]["PostalCode"].AsString
                }
            })
            .ToList();
    }

    [HttpPost("GenerateSampleData")]
    public void GenerateSampleData()
    {
        var firstNameBase = new List<string>
            { "John", "Jane", "Michael", "Jessica", "Robert", "Emily", "David", "Sarah", "William", "Ashley" };
        var lastNameBase = new List<string>
            { "Smith", "Johnson", "Williams", "Jones", "Brown", "Davis", "Miller", "Wilson", "Moore", "Taylor" };
        var middleNameBase = new List<string>
            { "Adam", "Eve", "George", "Lucy", "Henry", "Grace", "Samuel", "Olivia", "Benjamin", "Sophia" };
        var cityBase = new List<string>
        {
            "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego",
            "Dallas", "San Jose"
        };
        var streetBase = new List<string>
            { "Main", "First", "Second", "Third", "Fourth", "Fifth", "Sixth", "Seventh", "Eighth", "Ninth" };
        var postalCodeBase = new List<string>
            { "10001", "90001", "60601", "77001", "85001", "19101", "78201", "92101", "75201", "95101" };

        var client = new MongoClient(ConnectionUri);
        var database = client.GetDatabase("ClientInfo");
        var collection = database.GetCollection<BsonDocument>("Patients");

        var random = new Random();
        for (var i = 0; i < 100; i++)
        {
            var middleName = random.Next(1, 11) <= 8 ? middleNameBase[random.Next(0, 10)] : null;
            var phoneNumber = random.Next(1, 21) <= 19 ? "555-555-555" + random.Next(100, 1000) : null;
            var email = random.Next(1, 11) <= 9 ? firstNameBase[random.Next(0, 10)] + lastNameBase[random.Next(0, 10)] + "@example.com" : null;
            var apartmentNumber = random.Next(1, 3) == 1 ? random.Next(1, 100).ToString() : null;
            var pesel = random.NextInt64(10000000000, 100000000000).ToString();
            
            var document = new BsonDocument
            {
                { "FirstName", firstNameBase[random.Next(0, 10)] },
                { "LastName", lastNameBase[random.Next(0, 10)] },
            };
            
            if (middleName != null) document.Add("MiddleName", middleName);
            document.Add("Pesel", pesel);
            if (phoneNumber != null) document.Add("PhoneNumber", phoneNumber);
            if (email != null) document.Add("Email", email);
            if (apartmentNumber != null)
            {
                document.Add("Address", new BsonDocument
                    {
                        { "City", cityBase[random.Next(0, 10)] },
                        { "Street", streetBase[random.Next(0, 10)] },
                        { "HouseNumber", random.Next(1, 100).ToString() },
                        { "ApartmentNumber", apartmentNumber },
                        { "PostalCode", postalCodeBase[random.Next(0, 10)] }
                    }
                );
            }
            else
            {
                document.Add("Address", new BsonDocument
                    {
                        { "City", cityBase[random.Next(0, 10)] },
                        { "Street", streetBase[random.Next(0, 10)] },
                        { "HouseNumber", random.Next(1, 100).ToString() },
                        { "PostalCode", postalCodeBase[random.Next(0, 10)] }
                    }
                );
            }
            
            collection.InsertOne(document);
        }
    }

    [HttpGet]
    public IEnumerable<Patient> Get()
    {
        var client = new MongoClient(ConnectionUri);
        var database = client.GetDatabase("ClientInfo");
        var collection = database.GetCollection<BsonDocument>("Patients");

        var documents = collection.Find(new BsonDocument()).ToList();

        return _processBsonDocuments(documents);
    }
    
    [HttpPost]
    public void Post([FromBody] Patient patient)
    {
        var client = new MongoClient(ConnectionUri);
        var database = client.GetDatabase("ClientInfo");
        var collection = database.GetCollection<BsonDocument>("Patients");

        var document = new BsonDocument
        {
            { "FirstName", patient.FirstName },
            { "LastName", patient.LastName },
        };
        
        if (patient.MiddleName != null)
        {
            document.Add("MiddleName", patient.MiddleName);
        }
        
        document.Add("Pesel", patient.Pesel);
        
        if (patient.PhoneNumber != null)
        {
            document.Add("PhoneNumber", patient.PhoneNumber);
        }
        
        if (patient.Email != null)
        {
            document.Add("Email", patient.Email);
        }
        
        if (patient.Address.ApartmentNumber != null)
        {
            document.Add("Address", new BsonDocument
                {
                    { "City", patient.Address.City },
                    { "Street", patient.Address.Street },
                    { "HouseNumber", patient.Address.HouseNumber },
                    { "ApartmentNumber", patient.Address.ApartmentNumber },
                    { "PostalCode", patient.Address.PostalCode }
                }
            );
        }
        else
        {
            document.Add("Address", new BsonDocument
                {
                    { "City", patient.Address.City },
                    { "Street", patient.Address.Street },
                    { "HouseNumber", patient.Address.HouseNumber },
                    { "PostalCode", patient.Address.PostalCode }
                }
            );
        }

        collection.InsertOne(document);
    }
    
    [HttpPut]
    public void Put([FromBody] Patient patient)
    {
        var client = new MongoClient(ConnectionUri);
        var database = client.GetDatabase("ClientInfo");
        var collection = database.GetCollection<BsonDocument>("Patients");

        var filter = Builders<BsonDocument>.Filter.Eq("Pesel", patient.Pesel);
        var update = Builders<BsonDocument>.Update
            .Set("FirstName", patient.FirstName)
            .Set("LastName", patient.LastName);
        
        update = patient.MiddleName != null ? update.Set("MiddleName", patient.MiddleName) : update.Unset("MiddleName");
        update = patient.PhoneNumber != null ? update.Set("PhoneNumber", patient.PhoneNumber) : update.Unset("PhoneNumber");
        update = patient.Email != null ? update.Set("Email", patient.Email) : update.Unset("Email");
        
        if (patient.Address.ApartmentNumber != null)
        {
            update = update.Set("Address", new BsonDocument
                {
                    { "City", patient.Address.City },
                    { "Street", patient.Address.Street },
                    { "HouseNumber", patient.Address.HouseNumber },
                    { "ApartmentNumber", patient.Address.ApartmentNumber },
                    { "PostalCode", patient.Address.PostalCode }
                }
            );
        }
        else
        {
            update = update.Set("Address", new BsonDocument
                {
                    { "City", patient.Address.City },
                    { "Street", patient.Address.Street },
                    { "HouseNumber", patient.Address.HouseNumber },
                    { "PostalCode", patient.Address.PostalCode }
                }
            );
        }
        
        collection.UpdateOne(filter, update);
    }
    
    [HttpDelete("{pesel}")]
    public void DeletePatientByPesel(string pesel)
    {
        var client = new MongoClient(ConnectionUri);
        var database = client.GetDatabase("ClientInfo");
        var collection = database.GetCollection<BsonDocument>("Patients");

        var filter = Builders<BsonDocument>.Filter.Eq("Pesel", pesel);

        collection.DeleteOne(filter);
    }
    
    [HttpGet("{pesel}")]
    public Patient? GetPatientByPesel(string pesel)
    {
        var client = new MongoClient(ConnectionUri);
        var database = client.GetDatabase("ClientInfo");
        var collection = database.GetCollection<BsonDocument>("Patients");

        var filter = Builders<BsonDocument>.Filter.Eq("Pesel", pesel);
        var document = collection.Find(filter).FirstOrDefault();

        var patient = _processBsonDocuments(new List<BsonDocument> { document }).FirstOrDefault();
        
        return patient;
    }
    
    [HttpGet("City/{city}")]
    public IEnumerable<Patient> GetPatientsByCity(string city)
    {
        var client = new MongoClient(ConnectionUri);
        var database = client.GetDatabase("ClientInfo");
        var collection = database.GetCollection<BsonDocument>("Patients");

        var filter = Builders<BsonDocument>.Filter.Eq("Address.City", city);
        var documents = collection.Find(filter).ToList();
        
        return _processBsonDocuments(documents);
    }
    
    [HttpGet("PostalCode/{postalCode}")]
    public IEnumerable<Patient> GetPatientsByPostalCode(string postalCode)
    {
        var client = new MongoClient(ConnectionUri);
        var database = client.GetDatabase("ClientInfo");
        var collection = database.GetCollection<BsonDocument>("Patients");

        var filter = Builders<BsonDocument>.Filter.Eq("Address.PostalCode", postalCode);
        var documents = collection.Find(filter).ToList();

        return _processBsonDocuments(documents);
    }
    
    [HttpGet("LastName/{lastName}")]
    public IEnumerable<Patient> GetPatientsByLastName(string lastName)
    {
        var client = new MongoClient(ConnectionUri);
        var database = client.GetDatabase("ClientInfo");
        var collection = database.GetCollection<BsonDocument>("Patients");

        var filter = Builders<BsonDocument>.Filter.Eq("LastName", lastName);
        var documents = collection.Find(filter).ToList();

        return _processBsonDocuments(documents);
    }
}