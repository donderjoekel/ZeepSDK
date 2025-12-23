using System;
using System.Collections.Generic;
using System.Linq;

namespace ZeepSDK.Utilities;

/// <summary>
/// A container that holds multiple <see cref="IDisposable"/> objects and disposes them all when disposed.
/// Useful for managing multiple disposable resources that should be cleaned up together.
/// </summary>
public readonly struct DisposableBag : IDisposable
{
    private readonly List<IDisposable> _disposables;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableBag"/> struct with the specified disposables.
    /// </summary>
    /// <param name="disposables">The disposable objects to include in the bag.</param>
    public DisposableBag(params IDisposable[] disposables)
    {
        _disposables = disposables.ToList();
    }
    
    /// <summary>
    /// Adds a disposable object to the bag.
    /// </summary>
    /// <param name="disposable">The disposable object to add.</param>
    public void Add(IDisposable disposable)
    {
        _disposables.Add(disposable);
    }

    /// <summary>
    /// Disposes all disposable objects contained in the bag and clears the collection.
    /// Null disposables are safely skipped.
    /// </summary>
    public void Dispose()
    {
        if (_disposables == null) return;
        foreach (IDisposable disposable in _disposables)
        {
            if (disposable == null) continue;
            disposable.Dispose();
        }
        _disposables.Clear();
    }
}
