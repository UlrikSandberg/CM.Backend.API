namespace CM.Backend.Persistence.UnitTests
{
    public class BrandRepository_Should
    {
        /*
        private Fixture _fixture = new Fixture();

        public BrandRepository_Should()
        {
            _fixture.Customize(new AutoMoqCustomization());
        }

        [Theory, AutoData]
        public async Task Insert_Brand(BrandData entity)
        {
            //Arrange
            entity.Id = ObjectId.Empty.ToString();
            var mongoDb = _fixture.Freeze<Mock<IMongoDatabase>>();
            var coll = new Mock<IMongoCollection<BrandData>>();

            mongoDb.Setup(x => x.GetCollection<BrandData>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(coll.Object);

            var sut = _fixture.Create<BrandRepository>();

            //Act
            await sut.Insert(entity);

            //Assert
            coll.Verify(x => x.InsertOneAsync(entity, It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory, AutoData]
        public async Task Update_Brand(BrandData entity)
        {
            //Arrange
            entity.Id = ObjectId.Empty.ToString();
            var mongoDb = _fixture.Freeze<Mock<IMongoDatabase>>();
            var coll = new Mock<IMongoCollection<BrandData>>();

            mongoDb.Setup(x => x.GetCollection<BrandData>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(coll.Object);
            coll.Setup(x => x.UpdateOneAsync(
                It.IsAny<FilterDefinition<BrandData>>(),
                It.IsAny<UpdateDefinition<BrandData>>(),
                It.IsAny<UpdateOptions>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(new UpdateResult.Acknowledged(1, 1, ObjectId.Empty));

            var sut = _fixture.Create<BrandRepository>();

            //Act
            var result = await sut.UpdateBrand(entity);

            //Assert
            result.ShouldBeTrue();
            coll.Verify(x => x.UpdateOneAsync(
                It.IsAny<FilterDefinition<BrandData>>(),
                It.IsAny<UpdateDefinition<BrandData>>(),
                It.IsAny<UpdateOptions>(),
                It.IsAny<CancellationToken>()), Times.Once());
        }

        [Theory, AutoData]
        public async Task Save_Published_Brand_State(string brandId)
        {
            //Arrange
            var mongoDb = _fixture.Freeze<Mock<IMongoDatabase>>();
            var coll = new Mock<IMongoCollection<BrandData>>();

            mongoDb.Setup(x => x.GetCollection<BrandData>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(coll.Object);
            coll.Setup(x => x.UpdateOneAsync(
                It.IsAny<FilterDefinition<BrandData>>(),
                It.IsAny<UpdateDefinition<BrandData>>(),
                It.IsAny<UpdateOptions>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(new UpdateResult.Acknowledged(1, 1, ObjectId.Empty));

            var sut = _fixture.Create<BrandRepository>();

            //Act
            var result = await sut.SetPublishingStatus(brandId, true);
            
            //Assert
            result.ShouldBeTrue();
            coll.Verify(x => x.UpdateOneAsync(
                It.IsAny<FilterDefinition<BrandData>>(),
                It.IsAny<UpdateDefinition<BrandData>>(),
                It.IsAny<UpdateOptions>(),
                It.IsAny<CancellationToken>()), Times.Once());        
        }
        */
    }
}