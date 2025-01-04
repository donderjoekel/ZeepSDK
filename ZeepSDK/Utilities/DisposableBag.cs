using System;
using System.Collections.Generic;
using System.Linq;

namespace ZeepSDK.Utilities;

public readonly struct DisposableBag : IDisposable
{
    private readonly List<IDisposable> _disposables;
    
    public DisposableBag(params IDisposable[] disposables)
    {
        _disposables = disposables.ToList();
    }
    
    public void Add(IDisposable disposable)
    {
        _disposables.Add(disposable);
    }

    public void Dispose()
    {
        foreach (IDisposable disposable in _disposables)
        {
            disposable.Dispose();
        }
    }
}
