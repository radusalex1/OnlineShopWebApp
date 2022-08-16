using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using OnlineShopWebApp.Controllers;
using OnlineShopWebApp.DataModels;
using OnlineShopWebApp.Repositories;

namespace OnlineShopWebAppTests
{
    [TestFixture]
    public class ClientControllerTests
    {
        private List<Client> _clients;
        private List<Gender> _genders;
        private ClientsController _clientController;
        private Mock<IClientRepository> _mockClientRepository;
        private Mock<IGenderRepository> _mockGenderRepository;

        [SetUp]
        public void Setup()
        {
            _clients = new()
            {
                new Client() {
                    Id = 1,
                    Name = "Radu",
                    Street = "Street",
                    City ="Brasov",
                    Country ="Romania",
                    PhoneNumber = "0725342567",
                    GenderId = 1
                }
            };

            _genders = new()
            {
                new Gender() { Id = 1, GenderType = "Male"},
                new Gender() { Id = 2, GenderType ="Female"}
            };

            _mockClientRepository = new Mock<IClientRepository>();
            _mockGenderRepository = new Mock<IGenderRepository>();
            _clientController = new ClientsController(_mockClientRepository.Object, _mockGenderRepository.Object);
        }

        [TearDown]
        public void TearDown()
        {
            //_clientController = null;
            _mockClientRepository.Reset();
            _mockGenderRepository.Reset();
        }

        [Test]
        public async Task GetClients_ShouldPass_WhenCallingIndexMethod()
        {
            //arrange
            _mockClientRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(_clients));

            //act
            var actionResult = await _clientController.Index();

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task GetClient_ShouldPass_WhenCallingByInvalidId()
        {
            Client? client = null;

            //arrange
            _mockClientRepository.Setup(m => m.Get(It.IsAny<int>())).Returns(Task.FromResult(client));

            //act
            var actionResult = await _clientController.Details(2);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task GetClient_ShouldPass_WhenCallingByValidId()
        {
            Client? client = _clients[0];
            //arrange
            _mockClientRepository.Setup(m => m.Get(It.IsAny<int>())).Returns(Task.FromResult(client));

            //act
            var actionResult = await _clientController.Details(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task GetClient_ShouldPass_WhenCallingByNullId()
        {
            //act
            var actionResult = await _clientController.Details(null);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task CreateClient_ShouldPass_WhenCreatingValidClient()
        {
            //arrange
            Client client = _clients[0];

            _mockClientRepository.Setup(m => m.Add(It.IsAny<Client>())).Returns(Task.FromResult(true));
            _mockClientRepository.Setup(n => n.IfExists(It.IsAny<string>())).Returns(Task.FromResult(false));

            //act
            var actionResult = await _clientController.Create(client);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public async Task CreateClient_ShouldPass_WhenCreatingInvalidClient()
        {
            //arrange
            Client client = _clients[0];
            
            _mockClientRepository.Setup(n => n.IfExists(It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockGenderRepository.Setup(n => n.Get(It.IsAny<int>())).Returns(Task.FromResult(_genders[0]));
            _mockGenderRepository.Setup(n => n.GetAll()).Returns(Task.FromResult(_genders));


            //act
            var actionResult = await _clientController.Create(client);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task EditClient_ShouldPass_WhenCallingByNullId()
        {
            //act
            var actionResult = await _clientController.Edit(null);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task EditClient_ShouldPass_WhenCallingByInvalidId()
        {
            Client? client = null;
            //arrange
            _mockClientRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(client));

            //act
            var actionResult = await _clientController.Edit(2);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task EditClient_ShouldPass_WhenCallingByValidId()
        {
            Client? client = _clients[0];
            //arrange
            _mockClientRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(client));
            _mockGenderRepository.Setup(n => n.GetAll()).Returns(Task.FromResult(_genders));

            //act
            var actionResult = await _clientController.Edit(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task EditClientPost_ShouldPass_WhenCallingByInvalidId()
        {
            Client? client = _clients[0];

            //act
            var actionResult = await _clientController.Edit(2, client);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task EditClientPost_ShouldPass_WhenCallingWithInvalidClient()
        {
            //arrange
            Client? client = _clients[0];
            client.Id = 2;

            _mockClientRepository.Setup(m => m.Update(It.IsAny<Client>())).Throws<DbUpdateConcurrencyException>();
            _mockClientRepository.Setup(m => m.IfExists(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(true));

            //act
            var actionResult = await _clientController.Edit(2, client);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public void EditClientPost_ShouldPass_WhenThrowException()
        {
            //arrange
            Client? client = _clients[0];

            _mockClientRepository.Setup(m => m.Update(It.IsAny<Client>())).Throws<DbUpdateConcurrencyException>();
            _mockClientRepository.Setup(m => m.IfExists(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(true));
            _mockClientRepository.Setup(m => m.IfExists(It.IsAny<int>())).Returns(Task.FromResult(true));

            //assert
            Assert.That(async () => await _clientController.Edit(1, client), Throws.TypeOf<DbUpdateConcurrencyException>());
        }

        [Test]
        public async Task EditClientPost_ShouldPass_WhenEditValidClient()
        {
            //arrange
            Client? client = _clients[0];

            _mockClientRepository.Setup(m => m.Update(It.IsAny<Client>())).Returns(Task.FromResult(true));
            _mockClientRepository.Setup(m => m.IfExists(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(true));

            //act
            var actionResult = await _clientController.Edit(1, client);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public async Task EditClient_ShouldPass_WhenPassingInvalidClient()
        {
            //arrange
            Client? client = _clients[0];
            client.Id = 2;

            _mockClientRepository.Setup(m => m.IfExists(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(false));
            _mockGenderRepository.Setup(n => n.GetAll()).Returns(Task.FromResult(_genders));

            //act
            var actionResult = await _clientController.Edit(2, client);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task DeleteClientPage_ShouldPass_WhenPassingNullId()
        {
            //act
            var actionResult = await _clientController.Delete(null);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task DeleteClientPage_ShouldPass_WhenPassingInvalidId()
        {
            Client? client = null;

            //arrange
            _mockClientRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(client));

            //act
            var actionResult = await _clientController.Delete(2);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteClientPage_ShouldPass_WhenPassingValidId()
        {
            //arrange
            Client? client = _clients[0];

            _mockClientRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(client));

            //act
            var actionResult = await _clientController.Delete(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task DeleteClientPage_ShouldPass_WhenThereAreNoClients()
        {
            //arrange
            List<Client> clients = null;

            _mockClientRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(clients));

            //act
            var actionResult = await _clientController.DeleteConfirmed(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
        }

        [Test]
        public async Task DeleteClientPage_ShouldPass_WhenCallingDeletePostMethod()
        {
            //arrange
            _mockClientRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(_clients));

            //act
            var actionResult = await _clientController.DeleteConfirmed(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public async Task ClientExists_ShoudPass_WhenThereAreDuplicates()
        {
            //arrange
            _mockClientRepository.Setup(m => m.IfExists(It.IsAny<int>())).Returns(Task.FromResult(true));

            //act
            var actionResult = await _clientController.ClientExists(1);

            //assert
            Assert.That(actionResult, Is.EqualTo(true));
        }

        [Test]
        public async Task ClientExists_ShoudPass_WhenThereAreNoDuplicates()
        {
            //arrange
            _mockClientRepository.Setup(m => m.IfExists(It.IsAny<string>())).Returns(Task.FromResult(false));

            //act
            var actionResult = await _clientController.ClientExists(1);

            //assert
            Assert.That(actionResult, Is.EqualTo(false));
        }
    }
}
