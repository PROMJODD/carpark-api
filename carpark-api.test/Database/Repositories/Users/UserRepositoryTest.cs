using Moq;
using Xunit;
using Prom.LPR.Api.Database;
using Prom.LPR.Api.Models;
using Prom.LPR.Api.Database.Repositories;
using System.Data.Common;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Prom.LPR.Test.Database.Repositories;

public class UserRepositoryTest
{
    private void CreateUser(List<MUser> list, string name, string email)
    {
        var r = new MUser
        {
            UserName = name,
            UserEmail = email
        };

        list.Add(r);
    }

    [Fact]
    public void GetUsersSuccess()
    {
        var users = new List<MUser>();
        CreateUser(users, "user1", "email1@yyy.com");
        CreateUser(users, "user2", "email2@yyy.com");
        CreateUser(users, "user3", "email3@yyy.com");

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.Users).Returns(DbContextMock.GetQueryableMockDbSet(users));

        var repo = new UserRepository(ctxMock.Object);

        var list = repo.GetUsers();
        var cnt = list.Count();

        Assert.Equal(3, cnt);
    }

    [Fact]
    public void GetUsersWithException()
    {
        var users = new List<MUser>();
        CreateUser(users, null!, "email1@yyy.com");

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.Users).Returns(DbContextMock.GetQueryableMockDbSet(users));

        var repo = new UserRepository(ctxMock.Object);

        Action act = () => repo.GetUsers();
        var ex = Record.Exception(act);
        Assert.NotNull(ex);
    }

    //#### AddUser ####
    [Fact]
    public void AddUsersSuccess()
    {
        var users = new List<MUser>();
        CreateUser(users, "user1", "email1@yyy.com");
        CreateUser(users, "user2", "email2@yyy.com");
        CreateUser(users, "user3", "email3@yyy.com");

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.Users).Returns(DbContextMock.GetQueryableMockDbSet(users));

        var repo = new UserRepository(ctxMock.Object);

        var u = new MUser() { UserName = "fake", UserEmail = "fake" };
        var retUser = repo.AddUser(u);
        var cnt = users.Count();

        Assert.Equal(4, cnt);
        Assert.Equal(u.UserName, retUser.UserName);
    }

    [Fact]
    public void AddUsersWithException()
    {
        var users = new List<MUser>();

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.Users).Returns(DbContextMock.GetQueryableMockDbSet(users));

        var repo = new UserRepository(ctxMock.Object);

        Action act = () => repo.AddUser(null!);
        var ex = Record.Exception(act);
        Assert.NotNull(ex);
    }

    //#### IsEmailExist ####
    [Theory]
    [InlineData("email1@yyy.com", true)]
    [InlineData("email2@yyy.com", true)]
    [InlineData("email3", false)]
    public void IsEmailExistTest(string email, bool isExist)
    {
        var users = new List<MUser>();
        CreateUser(users, "user1", "email1@yyy.com");
        CreateUser(users, "user2", "email2@yyy.com");

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.Users).Returns(DbContextMock.GetQueryableMockDbSet(users));

        var repo = new UserRepository(ctxMock.Object);

        var found = repo.IsEmailExist(email);

        Assert.Equal(isExist, found);
    }

    [Fact]
    public void IsEmailExistWithException()
    {
        var users = new List<MUser>();
        CreateUser(users, "user1", null!);
        CreateUser(users, "user2", "email2@yyy.com");

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.Users).Returns(DbContextMock.GetQueryableMockDbSet(users));

        var repo = new UserRepository(ctxMock.Object);

        Action act = () => repo.IsEmailExist("fake-email");
        var ex = Record.Exception(act);
        Assert.NotNull(ex);
    }

    //#### IsUserNameExist ####
    [Theory]
    [InlineData("user1", true)]
    [InlineData("user2", true)]
    [InlineData("user3", false)]
    public void IsUserNameExistTest(string userName, bool isExist)
    {
        var users = new List<MUser>();
        CreateUser(users, "user1", "email1@yyy.com");
        CreateUser(users, "user2", "email2@yyy.com");

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.Users).Returns(DbContextMock.GetQueryableMockDbSet(users));

        var repo = new UserRepository(ctxMock.Object);

        var found = repo.IsUserNameExist(userName);

        Assert.Equal(isExist, found);
    }

    [Fact]
    public void IsUserNameExistWithException()
    {
        var users = new List<MUser>();
        CreateUser(users, null!, "email1@yyy.com");
        CreateUser(users, "user2", "email2@yyy.com");

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.Users).Returns(DbContextMock.GetQueryableMockDbSet(users));

        var repo = new UserRepository(ctxMock.Object);

        Action act = () => repo.IsUserNameExist("fake-username");
        var ex = Record.Exception(act);
        Assert.NotNull(ex);
    }

    //#### IsUserIdExist ####
    [Fact]
    public void IsUserIdExistTest()
    {
        var users = new List<MUser>();
        CreateUser(users, "user1", "email1@yyy.com");
        CreateUser(users, "user2", "email2@yyy.com");
        var id = users[0].UserId;

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.Users).Returns(DbContextMock.GetQueryableMockDbSet(users));

        var repo = new UserRepository(ctxMock.Object);

        var found = repo.IsUserIdExist(id.ToString()!);

        Assert.True(found);
    }

    [Fact]
    public void IsUserIdExistWithException()
    {
        var users = new List<MUser>();

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.Users).Returns(DbContextMock.GetQueryableMockDbSet(users));

        var repo = new UserRepository(ctxMock.Object);
        var found = repo.IsUserIdExist("fake-user-id");

        Assert.False(found);
    }

    //#### GetUserByName ####
    [Theory]
    [InlineData("user1", true)]
    [InlineData("user2", true)]
    [InlineData("user3", false)]
    public void GetUserByNameTest(string user, bool found)
    {
        var users = new List<MUser>();
        CreateUser(users, "user1", "email1@yyy.com");
        CreateUser(users, "user2", "email2@yyy.com");

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.Users).Returns(DbContextMock.GetQueryableMockDbSet(users));

        var repo = new UserRepository(ctxMock.Object);

        var u = repo.GetUserByName(user);
        var isFound = u != null;

        Assert.Equal(found, isFound);
    }

    [Fact]
    public void GetUserByNameWithException()
    {
        var users = new List<MUser>();
        CreateUser(users, null!, "email1@yyy.com");
        CreateUser(users, "user2", "email2@yyy.com");

        var ctxMock = new Mock<IDataContext>();
        ctxMock.Setup(x => x.Users).Returns(DbContextMock.GetQueryableMockDbSet(users));

        var repo = new UserRepository(ctxMock.Object);

        Action act = () => repo.GetUserByName("user2");
        var ex = Record.Exception(act);
        Assert.NotNull(ex);
    }
}
