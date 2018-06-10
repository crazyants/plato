using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Plato.Layout.ViewProviders;
using Plato.Layout.Views;

namespace Plato.Layout
{
    public class LayoutViewProviderResult : CombinedViewProviderResult
    {
        private readonly IEnumerable<IViewProviderResult> _results;
        private readonly ConcurrentDictionary<string, IList<IPositionedView>> _zonedViews =
            new ConcurrentDictionary<string, IList<IPositionedView>>();
        
        public IEnumerable<IView> Header { get; set; }

        public IEnumerable<IView> Meta { get; set; }

        public IEnumerable<IView> Content { get; set; }
        
        public IEnumerable<IView> SideBar { get; set; }

        public IEnumerable<IView> Footer { get; set; }
        
        public LayoutViewProviderResult(params IViewProviderResult[] results) : base(results)
        {
        }
        
        public LayoutViewProviderResult BuildLayout()
        {
            EnsureZonedDictionary(base.GetResults());
            PrepareModel();
            return this;
        }

        void EnsureZonedDictionary(IEnumerable<IViewProviderResult> providerResults)
        {
            foreach (var result in providerResults)
            {
                foreach (var view in result.Views)
                {
                    if (view is IPositionedView positionedView)
                    {
                        var key = positionedView.Position.Zone;
                        _zonedViews.AddOrUpdate(key.ToLower(), new List<IPositionedView>()
                        {
                            positionedView
                        }, (k, v) =>
                        {
                            v.Add(positionedView);
                            return v;
                        });

                    }
                }
            }

        }

        void PrepareModel()
        {
            this.Header = _zonedViews["header"];
            this.Meta = _zonedViews["meta"];
            this.Content = _zonedViews["content"];
            this.SideBar = _zonedViews["sidebar"];
            this.Footer = _zonedViews["footer"];
        }
        
    }

}
