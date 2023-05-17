namespace Booking.Booking.Domain;

public class ConferenceCentre
{
    public virtual ICollection<Room> Rooms { get; set; }
    public string Name { get; set; }
}