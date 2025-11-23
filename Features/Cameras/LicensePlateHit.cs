namespace GRIDWATCH.Features.Cameras;

/// <summary>
/// Represents a license plate hit detected by the system.
/// </summary>
/// <param name="location">The geographical location where the license plate was detected.</param>
/// <param name="licensePlate">The detected license plate number.</param>
/// <param name="make">The make of the vehicle.</param>
/// <param name="model">The model of the vehicle.</param>
/// <param name="primaryColor">The primary color of the vehicle.</param>
/// <param name="secondaryColor">The secondary color of the vehicle.</param>
/// <param name="message">An additional message or note related to the license plate hit.</param>
public readonly struct LicensePlateHit(
    Vector3 location,
    string licensePlate,
    string make,
    string model,
    string primaryColor,
    string secondaryColor,
    string message)
{
    /// <summary>
    /// Gets the geographical location where the license plate was detected.
    /// </summary>
    public Vector3 Location { get; } = location;

    /// <summary>
    /// Gets the detected license plate number.
    /// </summary>
    public string LicensePlate { get; } = licensePlate;

    /// <summary>
    /// Gets the timestamp when the license plate hit was recorded.
    /// </summary>
    public DateTime Timestamp { get; } = DateTime.Now;

    /// <summary>
    /// Gets the make of the vehicle.
    /// </summary>
    public string Make { get; } = make;

    /// <summary>
    /// Gets the model of the vehicle.
    /// </summary>
    public string Model { get; } = model;

    /// <summary>
    /// Gets the primary color of the vehicle.
    /// </summary>
    public string PrimaryColor { get; } = primaryColor;

    /// <summary>
    /// Gets the secondary color of the vehicle.
    /// </summary>
    public string SecondaryColor { get; } = secondaryColor;

    /// <summary>
    /// Gets an additional message or note related to the license plate hit.
    /// </summary>
    internal string Message { get; } = message;
}
