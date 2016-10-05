﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Layout.Elements;
using Plato.Layout.ModelBinding;
using Plato.Layout.Handlers;
using Plato.Layout.Placement;

namespace Plato.Layout.Views
{
    public class ElementResult
    {
        
        private readonly string _shapeType;
        private readonly Func<IBuildShapeContext, dynamic> _shapeBuilder;
        private readonly Func<dynamic, Task> _processing;
        private IDictionary<string, string> _otherLocations;
        private string _groupId;
        public string _differentiator;
        private string _prefix;
        private string _defaultLocation;

        public ElementResult(
            string shapeType, 
            Func<IBuildShapeContext, dynamic> shapeBuilder)
            :this(shapeType, shapeBuilder, null)
        {
        }

        public ElementResult(
            string shapeType, 
            Func<IBuildShapeContext, dynamic> shapeBuilder, 
            Func<dynamic, Task> processing)
        {
            // The shape type is necessary before the shape is created as it will drive the placement
            // resolution which itself can prevent the shape from being created.

            _shapeType = shapeType;
            _shapeBuilder = shapeBuilder;
            _processing = processing;
        }
           
        public void Apply(BuildDisplayContext context)
        {
            ApplyImplementation(context, context.DisplayType);
        }
        
        public void Apply(BuildEditorContext context)
        {
            ApplyImplementation(context, "Edit");
        }

        private void ApplyImplementation(
        BuildShapeContext context,
        string displayType)
        {

            if (String.IsNullOrEmpty(_differentiator))
            {
                _differentiator = _prefix;
            }

            // Look into specific implementations of placements (like placement.info files)
            var placement = context.FindPlacement((IElement)context.Shape, _differentiator, displayType);

            // If no placement is found, use the default location
            if (placement == null)
            {
                // Look for mapped display type locations
                if (_otherLocations != null)
                {
                    _otherLocations.TryGetValue(displayType, out _defaultLocation);
                }

                placement = new PlacementInfo() { Location = _defaultLocation };
            }

            // If there are no placement or it's explicitely noop then stop rendering execution
            if (String.IsNullOrEmpty(placement.Location) || placement.Location == "-")
            {
                return;
            }

            // Parse group placement.
            _groupId = placement.GetGroup();

            // If the shape's group doesn't match the currently rendered one, return
            if (!String.Equals(context.GroupId ?? "", _groupId ?? "", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var newShape = _shapeBuilder(context);

            // Ignore it if the driver returned a null shape.
            if (newShape == null)
            {
                return;
            }

            ElementMetaData newShapeMetadata = newShape.Metadata;
            //newShapeMetadata.Prefix = _prefix;
            newShapeMetadata.DisplayType = displayType;
            //newShapeMetadata.PlacementSource = placement.Source;
            //newShapeMetadata.Tab = placement.GetTab();

            // The _processing callback is used to delay execution of costly initialization
            // that can be prevented by caching
            if (_processing != null)
            {
                //newShapeMetadata.OnProcessing(_processing);
            }

            //// Apply cache settings
            //if (!String.IsNullOrEmpty(_cacheId) && _cache != null)
            //{
            //    _cache(newShapeMetadata.Cache(_cacheId));
            //}

            // If a specific shape is provided, remove all previous alternates and wrappers.
            if (!String.IsNullOrEmpty(placement.ShapeType))
            {
                newShapeMetadata.Type = placement.ShapeType;
                //newShapeMetadata.Alternates.Clear();
                //newShapeMetadata.Wrappers.Clear();
            }

            //if (placement.Alternates != null)
            //{
            //    foreach (var alternate in placement.Alternates)
            //    {
            //        newShapeMetadata.Alternates.Add(alternate);
            //    }
            //}

            //if (placement.Wrappers != null)
            //{
            //    foreach (var wrapper in placement.Wrappers)
            //    {
            //        newShapeMetadata.Wrappers.Add(wrapper);
            //    }
            //}

            dynamic parentShape = context.Shape;

            if (placement.IsLayoutZone())
            {
                parentShape = context.Layout;
            }

            var position = placement.GetPosition();
            var zones = placement.GetZones();

            foreach (var zone in zones)
            {
                if (parentShape == null)
                {
                    break;
                }

                var zoneProperty = parentShape.Zones;
                if (zoneProperty != null)
                {
                    // parentShape is a ZoneHolding
                    parentShape = zoneProperty[zone];
                }
                else
                {
                    // try to access it as a member
                    parentShape = parentShape[zone];
                }
            }

            if (String.IsNullOrEmpty(position))
            {
                parentShape.Add(newShape);
            }
            else
            {
                parentShape.Add(newShape, position);
            }
        }





    }
}