using Orleans;
using Orleans.EventSourcing;
using System;
using System.Threading.Tasks;

//https://www.eventstore.com/blog/event-store-orleans

namespace Actor.Contract
{
    public class PickedUp
    {
        public PickedUp(DateTime dateTime)
        {
            DateTime = dateTime;
        }
        public DateTime DateTime { get; }
    }

    public class Delivered
    {
        public Delivered(DateTime dateTime)
        {
            DateTime = dateTime;
        }
        public DateTime DateTime { get; }
    }

    public enum TransitStatus
    {
        AwaitingPickup,
        InTransit,
        Delivered
    }

    public class ShipmentState
    {
        public TransitStatus Status { get; set; } = TransitStatus.AwaitingPickup;
        public DateTime? LastEventDateTime { get; set; }

        public ShipmentState Apply(PickedUp pickedUpEvent)
        {
            Status = TransitStatus.InTransit;
            LastEventDateTime = pickedUpEvent.DateTime;
            return this;
        }

        public ShipmentState Apply(Delivered delivered)
        {
            Status = TransitStatus.Delivered;
            LastEventDateTime = delivered.DateTime;
            return this;
        }
    }

    public interface IShipment : IGrainWithGuidKey
    {
        Task Pickup();
        Task Deliver();
        Task<TransitStatus> GetStatus();
    }

    public class Shipment : JournaledGrain<ShipmentState>, IShipment
    {
        public Task Pickup()
        {
            if (State.Status == TransitStatus.InTransit)
            {
                throw new InvalidOperationException("Shipment has already been picked up.");
            }

            if (State.Status == TransitStatus.Delivered)
            {
                throw new InvalidOperationException("Shipment has already been delivered.");
            }

            RaiseEvent(new PickedUp(DateTime.UtcNow));
            return ConfirmEvents();
        }

        public Task<TransitStatus> GetStatus()
        {
            Console.WriteLine($"Version: {Version}");
            return Task.FromResult(State.Status);
        }

        public Task Deliver()
        {
            if (State.Status == TransitStatus.AwaitingPickup)
            {
                throw new InvalidOperationException("Shipment has not yet been picked up.");
            }

            if (State.Status == TransitStatus.Delivered)
            {
                throw new InvalidOperationException("Shipment has already been delivered.");
            }

            RaiseEvent(new Delivered(DateTime.UtcNow));
            return ConfirmEvents();
        }
    }
}