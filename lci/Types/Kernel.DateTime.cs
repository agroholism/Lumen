using System;

namespace StandartLibrary {
	internal class DateTimeClass : KType {
		public DateTimeClass() {
			this.meta = new TypeMetadata {
				Name = "Kernel.DateTime",
				Fields = new String[0],
			};

			Set("get_now", new LambdaFun((e, args) => {
				return new DateTime(System.DateTime.Now);
			}));
		}
	}
}
