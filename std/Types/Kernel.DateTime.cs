using System;

namespace Lumen.Lang.Std {
	internal class DateTimeClass : Record {
		public DateTimeClass() {
			this.meta = new TypeMetadata {
				Name = "Kernel.DateTime"
			};

			SetAttribute("get_now", new LambdaFun((e, args) => {
				return new DateTime(System.DateTime.Now);
			}));
		}
	}
}
