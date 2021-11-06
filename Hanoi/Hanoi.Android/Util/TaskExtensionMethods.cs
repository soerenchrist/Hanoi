using System.Threading.Tasks;

namespace Hanoi.Droid.Util
{
    public static class TaskExtensionMethods
    {
        public static Task<Java.Lang.Object> ToAwaitableTask(this Android.Gms.Tasks.Task task)
        {
            var tcs = new TaskCompletionSource<Java.Lang.Object>();
            var listener = new TaskCompleteListener(tcs);

            task.AddOnCompleteListener(listener);

            return tcs.Task;
        }
    }
}