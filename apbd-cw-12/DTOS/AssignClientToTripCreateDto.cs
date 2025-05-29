using System.ComponentModel.DataAnnotations;

namespace apbd_cw_12.DTOS;

public record AssignClientToTripCreateDto
{
    [MaxLength(120), Required] public required string FirstName { get; init; } = null!;
    [MaxLength(120), Required] public required string LastName { get; init; } = null!;
    [MaxLength(120), Required] public required string Email { get; init; } = null!;
    [MaxLength(120), Required] public required string Telephone { get; init; } = null!;
    [MaxLength(120), Required] public required string Pesel { get; init; } = null!;
    [MaxLength(120)] public DateTime? PaymentDate { get; init; }
}