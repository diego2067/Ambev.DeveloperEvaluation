using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SalesMongoRepository : MongoRepository<SaleMongo>
{
    public SalesMongoRepository(MongoContext context, IOptions<MongoSettings> settings)
        : base(context, settings)
    {
    }
}