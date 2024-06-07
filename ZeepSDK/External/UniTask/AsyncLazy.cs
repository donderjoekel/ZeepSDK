﻿#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Threading;

namespace ZeepSDK.External.Cysharp.Threading.Tasks
{
    public class AsyncLazy
    {
        static Action<object> continuation = SetCompletionSource;

        Func<UniTask> taskFactory;
        UniTaskCompletionSource completionSource;
        UniTask.Awaiter awaiter;

        object syncLock;
        bool initialized;

        public AsyncLazy(Func<UniTask> taskFactory)
        {
            this.taskFactory = taskFactory;
            this.completionSource = new UniTaskCompletionSource();
            this.syncLock = new object();
            this.initialized = false;
        }

        internal AsyncLazy(UniTask task)
        {
            this.taskFactory = null;
            this.completionSource = new UniTaskCompletionSource();
            this.syncLock = null;
            this.initialized = true;

            UniTask.Awaiter awaiter = task.GetAwaiter();
            if (awaiter.IsCompleted)
            {
                SetCompletionSource(awaiter);
            }
            else
            {
                this.awaiter = awaiter;
                awaiter.SourceOnCompleted(continuation, this);
            }
        }

        public UniTask Task
        {
            get
            {
                EnsureInitialized();
                return completionSource.Task;
            }
        }


        public UniTask.Awaiter GetAwaiter() => Task.GetAwaiter();

        void EnsureInitialized()
        {
            if (Volatile.Read(ref initialized))
            {
                return;
            }

            EnsureInitializedCore();
        }

        void EnsureInitializedCore()
        {
            lock (syncLock)
            {
                if (!Volatile.Read(ref initialized))
                {
                    Func<UniTask> f = Interlocked.Exchange(ref taskFactory, null);
                    if (f != null)
                    {
                        UniTask task = f();
                        UniTask.Awaiter awaiter = task.GetAwaiter();
                        if (awaiter.IsCompleted)
                        {
                            SetCompletionSource(awaiter);
                        }
                        else
                        {
                            this.awaiter = awaiter;
                            awaiter.SourceOnCompleted(continuation, this);
                        }

                        Volatile.Write(ref initialized, true);
                    }
                }
            }
        }

        void SetCompletionSource(in UniTask.Awaiter awaiter)
        {
            try
            {
                awaiter.GetResult();
                completionSource.TrySetResult();
            }
            catch (Exception ex)
            {
                completionSource.TrySetException(ex);
            }
        }

        static void SetCompletionSource(object state)
        {
            AsyncLazy self = (AsyncLazy)state;
            try
            {
                self.awaiter.GetResult();
                self.completionSource.TrySetResult();
            }
            catch (Exception ex)
            {
                self.completionSource.TrySetException(ex);
            }
            finally
            {
                self.awaiter = default;
            }
        }
    }

    public class AsyncLazy<T>
    {
        static Action<object> continuation = SetCompletionSource;

        Func<UniTask<T>> taskFactory;
        UniTaskCompletionSource<T> completionSource;
        UniTask<T>.Awaiter awaiter;

        object syncLock;
        bool initialized;

        public AsyncLazy(Func<UniTask<T>> taskFactory)
        {
            this.taskFactory = taskFactory;
            this.completionSource = new UniTaskCompletionSource<T>();
            this.syncLock = new object();
            this.initialized = false;
        }

        internal AsyncLazy(UniTask<T> task)
        {
            this.taskFactory = null;
            this.completionSource = new UniTaskCompletionSource<T>();
            this.syncLock = null;
            this.initialized = true;

            UniTask<T>.Awaiter awaiter = task.GetAwaiter();
            if (awaiter.IsCompleted)
            {
                SetCompletionSource(awaiter);
            }
            else
            {
                this.awaiter = awaiter;
                awaiter.SourceOnCompleted(continuation, this);
            }
        }

        public UniTask<T> Task
        {
            get
            {
                EnsureInitialized();
                return completionSource.Task;
            }
        }


        public UniTask<T>.Awaiter GetAwaiter() => Task.GetAwaiter();

        void EnsureInitialized()
        {
            if (Volatile.Read(ref initialized))
            {
                return;
            }

            EnsureInitializedCore();
        }

        void EnsureInitializedCore()
        {
            lock (syncLock)
            {
                if (!Volatile.Read(ref initialized))
                {
                    Func<UniTask<T>> f = Interlocked.Exchange(ref taskFactory, null);
                    if (f != null)
                    {
                        UniTask<T> task = f();
                        UniTask<T>.Awaiter awaiter = task.GetAwaiter();
                        if (awaiter.IsCompleted)
                        {
                            SetCompletionSource(awaiter);
                        }
                        else
                        {
                            this.awaiter = awaiter;
                            awaiter.SourceOnCompleted(continuation, this);
                        }

                        Volatile.Write(ref initialized, true);
                    }
                }
            }
        }

        void SetCompletionSource(in UniTask<T>.Awaiter awaiter)
        {
            try
            {
                T result = awaiter.GetResult();
                completionSource.TrySetResult(result);
            }
            catch (Exception ex)
            {
                completionSource.TrySetException(ex);
            }
        }

        static void SetCompletionSource(object state)
        {
            AsyncLazy<T> self = (AsyncLazy<T>)state;
            try
            {
                T result = self.awaiter.GetResult();
                self.completionSource.TrySetResult(result);
            }
            catch (Exception ex)
            {
                self.completionSource.TrySetException(ex);
            }
            finally
            {
                self.awaiter = default;
            }
        }
    }
}
