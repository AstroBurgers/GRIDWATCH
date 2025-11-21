namespace GRIDWATCH.Features.Cameras;

public struct LicensePlateHit(
    Vector3 location,
    string licensePlate,
    string make,
    string model,
    string primaryColor,
    string secondaryColor,
    string message)
{
    public Vector3 Location { get; } = location;
    public string LicensePlate { get; } = licensePlate;
    public DateTime Timestamp { get; } = DateTime.Now;
    public string Make { get; } = make;
    public string Model { get; } = model;
    public string PrimaryColor { get; } = primaryColor;
    public string SecondaryColor { get; } = secondaryColor;
    public string Message { get; } = message;
}