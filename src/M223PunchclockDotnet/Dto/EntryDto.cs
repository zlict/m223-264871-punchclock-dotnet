using System.ComponentModel.DataAnnotations;

namespace M223PunchclockDotnet.Dto;

public class EntryDto : IValidatableObject
{
    // Serialized in responses; System.Text.Json ignores the internal setter in requests.
    public int Id { get; internal set; }

    [Required(ErrorMessage = "Check-in is required.")]
    public DateTime? CheckIn { get; set; }

    [Required(ErrorMessage = "Check-out is required.")]
    public DateTime? CheckOut { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Required handles missing values before object-level validation.
        if (CheckIn is not DateTime checkIn || CheckOut is not DateTime checkOut)
        {
            yield break;
        }

        if (checkIn.Kind != DateTimeKind.Utc)
        {
            yield return new ValidationResult("Check-in must be a UTC timestamp (Z).", [nameof(CheckIn)]);
        }

        if (checkOut.Kind != DateTimeKind.Utc)
        {
            yield return new ValidationResult("Check-out must be a UTC timestamp (Z).", [nameof(CheckOut)]);
        }

        if (checkIn >= checkOut)
        {
            yield return new ValidationResult("Check-in must be before check-out.", [nameof(CheckIn), nameof(CheckOut)]);
        }
    }
}
