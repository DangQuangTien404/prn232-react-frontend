namespace RentalPropertyManagement.DAL.Enums
{
    public enum UserRole
    {
        Landlord = 1,
        Tenant = 2,
        ServiceProvider = 3
    }

    public enum ContractStatus
    {
        Pending = 1,
        Active = 2,
        Expired = 3,
        Terminated = 4
    }

    public enum RequestPriority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Urgent = 4
    }

    public enum RequestStatus
    {
        New = 1,
        Approved = 2,
        Rejected = 3,
        InProgress = 4,
        Completed = 5
    }
}