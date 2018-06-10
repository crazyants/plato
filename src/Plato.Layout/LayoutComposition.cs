using System.Collections.Concurrent;
using System.Collections.Generic;
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
                Header = GetZoneViews(LayoutZones.HeaderZoneName),
                Tools = GetZoneViews(LayoutZones.ToolsZoneName),
                Meta = GetZoneViews(LayoutZones.MetaZoneName),
                Content = GetZoneViews(LayoutZones.ContentZoneName),
                SideBar = GetZoneViews(LayoutZones.SideBarZoneName),
                Footer = GetZoneViews(LayoutZones.FooterZoneName),
                Actions = GetZoneViews(LayoutZones.ActionsZoneName),
                Asides = GetZoneViews(LayoutZones.AsidesZoneName)
            };

        }

        IEnumerable<IView> GetZoneViews(string zoneName)
        {

            if (_zonedViews.ContainsKey(zoneName))
            {
                return _zonedViews[zoneName];
            }

            return null;
        }

        void ZoneResults(IEnumerable<IViewProviderResult> providerResults)
        {
            foreach (var result in providerResults)
            {
                foreach (var view in result.Views)
                {
                    // We can only handle positioned views
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
