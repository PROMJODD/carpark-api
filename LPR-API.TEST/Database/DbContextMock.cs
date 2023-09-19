using Microsoft.EntityFrameworkCore;
using Moq;

namespace Prom.LPR.Test.Database;

public static class DbContextMock 
{
    public static DbSet<T> GetQueryableMockDbSet<T>(List<T> sourceList) where T: class 
    {
        var queryable = sourceList.AsQueryable();
        var dbSet = new Mock<DbSet<T>>();
    
        dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);

        dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((s) => sourceList.Add(s));
        
        return dbSet.Object;
    }
 }
