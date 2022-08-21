using IOfThings.Spatial.Geography;
using IOfThings.Spatial.Text.GeoJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace IOfThings.Spatial.Geofencing
{
    public static class GLineExtensions
    {
        public static ILocation InitialLocation(this IGLine line, Matrix4x4 t) => GetLocation(line, 0, line.InitialIndex, t);
        public static ILocation FinalLocation(this IGLine line, Matrix4x4 t) => GetLocation(line, 1, line.FinalIndex, t);
        public static ILocation InitialLocation(this IGLine line) => GetLocation(line, 0, line.InitialIndex, Matrix4x4.Identity);
        public static ILocation FinalLocation(this IGLine line) => GetLocation(line, 1, line.FinalIndex, Matrix4x4.Identity);
        private static ILocation GetLocation(this IGLine line, int index, int defaultIndex, Matrix4x4 t)
        {
            var geom = line.Geofence?.Geometry;
            if (geom != null)
            {
                if (geom is IGeoJsonGeometryCollection c)
                {
                    geom = c.Geometries.ElementAt(line.GeometryIndex);
                }
                if (geom is IGeoJsonLineString ls)
                {
                    var l = ls.Coordinates[defaultIndex].ToLocation();
                    return t.IsIdentity ? l : l.Transform(t);
                }
                if (geom is IGeoJsonMultiPoint mp)
                {
                    var l =  mp.Coordinates[index].ToLocation();
                    return t.IsIdentity ? l : l.Transform(t);
                }
            }
            return default(ILocation);
        }
        public static IEnvelope BuildEnvelope(this IGLine line)
        {
            ILocation[] locations = { line.InitialLocation(), line.FinalLocation() };
            return locations.Aggregate();
        }
        public static IEnvelope BuildEnvelope(this IGLine line, Matrix4x4 t)
        {
            ILocation[] locations = { line.InitialLocation(t), line.FinalLocation(t) };
            return locations.Aggregate();
        }
        public static IEnumerable<Vector2<double>> FenceIntersections(this IGLine line, IGeofencingNode node, float distance, ISegment<IGeofencingSample> segment)
        {

                //IEnvelope LE = node.Envelope;
                var wt = node.WorldTransform;
                ILocation LA = line.InitialLocation(wt);
                ILocation LB = line.FinalLocation(wt);

                var LE = new Envelope(LA).AddInPlace(LB);

                ILocation C = LE.GetCenter();
                ENUSystem ENU = null;
                if (distance != 0)
                {
                    ENU = line.GetENU(C);
                    LE.PadInPlace(distance, ENU);
                }

                IEnvelope SE = segment.Envelope;
                ILocation A = segment.First.Where;
                ILocation B = segment.Second.Where;

                // we first validate that there is a possible interaction between the segments.
                // So we verify the respective envelopes
                if (LE.IntersectsWith(SE))
                {
                    // we may compute the intersections of the segments.
                    // For the purpose we switch to ENU coordinate
                    // Note because the distance is limited, the spherical distorsion is also limited.
                    ENU = ENU ?? line.GetENU(C);
                    ENU.ConvertGeodeticToENU(LA.Latitude, LA.Longitude, LA.Altitude ?? 0.0, out double laxEast, out double layNorth, out double lazUp);
                    ENU.ConvertGeodeticToENU(LB.Latitude, LB.Longitude, LB.Altitude ?? 0.0, out double lbxEast, out double lbyNorth, out double lbzUp);

                    ENU.ConvertGeodeticToENU(A.Latitude, A.Longitude, A.Altitude ?? 0.0, out double axEast, out double ayNorth, out double azUp);
                    ENU.ConvertGeodeticToENU(B.Latitude, B.Longitude, B.Altitude ?? 0.0, out double bxEast, out double byNorth, out double bzUp);

                    if (Algorithms.LineSegmentToLineSegmentIntersection(axEast, ayNorth, bxEast, byNorth,
                                                                        laxEast, layNorth, lbxEast, lbyNorth,
                                                                        out Vector2<double> intersection))
                    {
                        yield return intersection;
                    }
                }

        }
        public static IEnumerable<IGeofencingEvent> CheckFence(this IGLine line, IPrimitive primitive, IGeofencingNode node, ISegment<IGeofencingSample> segment, IGeofencingEventFactory eventFactory)
        {
            IGeofencingEvent[] events = null;
            if (primitive.TypeCode == PrimitiveType.Fence)
            {
                var geoIntersections = FenceIntersections(line, node, 0, segment);
                var count = geoIntersections.Count();
                if (count != 0)
                {
                    IGeofencingShape shape = node.GetShape();
                    IEnvelope E = node.Envelope;
                    ILocation C = E.GetCenter();
                    ENUSystem ENU = shape.GetENU(C);
                    switch (count)
                    {
                        case 1:
                            {
                                var intersection = ENU.ConvertENUToGeodetic(geoIntersections).FirstOrDefault();
                                string who = segment.First.Who;
                                DateTime whenIntersection = segment.GetIntersectionTime(intersection, ENU);
                                events = new IGeofencingEvent[] { eventFactory.CreateEvent(eventFactory.BuildActorId(node.Geofence), who, node.Id, TriggerType.Crossing, intersection, whenIntersection) };
                                break;
                            }
                        case 2:
                            {
                                var enuIntersections = ENU.ConvertENUToGeodetic(geoIntersections).ToArray();
                                // ENU to geodetic MAY failed, in this case, the number of intersection will be less than 2.
                                // If happen, then do NOT trust the intersection location and send default value instead.
                                var success = enuIntersections.Length > 1;
                                var firstIntersection = success ? enuIntersections[0] : default;
                                var secondIntersection = success ? enuIntersections[1] : default;
                                string who = segment.First.Who;
                                DateTime whenFirstIntersection = segment.GetIntersectionTime(firstIntersection, ENU);
                                DateTime whenSecondIntersection = segment.GetIntersectionTime(secondIntersection, ENU);

                                var actorId = eventFactory.BuildActorId(node.Geofence);
                                events = new IGeofencingEvent[] {
                                eventFactory.CreateEvent(actorId, who, node.Id, TriggerType.Crossing, firstIntersection , whenFirstIntersection),
                                eventFactory.CreateEvent(actorId, who, node.Id, TriggerType.Crossing , secondIntersection, whenSecondIntersection)};
                                break;
                            }
                    }
                }
            }
            if (events != null)
            {
                return events;
            }
            return Enumerable.Empty<IGeofencingEvent>();
        }
    }
}
