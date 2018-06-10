using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plato.Layout.ViewProviders;
using Plato.Layout.Views;

namespace Plato.Layout
{

    public interface ILayoutManager
    {
        LayoutViewProviderResult Compose(IViewProviderResult results);

    }

    public class LayoutManager : ILayoutManager
    {

        string[] _supportedZones = new string[]
        {
            "header",
            "meta",
            "content",
            "sidebar",
            "footer"
        };

        ConcurrentDictionary<string, IPositionedView> _zonedViews =
            new ConcurrentDictionary<string, IPositionedView>();


        public LayoutViewProviderResult Compose(IViewProviderResult results)
        {
            EnsureZonedData(results);
            return PrepareModel();
        }

        void EnsureZonedData(IViewProviderResult results)
        {

            // Ensure we are working with a positional view
            if (results is IEnumerable<IPositionedView> views)
            {
                foreach (var view in views)
                {

                    // We already expect a zone
                    if (String.IsNullOrEmpty(view.ViewPosition.Zone))
                    {
                        throw new Exception(
                            $"No znon has been specified for the view {view.ViewName}.");
                    }

                    // Is the zone supported?
                    var zone = view.ViewPosition.Zone.ToLower();
                    if (!_supportedZones.Contains(zone))
                    {
                        throw new Exception(
                            $"The zone name '{zone}' is not supported. Supported zones include {String.Join(",", _supportedZones)}. Please update the zone naame.");
                    }

                    _zonedViews.TryAdd(zone.ToLower(), view);
               
                }

            }

        }

        LayoutViewProviderResult PrepareModel()
        {

            var model = new LayoutViewProviderResult();

            model.Header = (IEnumerable<IView>) _zonedViews.Where(z => z.Key.Contains("header"));
            model.Content = (IEnumerable<IView>)_zonedViews.Where(z => z.Key.Contains("contact"));
            model.Footer = (IEnumerable<IView>)_zonedViews.Where(z => z.Key.Contains("footer"));

            return model;

        }

    }
}
