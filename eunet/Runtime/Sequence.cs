using System.Collections.Generic;

namespace Argent.Xenon.Runtime {
	public class Sequence : ImmutObject {
		public KsTypeable Type => XnStd.SequenceType;

		internal IEnumerable<XnObject> xns;

		public Sequence(IEnumerable<XnObject> xns) {
			this.xns = xns;
		}
	}
}
