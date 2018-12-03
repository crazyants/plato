using System;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Tags.Models;

namespace Plato.Tags.Stores
{

    public class TagsStore : ITagsStore<Tag>
    {
        public IQuery<Tag> QueryAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IPagedResults<Tag>> SelectAsync(params object[] args)
        {
            throw new NotImplementedException();
        }

        public Task<Tag> CreateAsync(Tag model)
        {
            throw new NotImplementedException();
        }

        public Task<Tag> UpdateAsync(Tag model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Tag model)
        {
            throw new NotImplementedException();
        }

        public Task<Tag> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
