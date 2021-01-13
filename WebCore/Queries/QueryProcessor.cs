using System;
using System.Threading.Tasks;

namespace WebCore.Queries
{
    public class QueryProcessor : IQueryProcessor
    {
        private readonly IServiceProvider _serviceProvider;
        public QueryProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResult> ProcessAsync<TQuery, TResult>(TQuery query)
            where TQuery : IQuery<TResult>
        {
            var service = _serviceProvider.GetService(typeof(IQueryHandler<TQuery, TResult>)) as IQueryHandler<TQuery, TResult>;
            return await service.HandleAsync(query);
        }
    }
}