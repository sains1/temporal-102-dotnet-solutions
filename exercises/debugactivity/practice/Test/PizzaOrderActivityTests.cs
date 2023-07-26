using Application;
using Application.Models;
using Temporalio.Testing;

namespace Test;

public class PizzaOrderActivityTests
{
    [Fact]
    public async Task TestGetDistanceTwoLineAddress()
    {
        var env = new ActivityEnvironment();
        var input = new Address
        {
            Line1 = "701 Mission Street",
            Line2 = "Apartment 9C",
            City = "San Francisco",
            State = "CA",
            PostalCode = "94103"
        };

        var result = await env.RunAsync(() => Activities.GetDistance(input));

        Assert.Equal(20, result.Kilometers);
    }

    [Fact]
    public async Task TestGetDistanceOneLineAddress()
    {
        var env = new ActivityEnvironment();
        var input = new Address
        {
            Line1 = "917 Delores Street",
            City = "San Francisco",
            State = "CA",
            PostalCode = "94103"
        };

        var result = await env.RunAsync(() => Activities.GetDistance(input));

        Assert.Equal(8, result.Kilometers);
    }

    [Fact]
    public async Task TestSendBillTypicalOrder()
    {
        var env = new ActivityEnvironment();
        var input = new Bill
        {
            CustomerId = 12983,
            OrderNumber = "PI314",
            Description = "2 large cheese pizzas",
            Amount = 2600 // amount does not qualify for discount
        };

        var result = await env.RunAsync(() => Activities.SendBill(input));

        Assert.Equal("PI314", result.OrderNumber);
        Assert.Equal(2600, result.Amount);
    }

    [Fact]
    public async Task TestSendBillFailsWithNegativeAmount()
    {
        var env = new ActivityEnvironment();

        var input = new Bill
        {
            CustomerId = 21974, OrderNumber = "OU812", Description = "1 large sausage pizza", Amount = -1000
        };

        Task<OrderConfirmation> Act() => env.RunAsync(() => Activities.SendBill(input));

        var exception = await Assert.ThrowsAsync<ArgumentException>(Act);
        Assert.Equal("invalid charge amount: -1000 (must be above zero)", exception.Message);
    }
}
