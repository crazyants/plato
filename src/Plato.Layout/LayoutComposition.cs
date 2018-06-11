using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32.SafeHandles;
using Plato.Layout.ViewProviders;
using Plato.Layout.Views;

namespace Plato.Layout
{
    public class LayoutComposition
    {
        
        private readonly IEnumerable<IViewProviderResult> _results;
        private readonly ConcurrentDictionary<string, IList<IPositionedView>> _zonedViews =
            new ConcurrentDictionary<string, IList<IPositionedView>>();

        public LayoutComposition(IEnumerable<IViewProviderResult> results)
        {
            _results = results;
        }

        public LayoutViewModel Compose()
        {

            ZoneResults(_results);
            
            return new LayoutViewModel()
            {
                Header = GetPositionedViews(LayoutZones.HeaderZoneName),
                Tools = GetPositionedViews(LayoutZones.ToolsZoneName),
                Meta = GetPositionedViews(LayoutZones.MetaZoneName),
                Content = GetPositionedViews(LayoutZones.ContentZoneName),
                SideBar = GetPositionedViews(LayoutZones.SideBarZoneName),
                Footer = GetPositionedViews(LayoutZones.FooterZoneName),
                Actions = GetPositionedViews(LayoutZones.ActionsZoneName),
                Asides = GetPositionedViews(LayoutZones.AsidesZoneName)
            };

        }

        IEnumerable<IView> GetPositionedViews(string zoneName)
        {

            if (_zonedViews.ContainsKey(zoneName))
            {
                return _zonedViews[zoneName].OrderBy(v => v.Position.Order);
            }

            return null;
        }

        void ZoneResults(IEnumerable<IViewProviderResult> providerResults)
        {
            foreach (var result in providerResults)
            {
                foreach (var view in result.Views)
                {
                    // We can only handle views that implement IPositionedView
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
        
    }
}
