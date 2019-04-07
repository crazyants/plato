using System;
using System.Collections.Generic;
using System.Linq;
using Plato.Entities.Models;

namespace Plato.Entities.Extensions
{
    public static class EntityExtensions
    {

        public static bool IsHidden(this IEntity entity)
        {

            if (entity.IsPrivate)
            {
                return true;
            }

            if (entity.IsDeleted)
            {
                return true;
            }

            if (entity.IsSpam)
            {
                return true;
            }

            return false;

        }

        public static IEnumerable<TEntity> BuildHierarchy<TEntity>(this IEnumerable<IEntity> input) where TEntity : class, IEntity
        {
            return BuildHierarchyRecursively<TEntity>(input.ToLookup(c => c.ParentId));
        }

        public static IEnumerable<TEntity> RecurseParents<TEntity>(this IEnumerable<IEntity> input, int id) where TEntity : class, IEntity
        {
            return RecurseParentsInternal<TEntity>(input.ToList(), id);
        }

        public static IEnumerable<TEntity> RecurseChildren<TEntity>(this IEnumerable<IEntity> input, int id) where TEntity : class, IEntity
        {
            return RecurseChildrenInternal<TEntity>(input.ToList(), id);
        }

        private static IList<TEntity> BuildHierarchyRecursively<TEntity>(
           ILookup<int, IEntity> input,
           IList<TEntity> output = null,
           IEntity parent = null,
           int parentId = 0,
           int depth = 0) where TEntity : class, IEntity
        {

            if (input == null) throw new ArgumentNullException(nameof(input));
            if (output == null) output = new List<TEntity>();
            if (parentId == 0) depth = 0;

            foreach (var entity in input[parentId])
            {

                var item = (TEntity) entity;
                if (depth < 0) depth = 0;
                if (parent != null) depth++;

                item.Depth = depth;
                item.Parent = parent;

                if (parent != null)
                {
                    var children = new List<IEntity>() { item };
                    if (parent.Children != null)
                    {
                        children.AddRange(parent.Children);
                    }

                    parent.Children = children.OrderBy(c => c.SortOrder);
                }

                output.Add(item);

                // recurse
                BuildHierarchyRecursively<TEntity>(input, output, item, item.Id, depth--);
            }

            return output;

        }

        private static IEnumerable<TEntity> RecurseParentsInternal<TEntity>(
            IList<IEntity> input,
            int rootId,
            IList<TEntity> output = null) where TEntity : class, IEntity
        {
            if (output == null)
            {
                output = new List<TEntity>();
            }

            foreach (var entity in input)
            {
                var item = (TEntity) entity;
                if (item.Id == rootId)
                {
                    if (item.ParentId > 0)
                    {
                        output.Add(item);
                        RecurseParentsInternal(input, item.ParentId, output);
                    }
                    else
                    {
                        output.Add(item);
                    }
                }
            }

            return output;

        }

        private static IEnumerable<TEntity> RecurseChildrenInternal<TEntity>(
            IList<IEntity> input,
            int rootId,
            IList<TEntity> output = null) where TEntity : class, IEntity
        {

            if (output == null)
            {
                output = new List<TEntity>();
            }

            foreach (var entity in input)
            {
                var item = (TEntity) entity;
                if (item.ParentId == rootId)
                {
                    output.Add(item);
                    RecurseChildrenInternal(input, item.Id, output);
                }
            }

            return output;

        }

    }

}
