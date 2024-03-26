using AuctionService.Controllers;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.RequestHelpers;
using AutoFixture;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AuctionService.UnitTests
{
	public class AuctionControllerTests
	{
		private readonly Mock<IAuctionRepository> _auctionRepo;
		private readonly Mock<IPublishEndpoint> _publishEndpoint;
		private readonly Fixture _fixture;
		private readonly AuctionsController _controller;
		private readonly IMapper _mapper;
		
		public AuctionControllerTests()
		{
			_fixture = new Fixture();
			_auctionRepo = new Mock<IAuctionRepository>();
			_publishEndpoint = new Mock<IPublishEndpoint>();
			
			var mockMapper = new MapperConfiguration(mc => 
			{
				mc.AddMaps(typeof(MappingProfiles).Assembly);
			}).CreateMapper().ConfigurationProvider;
			
			_mapper = new Mapper(mockMapper);
			
			_controller = new AuctionsController(_auctionRepo.Object, _mapper, _publishEndpoint.Object);
		}
		
		[Fact]
		public async Task GetAuctions_WithNoParams_Returns10Auctions()
		{
			// arrange
			var auctions = _fixture.CreateMany<AuctionDto>(10).ToList();
			
			_auctionRepo.Setup(repo => repo.GetAuctionsAsync(null))
				.ReturnsAsync(auctions);
				
			// act
			var result = await _controller.GetAllAuctions(null);
			
			// assert
			Assert.Equal(10, result.Value.Count);
			Assert.IsType<ActionResult<List<AuctionDto>>>(result);
		}
	}
}