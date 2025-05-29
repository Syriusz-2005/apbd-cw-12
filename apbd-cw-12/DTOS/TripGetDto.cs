namespace apbd_cw_12.DTOS;

public record TripCountryGetDto(string Name);

public record TripClientGetDto(string FirstName, string LastName);

public record TripGetDto
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required DateTime DateFrom { get; init; }
    public required DateTime DateTo { get; init; }
    public required int MaxPeople { get; init; }
    public required ICollection<TripCountryGetDto> Countries { get; init; }
    public required ICollection<TripClientGetDto> Clients { get; init; }
};