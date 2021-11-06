using Android.Gms.Tasks;
using System.Threading.Tasks;

namespace Hanoi.Droid.Util
{
    public class TaskCompleteListener : Java.Lang.Object, IOnCompleteListener
    {
        private readonly TaskCompletionSource<Java.Lang.Object> _taskCompletionSource;

        public TaskCompleteListener(TaskCompletionSource<Java.Lang.Object> tcs)
        {
            _taskCompletionSource = tcs;
        }

        public void OnComplete(Android.Gms.Tasks.Task task)
        {
            if (task.IsCanceled)
                _taskCompletionSource.SetCanceled();
            else if (task.IsSuccessful)
                _taskCompletionSource.SetResult(task.Result);
            else
                this._taskCompletionSource.SetException(task.Exception);
        }
    }
}