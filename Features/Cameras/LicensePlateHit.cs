namespace GRIDWATCH.Features.Cameras;

internal struct LicensePlateHit
{
    internal Vector3 Location { get; }
    internal string LicensePlate { get; }
    internal DateTime Timestamp { get; }
    internal string Make { get; }
    internal string Model { get; }
    internal string PrimaryColor { get; }
    internal string SecondaryColor { get; }
    internal string Message { get; }
    
    internal LicensePlateHit(Vector3 location, string licensePlate, string make, string model, string primaryColor, string secondaryColor, string message)
    {
        Location = location;
        LicensePlate = licensePlate;
        Timestamp = DateTime.Now;
        Make = make;
        Model = model;
        PrimaryColor = primaryColor;
        SecondaryColor = secondaryColor;
        Message = message;
    }
}