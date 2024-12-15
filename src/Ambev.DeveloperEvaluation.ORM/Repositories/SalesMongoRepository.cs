using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SalesMongoRepository : MongoRepository<Sale>
{
    public SalesMongoRepository(MongoContext context)
        : base(context, "Sales")
    {
    }
}