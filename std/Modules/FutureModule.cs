using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal sealed class FutureModule : Module {
		internal FutureModule() {
			this.Name = "Future";

			// 23.12.2020
			this.SetMember("run", new LambdaFun((scope, args) => {
				Fun action = scope["action"].ToFunction(scope);

				Task<Value> task = new(() => action.Call(new Scope(), Const.UNIT));

				task.Start();

				return new Future(task);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("action")
				}
			});

			this.SetMember("delay", new LambdaFun((scope, args) => {
				Task<Value> task = Task.Delay(scope["time"].ToInt(scope))
					.ContinueWith(_ => Const.UNIT);

				return new Future(task);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("time")
				}
			});

			this.SetMember("all", new LambdaFun((scope, args) => {
				IEnumerable<Value> futures = scope["futures"].ToStream(scope);

				IEnumerable<Task<Value>> tasks =
					futures.Select(i => (i as Future).Task); // unsafe!

				return new Future(
					Task.WhenAll(tasks)
					.ContinueWith(task => (Value)new List(task.Result))
				);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("futures")
				}
			});

			this.SetMember("any", new LambdaFun((scope, args) => {
				IEnumerable<Value> futures = scope["futures"].ToStream(scope);

				IEnumerable<Task<Value>> tasks =
					futures.Select(i => (i as Future).Task); // unsafe!

				return new Future(
					Task.WhenAny(tasks)
					.ContinueWith(task => task.Result.Result)
				);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("futures")
				}
			});

			this.SetMember("wait", new LambdaFun((scope, args) => {
				Value task = scope["task"];

				if (task is Future t) {
					try {
						t.Task.Wait();
					}
					catch (AggregateException e) {
						throw e.GetBaseException();
					}

					return t.Task.Result;
				}

				IEnumerable<System.Threading.Tasks.Task<Value>> tasks =
					task.ToStream(scope).Select(i => (i as Future).Task); // unsafe!
				Task.WaitAll(tasks.ToArray());

				List results = new List(tasks.Select(i => {
					try {
						i.Wait();
					}
					catch (AggregateException e) {
						throw e.GetBaseException();
					}

					return i.Result;
				}));

				return results;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("task")
				}
			});

			this.SetMember("then", new LambdaFun((scope, args) => {
				Future future = scope["future"] as Future;
				Fun then = scope["thenFunction"].ToFunction(scope);

				return new Future(
					future.Task.ContinueWith(
						task => {
							if (task.IsFaulted) {
								throw task.Exception.GetBaseException();
							}

							return then.Call(new Scope(), task.Result);
						}
					)
				);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("thenFunction"),
					new NamePattern("future"),
				}
			});

			this.SetMember("catch", new LambdaFun((scope, args) => {
				Future future = scope["future"] as Future;
				Fun then = scope["thenFunction"].ToFunction(scope);

				return new Future(
					future.Task.ContinueWith(
						task => {
							if (task.IsFaulted) {
								Exception e = task.Exception.GetBaseException();
								return then.Call(new Scope(), e as LumenException);
							}

							return task.Result;
						}
					)
				);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("thenFunction"),
					new NamePattern("future"),
				}
			});
		}
	}
}
