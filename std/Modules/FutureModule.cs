using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal sealed class FutureModule : Module {
		internal FutureModule() {
			this.Name = "Future";

			this.AppendImplementation(Prelude.Monad);

			this.SetMember("run", new LambdaFun((scope, args) => {
				Fun action = scope["action"].ToFunction(scope);

				return new Future(Task.Factory.StartNew(() => action.Call(new Scope(), Const.UNIT)));
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("action", Prelude.Function)
				}
			});

			this.SetMember("wrap", new LambdaFun((scope, args) => {
				Value val = scope["val"];

				return new Future(Task.FromResult(val));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("val")
				}
			});

			this.SetMember("delay", new LambdaFun((scope, args) => {
				Int32 time = scope["time"].ToInt(scope);

				return new Future(Task.Delay(time).ContinueWith(_ => Const.UNIT));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("time")
				}
			});

			this.SetMember("delayed", new LambdaFun((scope, args) => {
				Int32 time = scope["time"].ToInt(scope);
				Fun continuation = scope["continuation"].ToFunction(scope);

				return new Future(
					Task.Delay(time).ContinueWith(
						_ => continuation.Call(new Scope(), Const.UNIT)
					)
				);
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("time", Prelude.Number),
					new ExactTypePattern("continuation", Prelude.Function)
				}
			});


			this.SetMember("getException", new LambdaFun((scope, args) => {
				Future future = scope["future"] as Future;


				try {
					future.Task.Wait();
				}
				catch (AggregateException e) {
					return e.GetBaseException() as LumenException;
				}

				return Const.UNIT;
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("future", this),
				}
			});


			this.SetMember("isFailed", new LambdaFun((scope, args) => {
				Future future = scope["future"] as Future;

				return new Logical(future.Task.IsFaulted);
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("future", this),
				}
			});

			this.SetMember("isSuccess", new LambdaFun((scope, args) => {
				Future future = scope["future"] as Future;

				return new Logical(future.Task.Status == TaskStatus.RanToCompletion);
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("future", this),
				}
			});


			this.SetMember("wait", new LambdaFun((scope, args) => {
				Future future = scope["future"].ToFuture(scope);

				try {
					future.Task.Wait();
				} catch (AggregateException) {

				}

				return future;
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("future", this),
				}
			});

			this.SetMember("waitFor", new LambdaFun((scope, args) => {
				Int32 time = scope["time"].ToInt(scope);
				Future future = scope["future"].ToFuture(scope);

				try {
					return new Logical(future.Task.Wait(time));
				}
				catch (AggregateException) {
					return new Logical(true);
				}
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("time", Prelude.Number),
					new ExactTypePattern("future", this),
				}
			});

			this.SetMember("waitResult", new LambdaFun((scope, args) => {
				Future future = scope["future"].ToFuture(scope);

				try {
					future.Task.Wait();
				} catch (AggregateException e) {
					return Helper.Failed(e.GetBaseException() as LumenException);
				}

				return Helper.Success(future.Task.Result);
			}) { 
				Parameters = new List<IPattern> {
					new ExactTypePattern("future", this)
				}
			});

			this.SetMember("await", new LambdaFun((scope, args) => {
				Value futureOrFutures = scope["future"];

				if (futureOrFutures is Future future) {
					try {
						future.Task.Wait();
					}
					catch (AggregateException e) {
						throw e.GetBaseException();
					}

					return future.Task.Result;
				}

				Task<Value>[] futures =
					futureOrFutures.ToSeq(scope)
					.Select(i => i.ToFuture(scope).Task)
					.ToArray();

				Task.WaitAll(futures);

				return new List(futures.Select(i =>
					!i.IsFaulted ? i.Result : throw i.Exception.GetBaseException()
				));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("future"),
				}
			});


			this.SetMember("whenAll", new LambdaFun((scope, args) => {
				IEnumerable<Value> futures = scope["futures"].ToSeq(scope);

				IEnumerable<Task<Value>> tasks =
					futures.Select(i => i.ToFuture(scope).Task);

				return new Future(
					Task.WhenAll(tasks)
					.ContinueWith(task => (Value)new List(task.Result))
				);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("futures")
				}
			});

			this.SetMember("whenAny", new LambdaFun((scope, args) => {
				IEnumerable<Value> futures = scope["futures"].ToSeq(scope);

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


			static Value Then(Task<Value> task, Fun continuation) {
				if (task.IsFaulted) {
					throw task.Exception.GetBaseException();
				}

				return continuation.Call(new Scope(), task.Result);
			}

			LambdaFun map = new LambdaFun((scope, args) => {
				Future future = scope["future"].ToFuture(scope);
				Fun continuation = scope["continuation"].ToFunction(scope);

				return new Future(
					future.Task.ContinueWith(
						task => Then(task, continuation)
					)
				);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("continuation"),
					new NamePattern("future"),
				}
			};

			this.SetMember("then", map);
			this.SetMember("map", map);

			this.SetMember("onCompleted", new LambdaFun((scope, args) => {
				Future future = scope["future"].ToFuture(scope);
				Fun continuation = scope["continuation"].ToFunction(scope);

				return new Future(
					future.Task.ContinueWith(
						task => continuation.Call(new Scope(), new Future(task))
					)
				);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("continuation"),
					new NamePattern("future"),
				}
			});

			this.SetMember("catch", new LambdaFun((scope, args) => {
				Future future = scope["future"].ToFuture(scope);
				Fun continuation = scope["continuation"].ToFunction(scope);

				return new Future(
					future.Task.ContinueWith(
						task => task.IsFaulted
							? continuation.Call(new Scope(), task.Exception.GetBaseException() as LumenException)
							: task.Result
					)
				);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("continuation"),
					new NamePattern("future"),
				}
			});

			this.SetMember("lift", new LambdaFun((scope, args) => {
				Future future = scope["future"].ToFuture(scope);
				Future continuationFuture = scope["continuation"].ToFuture(scope);

				return new Future(
					continuationFuture.Task.ContinueWith(continuation => { 
						if (continuation.IsFaulted) {
							throw continuation.Exception.GetBaseException();
						}

						return future.Task.ContinueWith(
							task => Then(task, continuation.Result.ToFunction(scope))
						);
					}).Unwrap()
				);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("future"),
					new NamePattern("continuation"),
				}
			});

			this.SetMember("bind", new LambdaFun((scope, args) => {
				Future future = scope["future"].ToFuture(scope);
				Fun continuation = scope["continuation"].ToFunction(scope);

				return new Future(
					future.Task.ContinueWith(
						task => task.IsFaulted
								? throw task.Exception.GetBaseException()
								: continuation.Call(new Scope(), task.Result).ToFuture(scope).Task

					).Unwrap()
				);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("continuation"),
					new NamePattern("future"),
				}
			});

			this.SetMember("unwrap", new LambdaFun((scope, args) => {
				Future future = scope["future"] as Future;

				return new Future(
					future.Task.ContinueWith(
						task => task.Result.ToFuture(scope).Task
					).Unwrap()
				);
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("future", this),
				}
			});
		}
	}
}
