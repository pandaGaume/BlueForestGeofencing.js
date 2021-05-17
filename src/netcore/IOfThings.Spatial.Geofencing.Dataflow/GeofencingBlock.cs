using IOfThings.Spatial;
using IOfThings.Spatial.Geography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace IOfThings.Spatial.Geofencing
{
    public class GeofencingBlockOptions
    {
        public readonly static GeofencingBlockOptions Default = new GeofencingBlockOptions();

        internal const int defaultMaxDepth = 16;
        internal const int defaultMaxCount = 16;
        internal const SplitMode defaultSplitMode = SplitMode.Octree;

        Ellipsoid _e = Ellipsoid.WGS84;
        int _imd = defaultMaxDepth;
        int _imc = defaultMaxCount;
        SplitMode _ism = defaultSplitMode;

        public Ellipsoid Ellipsoid { get => _e; set => _e = value; }
        public int IndexMaxDepth { get => _imd; set => _imd = value; }
        public int IndexMaxCount { get => _imc; set => _imc = value; }
        public SplitMode IndexSplitMode { get => _ism; set => _ism = value; }
    }

    public class GeofencingBlock : IPropagatorBlock<IEnumerable<ISegment<IGeofencingSample>>, IEnumerable<IGeofencingEvent>>, 
                                   IReceivableSourceBlock<IEnumerable<IGeofencingEvent>>
    {
        private readonly SpatialIndex<INode> _index;
        private readonly ENUSystem _enu;
        private readonly IList<IGeofence> _list;
        private readonly ITargetBlock<IEnumerable<ISegment<IGeofencingSample>>> _target;
        private readonly IReceivableSourceBlock<IEnumerable<IGeofencingEvent>> _source;
 
        public GeofencingBlock(IEnumerable<IGeofence> geofences, GeofencingBlockOptions options = null)
        {
            var o = options ?? GeofencingBlockOptions.Default;

            var source = new BufferBlock<IEnumerable<IGeofencingEvent>>();
            var target = new ActionBlock<IEnumerable<ISegment<IGeofencingSample>>>(
                messageValue =>
                {
                    foreach (var s in messageValue)
                    {
                        var env = s.BoundingEnvelope;
                        var box = env.ToENU(_enu);
                        var nodes = _index.Inside(box);
                        var events = nodes.Apply(s);
                        source.Post(events);
                    }
                }
            );
            target.Completion.ContinueWith(delegate
            {
                source.Complete();
            });

            _target = target;
            _source = source;

            _list = geofences.ToList();
            _enu = _list.DefineGeodeticSystem(o.Ellipsoid);
            var env = _list.Select(g => g.BoundingEnvelope).Aggregate();
            var box = env.ToENU(_enu);
            _index = new SpatialIndex<INode>(box, o.IndexMaxDepth, o.IndexMaxCount, o.IndexSplitMode);
            _index.AddRange(_list.SelectMany(g => g.Nodes));
        }
        
        public Task Completion => _source.Completion;
        public IEnumerable<IGeofencingEvent> ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<IEnumerable<IGeofencingEvent>> target, out bool messageConsumed)=>_source.ConsumeMessage(messageHeader, target, out messageConsumed);
        public IDisposable LinkTo(ITargetBlock<IEnumerable<IGeofencingEvent>> target, DataflowLinkOptions linkOptions)=>_source.LinkTo(target, linkOptions);
        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<IEnumerable<IGeofencingEvent>> target)=>_source.ReleaseReservation(messageHeader, target);
        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<IEnumerable<IGeofencingEvent>> target)=>_source.ReserveMessage(messageHeader, target);
        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, IEnumerable<ISegment<IGeofencingSample>> messageValue, ISourceBlock<IEnumerable<ISegment<IGeofencingSample>>> source, bool consumeToAccept)=>_target.OfferMessage(messageHeader, messageValue, source, consumeToAccept);
        public void Complete()=>_target.Complete();
        public void Fault(Exception exception)=>_target.Fault(exception);
        public bool TryReceive(Predicate<IEnumerable<IGeofencingEvent>> filter, out IEnumerable<IGeofencingEvent> item)=>_source.TryReceive(filter, out item);
        public bool TryReceiveAll(out IList<IEnumerable<IGeofencingEvent>> items)=>_source.TryReceiveAll(out items);
    }
}
