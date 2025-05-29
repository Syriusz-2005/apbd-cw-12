using apbd_cw_12.DTOS;
using apbd_cw_12.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace apbd_cw_12.Services;

public interface IDbService
{
    public Task<TripsGetDto> GetTripsAsync(int page, int pageSize);
    public Task DeleteClientAsync(int idClient);
    public Task AssignClientToTripAsync(AssignClientToTripCreateDto clientToTripCreateDto, int idTrip);
}

public class DbService(AppDbContext dbContext) : IDbService
{
    public async Task<TripsGetDto> GetTripsAsync(int page, int pageSize)
    {
        var trips = await dbContext.Trips
            .OrderByDescending((trip) => trip.DateFrom)
            .Select(trip => new TripGetDto
            {
                Name = trip.Name,
                Clients = trip.ClientTrips
                    .Select(clientTrip => new TripClientGetDto(clientTrip.Client.FirstName, clientTrip.Client.LastName))
                    .ToList(),
                Countries = trip.Countries
                    .Select(country => new TripCountryGetDto(country.Name))
                    .ToList(),
                Description = trip.Description,
                DateFrom = trip.DateFrom,
                DateTo = trip.DateTo,
                MaxPeople = trip.MaxPeople,
            })
            .Skip(pageSize * page)
            .Take(pageSize)
            .ToListAsync();

        var tripsCount = trips.Count;
        
        return new TripsGetDto
        {
            Trips = trips,
            PageNum = page,
            PageSize = pageSize,
            AllPages = tripsCount / pageSize + 1,
        };
    }

    public async Task DeleteClientAsync(int idClient)
    {
        var tripsCount = await dbContext.ClientTrips
            .Where(clientTrip => clientTrip.IdClient == idClient)
            .CountAsync();
        if (tripsCount != 0)
        {
            throw new ConflictException("Cannot remove client: client is assigned to one or more trips!");
        }
        var deletedCount = await dbContext.Clients
            .Where(client => client.IdClient == idClient)
            .ExecuteDeleteAsync();
        if (deletedCount == 0) throw new NotFoundException("Client not found");
    }

    public async Task AssignClientToTripAsync(AssignClientToTripCreateDto clientToTripCreateDto, int idTrip)
    {
        var clientsWithPesel = await dbContext.Clients
            .Where(client => client.Pesel == clientToTripCreateDto.Pesel)
            .CountAsync();
        if (clientsWithPesel > 0)
        {
            throw new ConflictException("Client with this PESEL already exists!");
        }

        var clientsSavedInThisTripCount = await dbContext.ClientTrips
            .Where(clientTrip => clientTrip.Client.Pesel == clientToTripCreateDto.Pesel && clientTrip.IdTrip == idTrip)
            .CountAsync();
        // This will never happen, because the previous condition guarantees that
        if (clientsSavedInThisTripCount > 0)
        {
            throw new ConflictException("Client with this PESEL is already saved on this trip!");
        }
        
        var trip = await dbContext.Trips
            .Where(trip => trip.IdTrip == idTrip)
            .FirstAsync();
        if (trip is null)
        {
            throw new NotFoundException("Trip does not exist!");
        }

        if (trip.DateFrom > DateTime.Now)
        {
            throw new ConflictException("Trip date is in the future!");
        }

        dbContext.Clients.Add(new Client
        {
            Email = clientToTripCreateDto.Email,
            Pesel = clientToTripCreateDto.Pesel,
            FirstName = clientToTripCreateDto.FirstName,
            LastName = clientToTripCreateDto.LastName,
            Telephone = clientToTripCreateDto.Telephone,
            ClientTrips = [
                new ClientTrip
                {
                    PaymentDate = clientToTripCreateDto.PaymentDate,
                    IdTrip = idTrip,
                    RegisteredAt = DateTime.Now
                }
            ]
        });

        await dbContext.SaveChangesAsync();
    }
}