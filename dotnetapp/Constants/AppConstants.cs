namespace dotnetapp.Constants
{
    public static class UserRoles
    {
        public const string Organizer = "Organizer";
        public const string Customer = "Customer";
    }

    public static class EventStatuses
    {
        public const string Active = "Active";
        public const string Cancelled = "Cancelled";
        public const string Completed = "Completed";
    }

    public static class BookingStatuses
    {
        public const string Pending = "Pending";
        public const string Confirmed = "Confirmed";
        public const string Cancelled = "Cancelled";
        public const string Completed = "Completed";
    }

    public static class PaymentStatuses
    {
        public const string Pending = "Pending";
        public const string Paid = "Paid";
        public const string Failed = "Failed";
        public const string Refunded = "Refunded";
    }
}