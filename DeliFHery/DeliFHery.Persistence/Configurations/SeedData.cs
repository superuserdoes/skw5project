namespace DeliFHery.Persistence.Configurations;

internal static class SeedData
{
    public static readonly Guid OrchardMarketCustomerId = Guid.Parse("5a6b9e21-7c74-4d3b-8a6a-1ef65c94af10");
    public static readonly Guid CityBakeryCustomerId = Guid.Parse("dbcd7f5b-7e9e-4c8f-9a2c-6f2b2e4c60f7");

    public static readonly Guid OrchardMorningDeliveryId = Guid.Parse("95f44e9d-1c5a-4378-8f43-646f7414a87d");
    public static readonly Guid CityLunchDeliveryId = Guid.Parse("d882e388-611d-49c2-9d2e-5a8bf9fa33e5");

    public static readonly Guid OrchardPrimaryContactId = Guid.Parse("6ed5825d-45df-4f23-9bd7-dfa48bb3cdb8");
    public static readonly Guid OrchardBackupContactId = Guid.Parse("85fe89b7-8ac8-42ba-9013-38f5fe403fb0");
    public static readonly Guid CityPrimaryContactId = Guid.Parse("df1a0a3c-92fd-4f0c-a341-1af7b7f33d72");

    public static readonly DateTimeOffset OrchardMorningSchedule = new(2024, 4, 10, 7, 30, 0, TimeSpan.Zero);
    public static readonly DateTimeOffset CityLunchSchedule = new(2024, 4, 10, 11, 30, 0, TimeSpan.Zero);
}
