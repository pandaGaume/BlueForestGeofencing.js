using IOfThings.Spatial.Geography;
using IOfThings.Spatial.Text.GeoJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace IOfThings.Spatial.Geofencing
{
    public static class GPolygonExtensions
    {
        public static IEnumerable<IEnumerable<ILocation>> GetGeometries(this IGPolygon shape)
        {
            if (shape?.Geofence?.Geometry != null)
            {
                var geometry = shape.Geofence.Geometry;

                // accept feature as indirection
                if (geometry is GeoJsonFeature f)
                {
                    geometry = f.Geometry;
                }

                if (geometry is IGeoJsonGeometryCollection c)
                {
                     geometry = c.Geometries.ElementAt(shape.GeometryIndex);
                }

                float[][][] coordinates = null;
                if (geometry is GeoJsonMultiPolygon mp)
                {
                    var i = shape.Index.HasValue ? shape.Index.Value : shape.GeometryIndex;
                    coordinates = mp.Coordinates[i];
                }
                else if (geometry is GeoJsonPolygon p)
                {
                    coordinates = p.Coordinates;
                }
                if (coordinates != null)
                {
                    foreach (var subCoordinates in coordinates)
                    {
                        IEnumerable<ILocation> locations = subCoordinates.Select((c) => new Location(c[1], c[0], c.Length > 2 ? (float?)c[2] : null));
                        yield return locations.ToArray();
                    }
                }
                yield break;
            }
        }

        public static IPolygon ToPolygon(this IGPolygon shape, Matrix4x4 t, ENUSystem system)
        {
            // For type "Polygon", the "coordinates" member must be an array of LinearRing coordinate arrays. For Polygons with multiple rings, the first must be the exterior ring and any others must be interior rings or holes.
            var geometries = shape.GetGeometries().ToArray();

            // create the exterior
            var ext = geometries[0];
            if( t.IsIdentity == false)
            {
                ext = ext.TransformInPlace(t);
            }
            var coordinates = ext.ToENU(system);
            var polygon = new Polygon(coordinates).ForceDirection(PolygonOrientation.clockwise);
            if( geometries.Length > 1)
            {
                for(int i=1; i != geometries.Length; i++)
                {
                    var hole = geometries[0];
                    if (t.IsIdentity == false)
                    {
                        hole = hole.TransformInPlace(t);
                    }
                    var holeCoordinates = hole.ToENU(system);
                    polygon.AddInner(new Polygon(holeCoordinates).ForceDirection(PolygonOrientation.counterclockwise));
                }
            }
            return polygon;
        }

        public static IEnumerable<IGeofencingEvent> CheckFence(this IGPolygon gpoly, IPrimitive primitive, IGeofencingNode node, ISegment<IGeofencingSample> segment, float distance, IGeofencingEventFactory eventFactory)
        {
            List<Vector2<double>> intersections = null;

            IEnvelope SE = segment.Envelope;
            ILocation C = SE.GetCenter();

            ENUSystem ENU = gpoly.GetENU(C);
            IEnvelope E = gpoly.Envelope;
            var wt = node.WorldTransform;
            if (wt.IsIdentity == false)
            {
                E = E.Transform(wt);
            }
            // we first validate that there is a possible interaction between the segment and polygon.
            // So we verify the respective envelopes
            if (E.IntersectsWith(SE))
            {
                // we build a polygon with exterior as Clockwise and holes as CounterClockwise
                var polygon = gpoly.ToPolygon(wt, ENU);
                ILocation LA = segment.First.Where;
                ILocation LB = segment.Second.Where;
                ENU.ConvertGeodeticToENU(LA.Latitude, LA.Longitude, LA.Altitude ?? 0.0, out double lax, out double lay, out double laz);
                ENU.ConvertGeodeticToENU(LB.Latitude, LB.Longitude, LB.Altitude ?? 0.0, out double lbx, out double lby, out double lbz);

                // prepare segment
                var xysegment = new double[] { lax, lay, laz, lbx, lby, lbz }; // add z to be consistent
                                                                               // retreive the relativ position of segment from exterior.
                var rPositions = polygon.GetRelativePositions(xysegment, 3);
                // retreive the intersection with the exterior. Stride is 3 because we add the z value
                var result = polygon.GetIntersections(xysegment, 3).ToArray();

                if (result.Length == 0)
                {
                    if (rPositions[0] == RelativePosition.Outside || !polygon.HasInners)
                    {
                        // this is cases whithout anymore intersection possibilities
                        yield break;
                    }
                }
                else
                {
                    intersections = new List<Vector2<double>>(result);
                }

                if (polygon.Inners != null)
                {
                    foreach (var p in polygon.Inners)
                    {
                        // retreive the relativ position of segment from hole (counterClockwise)
                        // if outside (inside the hole), then update the segment relativ position
                        var hPositions = polygon.GetRelativePositions(xysegment, 3);
                        rPositions[0] = hPositions[0] == RelativePosition.Outside ? RelativePosition.Outside : rPositions[0];
                        rPositions[1] = hPositions[1] == RelativePosition.Outside ? RelativePosition.Outside : rPositions[1];
                        result = p.GetIntersections(xysegment, 3).ToArray();
                        if (result.Length != 0)
                        {
                            intersections = intersections ?? new List<Vector2<double>>();
                            intersections.AddRange(result);
                        }
                    }
                }

                if (intersections.Count == 0)
                {
                    yield break;
                }

                // now we have all the intersections.
                string who = segment.First.Who;

                if (intersections.Count != 1)
                {
                    // odering the intersections
                    intersections = intersections.OrderBy(v =>
                    {
                        var dx = lax - v.X;
                        var dy = lay - v.Y;
                        return dx * dx + dy * dy;
                    }).ToList();
                }

                string actorId = eventFactory.BuildActorId(node.Geofence);
                foreach ( var crossing in intersections)
                {
                    if (ENU.TryConvertENUToGeodetic(crossing.X, crossing.Y, 0, out double lat, out double lon, out double alt))
                    {
                        var where = new Location(lat, lon, alt);
                        DateTime when = segment.GetIntersectionTime(where, ENU);
                        yield return eventFactory.CreateEvent(actorId, who, node.Id, TriggerType.Crossing, where, when);
                    }
                    else
                    {
                        // very unlikely.. just skip the event.
                    }

                }
            }
        }

        public static IEnumerable<IGeofencingEvent> CheckZone(this IGPolygon gpoly, IPrimitive primitive, IGeofencingNode node, ISegment<IGeofencingSample> segment, float distance, IGeofencingEventFactory eventFactory)
        {
            List<Vector2<double>> intersections = null;

            IEnvelope SE = segment.Envelope;
            ILocation C = SE.GetCenter();

            ENUSystem ENU = gpoly.GetENU(C);
            IEnvelope E = gpoly.Envelope;
            var wt = node.WorldTransform;
            if (wt.IsIdentity == false)
            {
                E = E.Transform(wt);
            }
            // we first validate that there is a possible interaction between the segment and polygon.
            // So we verify the respective envelopes
            if (E.IntersectsWith(SE))
            {
                // we build a polygon with exterior as Clockwise and holes as CounterClockwise
                var polygon = gpoly.ToPolygon(wt, ENU);
                ILocation LA = segment.First.Where;
                ILocation LB = segment.Second.Where;
                ENU.ConvertGeodeticToENU(LA.Latitude, LA.Longitude, LA.Altitude ?? 0.0, out double lax, out double lay, out double laz);
                ENU.ConvertGeodeticToENU(LB.Latitude, LB.Longitude, LB.Altitude ?? 0.0, out double lbx, out double lby, out double lbz);

                // prepare segment
                var xysegment = new double[] { lax, lay, laz, lbx, lby, lbz }; // add z to be consistent
                                                                               // retreive the relativ position of segment from exterior.
                var rPositions = polygon.GetRelativePositions(xysegment, 3);
                // retreive the intersection with the exterior. Stride is 3 because we add the z value
                var result = polygon.GetIntersections(xysegment, 3).ToArray();

                if (result.Length == 0)
                {
                    if (rPositions[0] == RelativePosition.Outside || !polygon.HasInners)
                    {
                        // this is cases whithout anymore intersection possibilities
                        yield break;
                    }
                }
                else
                {
                    intersections = new List<Vector2<double>>(result);
                }

                if (polygon.Inners != null)
                {
                    foreach (var p in polygon.Inners)
                    {
                        // retreive the relativ position of segment from hole (counterClockwise)
                        // if outside (inside the hole), then update the segment relativ position
                        var hPositions = polygon.GetRelativePositions(xysegment, 3);
                        rPositions[0] = hPositions[0] == RelativePosition.Outside ? RelativePosition.Outside : rPositions[0];
                        rPositions[1] = hPositions[1] == RelativePosition.Outside ? RelativePosition.Outside : rPositions[1];
                        result = p.GetIntersections(xysegment, 3).ToArray();
                        if (result.Length != 0)
                        {
                            intersections = intersections ?? new List<Vector2<double>>();
                            intersections.AddRange(result);
                        }
                    }
                }

                if (intersections.Count == 0)
                {
                    yield break;
                }

                // now we have all the intersections.
                string who = segment.First.Who;

                if (intersections.Count != 1)
                {
                    // odering the intersections
                    intersections = intersections.OrderBy(v =>
                    {
                        var dx = lax - v.X;
                        var dy = lay - v.Y;
                        return dx * dx + dy * dy;
                    }).ToList();
                }

                var offset = 0;
                // we consider the OnEdge already inside, so we remove the first intersection
                // remember that at this point the list has been ordered.
                // note we do not removethe intersection from the list but we ofsfet the start of the loop
                if (rPositions[0] == RelativePosition.OnEdge)
                {
                    switch (rPositions[1])
                    {
                        case RelativePosition.OnEdge:
                        case RelativePosition.Inside:
                            {
                                offset++;
                                break;
                            }
                        case RelativePosition.Outside:
                            {
                                if (intersections.Count > 1)
                                {
                                    offset++;
                                }
                                break;
                            }
                    }
                }

                if (intersections.Count - offset != 0)
                {
                    string actorId = eventFactory.BuildActorId(node.Geofence);

                    var trigger = rPositions[0] == RelativePosition.Outside ? TriggerType.Exiting : TriggerType.Entering;
                    for (int i = offset; i < intersections.Count; i++)
                    {
                        var v = intersections[i];
                        // switch trigger
                        trigger = trigger == TriggerType.Exiting ? TriggerType.Entering : TriggerType.Exiting;
                        if (ENU.TryConvertENUToGeodetic(v.X, v.Y, 0, out double lat, out double lon, out double alt))
                        {
                            var where = new Location(lat, lon, alt);
                            DateTime when = segment.GetIntersectionTime(where, ENU);
                            yield return eventFactory.CreateEvent(actorId, who, node.Id, trigger, where, when);
                        }
                        else
                        {
                            // very unlikely.. just skip the event.
                        }
                    }

                    trigger = rPositions[1] == RelativePosition.Outside ? TriggerType.Outside : TriggerType.Inside;
                    yield return eventFactory.CreateEvent(actorId, who, node.Id, trigger, LB, segment.Second.When);
                }
            }
        }
    }
}
