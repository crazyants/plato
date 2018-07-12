//using System;
//using System.Threading.Tasks;
//using Plato.Discuss.Models;
//using Plato.Entities.Models;
//using Plato.Entities.Stores;
//using Plato.Internal.Abstractions;
//using Plato.Internal.Data.Abstractions;

//namespace Plato.Discuss.Stores
//{
//    public class TopicStore : IEntityStore<Topic>
//    {

//        private readonly IEntityStore<IEntity> _entityStore;

//        public TopicStore(IEntityStore<IEntity> entityStore)
//        {
//            _entityStore = entityStore;
//        }

//        public async Task<Topic> CreateAsync(Topic model)
//        {
//            var result = await _entityStore.CreateAsync(model);
//            return (Topic)result;
//        }

//        public async Task<Topic> UpdateAsync(Topic model)
//        {
//            var result =  await _entityStore.UpdateAsync(model);
//            return (Topic)result;
//        }

//        public async Task<bool> DeleteAsync(Topic model)
//        {
//            return await _entityStore.DeleteAsync(model);
//        }

//        public async Task<Topic> GetByIdAsync(int id)
//        {
//            var result = await _entityStore.GetByIdAsync(id);

//            return (Topic)result;
//        }

//        public IQuery<Topic> QueryAsync()
//        {
//            return (IQuery<Topic>) Convert.ChangeType(_entityStore.QueryAsync(), typeof(IQuery<Topic>));
//        }

//        public async Task<IPagedResults<Topic>> SelectAsync(params object[] args)
//        {
//            var result = await _entityStore.SelectAsync(args);

//            return (IPagedResults<Topic>)Convert.ChangeType(result, typeof(IPagedResults<Topic>));
//        }
//    }
//}
