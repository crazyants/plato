using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.Views;

namespace Plato.Internal.Layout
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

            // Create a zoned ditionary 
            ZoneResults();
            
            // Return composed model
            return new LayoutViewModel()
            {
                Header = GetPositionedViews(LayoutZones.HeaderZoneName),
                Tools = GetPositionedViews(LayoutZones.ToolsZoneName),
                Meta = GetPositionedViews(LayoutZones.MetaZoneName),
                Content = GetPositionedViews(LayoutZones.ContentZoneName),
                SideBar = GetPositionedViews(LayoutZones.SideBarZoneName),
                Footer = GetPositionedViews(LayoutZones.FooterZoneName),
                Actions = GetPositionedViews(LayoutZones.ActionsZoneName),
                Asides = GetPositionedViews(LayoutZones.AsidesZoneName),
                Alerts = GetPositionedViews(LayoutZones.AlertsZoneName),
            };

        }

        IEnumerable<IPositionedView> GetPositionedViews(string zoneName)
        {

            // Returns an ordered list of all views within a zone. 
            if (_zonedViews.ContainsKey(zoneName))
            {
                return _zonedViews[zoneName].OrderBy(v => v.Position.Order);
            }

            return null;
        }

        void ZoneResults()
        {
            foreach (var result in _results)
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
