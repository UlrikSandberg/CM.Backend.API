using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using CM.Backend.Commands.Commands;
using CM.Backend.Commands.Handlers;
using CM.Backend.Commands.Responses;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Repositories;
using Moq;
using Shouldly;
using Xunit;

namespace CM.Backend.Commands.UnitTests
{
    public class BrandHandler_Should
    {
        private readonly Fixture _fixture = new Fixture();
        
        public BrandHandler_Should()
        {
            _fixture.Customize(new AutoMoqCustomization());
        }
        
        [Theory, AutoData]
        public async Task Create_Brand(CreateBrand message)
        {
            //Arrange
            var brandRepo = _fixture.Freeze<Mock<IBrandRepository>>();

            var sut = _fixture.Create<BrandHandler>();

            //Act
            var res = await sut.Handle(message);

            //Assert
            res.ShouldBeOfType<IdResponse>();
            brandRepo.Verify(r => r.Insert(It.Is<BrandData>(b => 
                b.BrandName == message.BrandName &&
                b.BrandCoverImageName == message.BrandCoverImageId &&
                b.BottleCoverImageName == message.BottleCoverImageId)), Times.Once);
        }
        
        [Theory, AutoData]
        public async Task Update_Brand(UpdateBasicBrandProfile message)
        {
            //Arrange
            var brandRepo = _fixture.Freeze<Mock<IBrandRepository>>();

            var sut = _fixture.Create<BrandHandler>();

            //Act
            await sut.Handle(message);

            //Assert
            brandRepo.Verify(r => r.UpdateBrand(It.Is<BrandData>(b => 
                b.Id == message.Id &&
                b.BrandName == message.BrandName &&
                b.BrandCoverImageName == message.BrandCoverImageId &&
                b.BottleCoverImageName == message.BottleCoverImageId)), Times.Once);
        }
        
        [Fact]
        public async Task Publish_Brand()
        {
            //Arrange
            var message = new SetBrandPublishingStatus(SetBrandPublishingStatus.PublishingStatusEnum.Published, _fixture.Create<string>());
            var brandRepo = _fixture.Freeze<Mock<IBrandRepository>>();

            var sut = _fixture.Create<BrandHandler>();

            //Act
            await sut.Handle(message);

            //Assert
            brandRepo.Verify(r => r.SetPublishingStatus(message.Id, true), Times.Once);
        }
        
        [Fact]
        public async Task Unpublish_Brand()
        {
            //Arrange
            var message = new SetBrandPublishingStatus(SetBrandPublishingStatus.PublishingStatusEnum.Unpublished, _fixture.Create<string>());
            var brandRepo = _fixture.Freeze<Mock<IBrandRepository>>();

            var sut = _fixture.Create<BrandHandler>();

            //Act
            await sut.Handle(message);

            //Assert
            brandRepo.Verify(r => r.SetPublishingStatus(message.Id, false), Times.Once);
        }
    }
}