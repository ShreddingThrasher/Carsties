using AuctionService.Entities;

namespace AuctionService.UnitTests;

public class AuctionEntityTests
{
	// Naming convention - Method_Scenario_ExpectedResult
	
	[Fact]
	public void HasReservePrice_ReservePriceGreaterThanZero_True()
	{
		// Arrange
		var auction = new Auction() 
		{
			Id = Guid.NewGuid(),
			ReservePrice = 20
		};
		
		// Act
		var result = auction.HasReservePrice();
		
		// Assert
		Assert.True(result);
	}
	
	[Fact]
	public void HasReservePrice_ReservePriceIsZero_False()
	{
		// Arrange
		var auction = new Auction() 
		{
			Id = Guid.NewGuid(),
			ReservePrice = 0
		};
		
		// Act
		var result = auction.HasReservePrice();
		
		// Assert
		Assert.False(result);
	}
	
	[Fact]
	public void HasReservePrice_ReservePriceBelowZero_False()
	{
		// Arrange
		var auction = new Auction() 
		{
			Id = Guid.NewGuid(),
			ReservePrice = -50
		};
		
		// Act
		var result = auction.HasReservePrice();
		
		// Assert
		Assert.False(result);
	}
}