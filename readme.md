# Truck Service

_Truck Service_ is a WebAPI developed using C# (.NET 7)

## Prerequisites: **database**

You need a running instance of _Azure Cosmos DB_.

Then make sure to provide one of the following in the settings:

```json
{
  "CosmosDatabase": {
    "ConnectionString": "",
    "AccountEndpoint":  ""
  }
}
```

## Prerequisites: **auth**.

The app requires the users to be authenticated/authorized, so you will need a valid user in my _Azure AD Tenant_ 🤓

## Usage

Open the _.sln_ in your favourite IDE and hit run  
OR use `dotnet run` in the _Api_ project.

If Swagger doesn't open automatically, navigate to `https://localhost:58781/swagger`

## Cool libraries used

- `AutoFixture.AutoMoq`
- `AutoFixture.Xunit2`
- `FluentAssertions`
