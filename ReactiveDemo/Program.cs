using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;

namespace ReactiveDemo
{
    class Program
    {
        static Random random = new Random();

        static void Main(string[] args)
        {
            Run4();
        }

        private static void Run1()
        {
            IObservable<Int32> input = Observable.Range(1, 15);
            input.Where(i => i % 2 == 0 || PlaceHolder() == false).Subscribe(x => Console.Write("{0}", x));
            Console.WriteLine("----------");
        }

        private static void Run2()
        {
            IEnumerable<Int32> input2 = Enumerable.Range(1, 15);
            var result = input2.Where(i => i % 2 == 0 || PlaceHolder() == false);
            foreach (var x in result)
            {
                Console.Write("{0}", x);
            }
            Console.WriteLine("----------");
        }

        private static void Run3()
        {
            IObservable<Int32> input = Observable.Range(1, 15);

            IScheduler scheduler = Scheduler.Default;

            var result1 = input
                .Where(i => i % 2 == 0);

            result1
                //.SubscribeOn(scheduler)
                .Subscribe(x =>
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("[{0}]\tWhere1:\t{1}", Thread.CurrentThread.ManagedThreadId, x);
                }
                );

            var result2 = result1.Where(i => i % 4 == 0);

            result2
                //.SubscribeOn(scheduler)
                .Subscribe(x =>
                {
                    Console.WriteLine("[{0}]\tWhere2:\t{1}", Thread.CurrentThread.ManagedThreadId, x);
                }
                );

            result2.ToList();

            Console.WriteLine();
            Console.Read();
        }

        private static void Run4()
        {
            IObservable<Int32> input = Observable.Range(1, 5);

            IScheduler scheduler = Scheduler.Default;

            var result1 = input
                .Where(i => PlaceHolder(i.ToString()) == false || i % 2 == 0);

            result1
                //.SubscribeOn(scheduler)
                .Subscribe(x =>
                    {
                        Console.WriteLine("[{0}]\tWhere1:\t{1}", Thread.CurrentThread.ManagedThreadId, x);
                    }
                );

            var result2 = result1.Where(i => PlaceHolder(i.ToString()) == false || i % 4 == 0);

            result2
                //.SubscribeOn(scheduler)
                .Subscribe(x =>
                    {
                        Console.WriteLine("[{0}]\tWhere2:\t{1}", Thread.CurrentThread.ManagedThreadId, x);
                    }
                );

            result2.ToList();

            Console.WriteLine();
            Console.Read();
        }

        private static void Run45()
        {

            Console.WriteLine("Starting on threadId:{0}", Thread.CurrentThread.ManagedThreadId);

            var source = Observable.Create<int>(o =>
                {
                    Console.WriteLine("Invoked on threadId:{0}", Thread.CurrentThread.ManagedThreadId);
                    o.OnNext(1);
                    o.OnNext(2);
                    o.OnNext(3);
                    o.OnCompleted();
                    Console.WriteLine("Finished on threadId:{0}", Thread.CurrentThread.ManagedThreadId);
                    return Disposable.Empty;
                }
            );

            source
            .SubscribeOn(Scheduler.Default)
            .Subscribe(
                o => Console.WriteLine("Received {1} on threadId:{0}", Thread.CurrentThread.ManagedThreadId, o),
                () => Console.WriteLine("OnCompleted on threadId:{0}", Thread.CurrentThread.ManagedThreadId)
            );

            Console.WriteLine("Subscribed on threadId:{0}", Thread.CurrentThread.ManagedThreadId);

            Console.Read();
        }

        static bool PlaceHolder(string @char = "-")
        {
            //Thread.Sleep(random.Next(1000));
            Console.WriteLine("------[{0}]\t{1}", Thread.CurrentThread.ManagedThreadId, @char);
            return true;
        }
    }
}
