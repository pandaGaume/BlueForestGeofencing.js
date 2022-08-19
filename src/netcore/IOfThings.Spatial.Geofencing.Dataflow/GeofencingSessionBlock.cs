using IOfThings.Spatial.Geography;
using IOfThings.Telemetry;
using IOfThings.Telemetry.Dataflow;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace IOfThings.Spatial.Geofencing.Dataflow
{
    using OUTPUT = IEnumerable<ISegment<IGeofencingSample>>;

    public class GeofencingSessionBlock<T> : IPropagatorBlock<T, OUTPUT>
        where T: ISequenceable
    {
        private readonly string _who;
        private readonly SequencingBlock<T> _target;
        private readonly ISourceBlock<OUTPUT> _source;

        public GeofencingSessionBlock(string who, Func<ISequence<T>,OUTPUT> transform, Sequencer<T> sequencer = null)
        {
            _who = who;
            var target = new SequencingBlock<T>(sequencer);
            var source = new TransformBlock<ISequence<T>, OUTPUT>(transform);
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            target.LinkTo(source, linkOptions);
            _target = target;
            _source = source;
        }
        public string Who => _who;
        public Sequencer<T> Sequencer => _target.Sequencer;
        public Task Completion => _source.Completion;
        public void Complete() => _target.Complete();
        public OUTPUT ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<OUTPUT> target, out bool messageConsumed) => _source.ConsumeMessage(messageHeader, target, out messageConsumed);
        public void Fault(Exception exception) => _target.Fault(exception);
        public IDisposable LinkTo(ITargetBlock<OUTPUT> target, DataflowLinkOptions linkOptions = default) => _source.LinkTo(target, linkOptions);
        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, T messageValue, ISourceBlock<T> source, bool consumeToAccept) => _target.OfferMessage(messageHeader, messageValue, source, consumeToAccept);
        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<OUTPUT> target) => _source.ReleaseReservation(messageHeader, target);
        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<OUTPUT> target) => _source.ReserveMessage(messageHeader, target);
    }
}
