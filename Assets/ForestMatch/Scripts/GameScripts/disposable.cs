//using System.Collections.Generic;
//using System;

//public sealed class DisposableScope : IDisposable
//{
//    // you can use ConcurrentQueue if you need thread-safe solution
//    private readonly Queue<IDisposable> _disposables = new();

//    public T Using<T>(T disposable) where T : IDisposable
//    {
//        _disposables.Enqueue(disposable);
//        return disposable;
//    }

//    public void Dispose()
//    {
//        foreach (var item in _disposables)
//            item.Dispose();
//    }
//}
