using Moq;
using MockQueryable.Moq;
using Microsoft.EntityFrameworkCore;

namespace Prom.LPR.Test.Database;

public static class DbContextMock 
{
    public static DbSet<T> GetQueryableMockDbSet<T>(List<T> sourceList) where T: class 
    {
        var addAction = (T s) => sourceList.Add(s);
        var addActionAsync = (T s, CancellationToken t) => { sourceList.Add(s); };

        var queryable = sourceList.AsQueryable().BuildMock();
        var dbSet = new Mock<DbSet<T>>();

        dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);

        dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback(addAction);

        dbSet.Setup(d => d.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .Callback(addActionAsync);
            //.Returns((T model, CancellationToken token) => Task.FromResult((T) null!));

        return dbSet.Object;
    }
 }
