using System.Text.Json.Serialization;
using OrderManagement.Domain;

namespace OrderManagement.Api.Dtos;

public class CustomerDto
{
    [JsonRequired]
    public Guid Id { get; set; }

    [JsonRequired]
    public string Name { get; set; }

    [JsonRequired]
    public int ZipCode { get; set; }

    [JsonRequired]
    [JsonPropertyName("location")]
    public string City { get; set; }

    [JsonRequired]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Rating Rating { get; set; }
    
    [JsonIgnore]
    public decimal TotalRevenue { get; set; }
}