using System;

namespace MobaVR
{
    public class RequestResultCallback<T>
    {
        public Action OnStart;
        public Action<T> OnSuccess;
        public Action<string> OnError;
        public Action OnFinish;
    }
}