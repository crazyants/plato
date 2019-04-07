using System;
using System.Collections.Generic;
using System.Linq;
using Plato.Categories.Models;

namespace Plato.Categories.Extensions
{
    public static class CategoryExtensions
    {
        
        public static IEnumerable<TCategory> BuildHierarchy<TCategory>(this IEnumerable<ICategory> input) where TCategory : class, ICategory
        {
            return BuildHierarchyRecursively<TCategory>(input.ToLookup(c => c.ParentId));
        }

        public static IEnumerable<TCategory> RecurseParents<TCategory>(this IEnumerable<ICategory> input, int id) where TCategory : class, ICategory
        {
            return RecurseParentsInternal<TCategory>(input.ToList(), id);
        }

        public static IEnumerable<TCategory> RecurseChildren<TCategory>(this IEnumerable<ICategory> input, int id) where TCategory : class, ICategory
        {
            return RecurseChildrenInternal<TCategory>(input.ToList(), id);
        }

        private static IList<TCategory> BuildHierarchyRecursively<TCategory>(
           ILookup<int, ICategory> input,
           IList<TCategory> output = null,
           ICategory parent = null,
           int parentId = 0,
           int depth = 0) where TCategory : class, ICategory
        {

            if (input == null) throw new ArgumentNullException(nameof(input));
            if (output == null) output = new List<TCategory>();
            if (parentId == 0) depth = 0;

            foreach (var entity in input[parentId])
            {

                var item = (TCategory) entity;
                if (depth < 0) depth = 0;
                if (parent != null) depth++;

                item.Depth = depth;
                item.Parent = parent;

                if (parent != null)
                {
                    var children = new List<ICategory>() { item };
                    if (parent.Children != null)
                    {
                        children.AddRange(parent.Children);
                    }

                    parent.Children = children.OrderBy(c => c.SortOrder);
                }

                output.Add(item);

                // recurse
                BuildHierarchyRecursively<TCategory>(input, output, item, item.Id, depth--);
            }

            return output;

        }

        private static IEnumerable<TCategory> RecurseParentsInternal<TCategory>(
            IList<ICategory> input,
            int rootId,
            IList<TCategory> output = null) where TCategory : class, ICategory
        {
            if (output == null)
            {
                output = new List<TCategory>();
            }

            foreach (var entity in input)
            {
                var item = (TCategory) entity;
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

        private static IEnumerable<TCategory> RecurseChildrenInternal<TCategory>(
            IList<ICategory> input,
            int rootId,
            IList<TCategory> output = null) where TCategory : class, ICategory
        {

            if (output == null)
            {
                output = new List<TCategory>();
            }

            foreach (var entity in input)
            {
                var item = (TCategory) entity;
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
