using System.Collections.Generic;

namespace Argent.Xenon.Runtime {
	public class Iterator : XnObject {
		internal IEnumerator<XnObject> InnerValue { get; private set; } 

		public KsTypeable Type => XnStd.IteratorType;

		public Iterator(IEnumerator<XnObject> enumerator) {
			this.InnerValue = enumerator;
		}
	}
}
