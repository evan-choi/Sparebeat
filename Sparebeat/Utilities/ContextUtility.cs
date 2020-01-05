using System;
using System.Collections.Generic;
using System.Threading;

namespace Sparebeat.Utilities
{
    public static class ContextUtility
    {
        static readonly Dictionary<object, ContextSnapshot> _snapshots =
            new Dictionary<object, ContextSnapshot>();

        public static ContextSnapshot Snapshot(this object target)
        {
            if (!_snapshots.TryGetValue(target, out ContextSnapshot snapshot))
            {
                snapshot = new ContextSnapshot(target);

                _snapshots[target] = snapshot;
            }
            else
            {
                snapshot.Shot();
            }

            return snapshot;
        }

        public class ContextSnapshot
        {
            public object Target { get; set; }

            SynchronizationContext _context;

            public ContextSnapshot(object target)
            {
                Target = target;
                Shot();
            }

            public void Shot()
            {
                _context = SynchronizationContext.Current ?? new SynchronizationContext();
            }

            public void Invoke(Action action)
            {
                _context.Send(InvokeImpl, action);
            }

            public void BeginInvoke(Action action)
            {
                _context.Post(InvokeImpl, action);
            }

            public void InvokeEvent(EventHandler eventHandler, object sender, EventArgs e)
            {
                Invoke(() => eventHandler?.Invoke(sender, e));
            }

            public void BeginInvokeEvent(EventHandler eventHandler, object sender, EventArgs e)
            {
                BeginInvoke(() => eventHandler?.Invoke(sender, e));
            }

            public void InvokeEvent<TEventArgs>(EventHandler<TEventArgs> eventHandler, object sender, TEventArgs e)
            {
                Invoke(() => eventHandler?.Invoke(sender, e));
            }

            public void BeginInvokeEvent<TEventArgs>(EventHandler<TEventArgs> eventHandler, object sender, TEventArgs e)
            {
                BeginInvoke(() => eventHandler?.Invoke(sender, e));
            }

            public TResult Invoke<TResult>(Func<TResult> function)
            {
                TResult result = default;

                _context.Send(o =>
                {
                    result = function.Invoke();
                }, null);

                return result;
            }

            public void BeginInvoke<TResult>(Func<TResult> function, AsyncCallback callback, object @object)
            {
                _context.Post(o =>
                {
                    function.BeginInvoke(callback, @object);
                }, null);
            }

            public TResult Invoke<T, TResult>(Func<T, TResult> function, T arg)
            {
                TResult result = default;

                _context.Send(o =>
                {
                    result = function.Invoke(arg);
                }, null);

                return result;
            }

            public void BeginInvoke<T, TResult>(Func<T, TResult> function, T arg, AsyncCallback callback, object @object)
            {
                _context.Post(o =>
                {
                    function.BeginInvoke(arg, callback, @object);
                }, null);
            }

            private void InvokeImpl(object state)
            {
                if (state is Action action)
                    action?.Invoke();
            }
        }
    }
}
