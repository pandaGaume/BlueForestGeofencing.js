using IOfThings.Spatial.Geography;
using IOfThings.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace IOfThings.Spatial.Geofencing
{
    public static class GCircleExtensions
    {
        public static ILocation Center(this IGCircle shape, Matrix4x4 transform) => shape.GetLocation(transform);
        public static IEnumerable<ILocation> Polyline(this IGCircle shape, Matrix4x4 transform, int n = 32, bool close = true, EllipticSystem system = null)
        {
            var center = shape.Center(transform);
            return center.ToCirclePolyline(shape.Radius.SemiMajorAxis, n, close, system);
        }
        public static RelativePosition GetDistanceStatus(this IGCircle shape, double D) => D < shape.Radius.SemiMajorAxis ? RelativePosition.Inside : D > shape.Radius.SemiMajorAxis ? RelativePosition.Outside : RelativePosition.OnEdge;

        public static RelativePosition GetRelativePosition(this IGCircle shape, ILocation l)
        {
            return RelativePosition.Unknown;
        }

        private static IEnumerable<Vector2<double>> Intersections(this IGCircle shape, IGeofencingNode node, float distance, ISegment<IGeofencingSample> segment)
        {

            IEnvelope E = node.Envelope;
            ILocation C = E.GetCenter();
            ENUSystem ENU = null;
            if (distance != 0)
            {
                ENU = shape.GetENU(C);
                E.PadInPlace(distance, ENU);
            }

            IEnvelope SE = segment.Envelope;
            ILocation A = segment.First.Where;
            ILocation B = segment.Second.Where;

            // we first validate that there is a possible interaction between the segment and the circle.
            // So we verify the respective bounding box
            if (E.IntersectsWith(SE))
            {
                // we may compute the intersections of the AB segment and the circle.
                // For the purpose we switch to ENU coordinate
                // Note because the distance is limited, the spherical distorsion is also limited.
                ENU = ENU ?? shape.GetENU(C);
                // reach this point mean the segment is on a line wich cross the circle
                ENU.ConvertGeodeticToENU(C.Latitude, C.Longitude, C.Altitude ?? 0.0, out double cxEast, out double cyNorth, out double czUp);
                ENU.ConvertGeodeticToENU(A.Latitude, A.Longitude, A.Altitude ?? 0.0, out double axEast, out double ayNorth, out double azUp);
                ENU.ConvertGeodeticToENU(B.Latitude, B.Longitude, B.Altitude ?? 0.0, out double bxEast, out double byNorth, out double bzUp);

                return Algorithms.LineSegmentToCircleIntersection(axEast, ayNorth, bxEast, byNorth, cxEast, cyNorth, shape.Radius.SemiMajorAxis + distance);
            }

            return Enumerable.Empty<Vector2<double>>();
        }
        public static IEnumerable<IConditionEvent> CheckZone(this IGCircle circle, IPrimitive primitive, IGeofencingNode node, ISegment<IGeofencingSample> segment, float distance, IGeofencingEventFactory eventFactory)
        {

            IConditionEvent[] events = null;
            var areaIntersections = Intersections(circle, node, distance, segment);
            var count = areaIntersections.Count();
            if (count != 0)
            {
                ILocation A = segment.First.Where;
                ILocation B = segment.Second.Where;
                IEnvelope E = node.Envelope;
                if (distance != 0)
                {
                    E.PadInPlace(distance);
                }
                ILocation C = E.GetCenter();
                ENUSystem ENU = circle.GetENU(C);
                string actorId = eventFactory.BuildActorId(node.Geofence);
                ;
                switch (count)
                {
                    case 1:
                        {
                            var intersection = ENU.ConvertENUToGeodetic(areaIntersections).FirstOrDefault();
                            string who = segment.First.Who;
                            DateTime whenIntersection = segment.GetIntersectionTime(intersection, ENU);
                            var da = ENU.GetDistanceBetweenTwoPoint(A, C);
                            var db = ENU.GetDistanceBetweenTwoPoint(B, C);

                            var sa = circle.GetDistanceStatus(da);
                            var sb = circle.GetDistanceStatus(db);

                            if (sa != sb)
                            {
                                if (sa.IsInside())
                                {
                                    events = new IConditionEvent[] {
                                                        eventFactory.CreateEvent(actorId, who, node.Id, TriggerType.Exiting, intersection, whenIntersection),
                                                        eventFactory.CreateEvent(actorId, who, node.Id, TriggerType.Outside, B           , segment.Second.When)};
                                    break;
                                }

                                // reach this point mean the second point is inside
                                if (B.HasAltitude() && E.HasAltitude())
                                {
                                    if (B.Altitude.Value < E.Lowest)
                                    {
                                        events = new IConditionEvent[] { eventFactory.CreateEvent(actorId, who, node.Id, TriggerType.Under, B, segment.Second.When) };
                                        break;
                                    }
                                    if (B.Altitude.Value > E.Highest)
                                    {
                                        events = new IConditionEvent[] { eventFactory.CreateEvent(actorId, who, node.Id, TriggerType.Above, B, segment.Second.When) };
                                        break;
                                    }
                                }
                                events = new IConditionEvent[] {
                                                    eventFactory.CreateEvent(actorId, who, node.Id, TriggerType.Entering, intersection, whenIntersection),
                                                    eventFactory.CreateEvent(actorId, who, node.Id, TriggerType.Inside  , B           , segment.Second.When)};
                                break;
                            }

                            if (B.HasAltitude() && E.HasAltitude())
                            {
                                if (B.Altitude.Value < E.Lowest)
                                {
                                    events = new IConditionEvent[] { eventFactory.CreateEvent(eventFactory.BuildActorId(node.Geofence), who, node.Id, TriggerType.Under, intersection, segment.Second.When) };
                                    break;
                                }
                                if (B.Altitude.Value > E.Highest)
                                {
                                    events = new IConditionEvent[] { eventFactory.CreateEvent(eventFactory.BuildActorId(node.Geofence), who, node.Id, TriggerType.Above, intersection, segment.Second.When) };
                                    break;
                                }
                            }
                            // we touch the circle. From outside to outside with one entering/exit point
                            events = new IConditionEvent[] {
                                            eventFactory.CreateEvent(actorId, who, node.Id, TriggerType.Entering, intersection, whenIntersection),
                                            eventFactory.CreateEvent(actorId, who, node.Id, TriggerType.Inside  , intersection, whenIntersection),
                                            eventFactory.CreateEvent(actorId, who, node.Id, TriggerType.Exiting , intersection, whenIntersection),
                                            eventFactory.CreateEvent(actorId, who, node.Id, TriggerType.Outside , B           , segment.Second.When)};
                            break;

                        }
                    case 2:
                        {
                            var intersections = ENU.ConvertENUToGeodetic(areaIntersections).ToArray();
                            // ENU to geodetic MAY failed, in this case, the number of intersection will be less than 2.
                            // If happen, then do NOT trust the intersection location and send default value instead.
                            var success = intersections.Length > 1;
                            var firstIntersection = success ? intersections[0] : default;
                            var secondIntersection = success ? intersections[1] : default;
                            string who = segment.First.Who;
                            DateTime whenFirstIntersection = segment.GetIntersectionTime(firstIntersection, ENU);
                            DateTime whenSecondIntersection = segment.GetIntersectionTime(secondIntersection, ENU);

                            if (B.HasAltitude() && E.HasAltitude())
                            {
                                if (B.Altitude.Value < E.Lowest)
                                {
                                    events = new IConditionEvent[] {
                                                      eventFactory.CreateEvent(actorId, who, node.Id, TriggerType.Under, firstIntersection, segment.Second.When),
                                                      eventFactory.CreateEvent(actorId, who, node.Id, TriggerType.Under, secondIntersection, segment.Second.When)};
                                    break;
                                }
                                if (B.Altitude.Value > E.Highest)
                                {
                                    events = new IConditionEvent[] {
                                                      eventFactory.CreateEvent(actorId, who, node.Id, TriggerType.Above, firstIntersection, segment.Second.When),
                                                      eventFactory.CreateEvent(actorId, who, node.Id, TriggerType.Above, secondIntersection, segment.Second.When)};
                                    break;
                                }
                            }
                            // It's a full crossing : From outside to outside with one entering point and one exit point
                            events = new IConditionEvent[] {
                                                eventFactory.CreateEvent(actorId, who, node.Id, TriggerType.Entering, firstIntersection , whenFirstIntersection),
                                                eventFactory.CreateEvent(actorId, who, node.Id, TriggerType.Inside  , firstIntersection , whenFirstIntersection),
                                                eventFactory.CreateEvent(actorId, who, node.Id, TriggerType.Exiting , secondIntersection, whenSecondIntersection),
                                                eventFactory.CreateEvent(actorId, who, node.Id, TriggerType.Outside , B                 , segment.Second.When)};
                            break;

                        }
                }
            }
            if (events != null)
            {
                return events;
            }
            return Enumerable.Empty<IConditionEvent>();
        }
        public static IEnumerable<IConditionEvent> CheckFence(this IGCircle circle, IPrimitive primitive, IGeofencingNode node, ISegment<IGeofencingSample> segment, float distance, IGeofencingEventFactory eventFactory)
        {
            var intersections = Intersections(circle, node, distance, segment);
            var count = intersections.Count();
            if (count != 0)
            {
                string who = segment.First.Who;
                IEnvelope E = node.Envelope;
                if (distance != 0)
                {
                    E.PadInPlace(distance);
                }
                ILocation C = E.GetCenter();
                ENUSystem ENU = circle.GetENU(C);
                string actorId = eventFactory.BuildActorId(node.Geofence);

                foreach (var crossing in intersections)
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
    }
}
