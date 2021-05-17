using IOfThings.Spatial.Geography;
using IOfThings.Telemetry.Flow;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace IOfThings.Spatial.Geofencing.Dataflow
{
    public class GeofencingSessionBlock<T> : IPropagatorBlock<T,ISegment<IGeofencingSample>>
        where T: ISequenceable
    {
        string _who;
        SequencingBlock<T> _target;
        ISourceBlock<ISegment<IGeofencingSample>> _source;

        public GeofencingSessionBlock(string who, Func<ISequence<T>,IEnumerable<ISegment<IGeofencingSample>>> transform)
        {
            _who = who;
            var target = new SequencingBlock<T>();
            var source = new TransformManyBlock<ISequence<T>, ISegment<IGeofencingSample>>(transform);
            target.LinkTo(source);
        }

        public Task Completion => _source.Completion;

        public void Complete() => _target.Complete();

        public ISegment<IGeofencingSample> ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<ISegment<IGeofencingSample>> target, out bool messageConsumed) => _source.ConsumeMessage(messageHeader, target, out messageConsumed);

        public void Fault(Exception exception) => _target.Fault(exception);

        public IDisposable LinkTo(ITargetBlock<ISegment<IGeofencingSample>> target, DataflowLinkOptions linkOptions) => _source.LinkTo(target, linkOptions);

        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, T messageValue, ISourceBlock<T> source, bool consumeToAccept) => _target.OfferMessage(messageHeader, messageValue, source, consumeToAccept);

        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<ISegment<IGeofencingSample>> target) => _source.ReleaseReservation(messageHeader, target);

        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<ISegment<IGeofencingSample>> target) => _source.ReserveMessage(messageHeader, target);
    }
}
