using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeLPT.Uitl.MVVM
{
    internal class WeakEventManager
    {
        private readonly Dictionary<string, List<WeakEventManager.Subscription>> _eventHandlers = new Dictionary<string, List<WeakEventManager.Subscription>>();

        public void AddEventHandler<TEventArgs>(EventHandler<TEventArgs> handler, [CallerMemberName] string eventName = null) where TEventArgs : EventArgs
        {
            if (string.IsNullOrEmpty(eventName))
                throw new ArgumentNullException(nameof(eventName));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            this.AddEventHandler(eventName, handler.Target, handler.GetMethodInfo());
        }

        public void AddEventHandler(EventHandler handler, [CallerMemberName] string eventName = null)
        {
            if (string.IsNullOrEmpty(eventName))
                throw new ArgumentNullException(nameof(eventName));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            this.AddEventHandler(eventName, handler.Target, handler.GetMethodInfo());
        }

        public void HandleEvent(object sender, object args, string eventName)
        {
            List<ValueTuple<object, MethodInfo>> valueTupleList = new List<ValueTuple<object, MethodInfo>>();
            List<WeakEventManager.Subscription> subscriptionList1 = new List<WeakEventManager.Subscription>();
            List<WeakEventManager.Subscription> subscriptionList2;
            if (this._eventHandlers.TryGetValue(eventName, out subscriptionList2))
            {
                for (int index = 0; index < subscriptionList2.Count; ++index)
                {
                    WeakEventManager.Subscription subscription = subscriptionList2[index];
                    if (subscription.Subscriber == null)
                    {
                        valueTupleList.Add(new ValueTuple<object, MethodInfo>((object)null, subscription.Handler));
                    }
                    else
                    {
                        object target = subscription.Subscriber.Target;
                        if (target == null)
                            subscriptionList1.Add(subscription);
                        else
                            valueTupleList.Add(new ValueTuple<object, MethodInfo>(target, subscription.Handler));
                    }
                }
                for (int index = 0; index < subscriptionList1.Count; ++index)
                {
                    WeakEventManager.Subscription subscription = subscriptionList1[index];
                    subscriptionList2.Remove(subscription);
                }
            }
            for (int index = 0; index < valueTupleList.Count; ++index)
            {
                ValueTuple<object, MethodInfo> valueTuple = valueTupleList[index];
                valueTuple.Item2.Invoke(valueTuple.Item1, new object[2]
                {
          sender,
          args
                });
            }
        }

        public void RemoveEventHandler<TEventArgs>(EventHandler<TEventArgs> handler, [CallerMemberName] string eventName = null) where TEventArgs : EventArgs
        {
            if (string.IsNullOrEmpty(eventName))
                throw new ArgumentNullException(nameof(eventName));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            this.RemoveEventHandler(eventName, handler.Target, (MemberInfo)handler.GetMethodInfo());
        }

        public void RemoveEventHandler(EventHandler handler, [CallerMemberName] string eventName = null)
        {
            if (string.IsNullOrEmpty(eventName))
                throw new ArgumentNullException(nameof(eventName));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            this.RemoveEventHandler(eventName, handler.Target, (MemberInfo)handler.GetMethodInfo());
        }

        private void AddEventHandler(string eventName, object handlerTarget, MethodInfo methodInfo)
        {
            List<WeakEventManager.Subscription> subscriptionList;
            if (!this._eventHandlers.TryGetValue(eventName, out subscriptionList))
            {
                subscriptionList = new List<WeakEventManager.Subscription>();
                this._eventHandlers.Add(eventName, subscriptionList);
            }
            if (handlerTarget == null)
                subscriptionList.Add(new WeakEventManager.Subscription((WeakReference)null, methodInfo));
            else
                subscriptionList.Add(new WeakEventManager.Subscription(new WeakReference(handlerTarget), methodInfo));
        }

        private void RemoveEventHandler(string eventName, object handlerTarget, MemberInfo methodInfo)
        {
            List<WeakEventManager.Subscription> subscriptionList;
            if (!this._eventHandlers.TryGetValue(eventName, out subscriptionList))
                return;
            for (int count = subscriptionList.Count; count > 0; --count)
            {
                WeakEventManager.Subscription subscription = subscriptionList[count - 1];
                if (subscription.Subscriber?.Target == handlerTarget && !(subscription.Handler.Name != methodInfo.Name))
                {
                    subscriptionList.Remove(subscription);
                    break;
                }
            }
        }

        private struct Subscription
        {
            public readonly WeakReference Subscriber;
            public readonly MethodInfo Handler;

            public Subscription(WeakReference subscriber, MethodInfo handler)
            {
                this.Subscriber = subscriber;
                MethodInfo methodInfo = handler;
                if ((object)methodInfo == null)
                    throw new ArgumentNullException(nameof(handler));
                this.Handler = methodInfo;
            }
        }
    }
}
