using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Entities.Models;

namespace Plato.Discuss.Stores
{

    public interface ITopicStore<T> : IEntityStore<T> where T : class
    {

    }

    public class TopicStore : ITopicStore<Topic>
    {

        private readonly IEntityStore<Topic> _entityStore;

        public TopicStore(IEntityStore<Topic> entityStore)
        {
            _entityStore = entityStore;
        }

        public async Task<Topic> CreateAsync(Topic model)
        {
            
            var topicDetails = new TopicDetails()
            {
                Participants = new List<Participant>()
                {
                    new Participant()
                    {
                        UserId = 1,
                        UserName = "Test",
                        Participations = 10
                        
                    },
                    new Participant()
                    {
                        UserId = 2,
                        UserName = "Mike Jones",
                        Participations = 5
                    },
                    new Participant()
                    {
                        UserId = 3,
                        UserName = "Sarah Smith",
                        Participations = 2
                    }
                }
            };


            model.Data = new List<EntityData>()
            {
                new EntityData()
                {
                    Key = "ToicDetails",
                    Value = topicDetails.Serialize()
                }
            };
            
            return await _entityStore.CreateAsync(model);
            
        }

        public Task<bool> DeleteAsync(Topic model)
        {
            throw new NotImplementedException();
        }

        public Task<Topic> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public IQuery QueryAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IPagedResults<T>> SelectAsync<T>(params object[] args) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<Topic> UpdateAsync(Topic model)
        {
            throw new NotImplementedException();
        }
    }
}
