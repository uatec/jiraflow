using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlassian.Jira;

namespace jiraflow
{
    public static class Extensions 
    {
        public static int ToTimestamp(this DateTime self)
        {
            return (Int32)(self.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        
        public static DateTime AsDateTime(this int timestamp, int dateJoinedTimestampInSeconds)
        {
            var unixEpoch = new DateTime(1970, 1, 1);
            return unixEpoch.AddSeconds(dateJoinedTimestampInSeconds + timestamp);
        }

        public static IEnumerable<T> FlattenChildren<T>(this IEnumerable<T> self, Func<T, IEnumerable<T>> selector)
        {
            if (self == null) return new T[]{};
            return self.Concat(self.SelectMany(item => selector(item).FlattenChildren(selector)));
        }

        public static bool IsCompleted(this Issue issue) 
        {
            return issue.Status == "Closed";
        }
        
        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action) 
        {
            foreach ( T item in self )
            {
                action(item);
            }
        }
        
        public static async Task ForEachAsync<T>(this IEnumerable<T> self, Func<T, Task> action) 
        {
            foreach ( T item in self )
            {
                await action(item);
            }
        }

        public static Task<TResult[]> Await<TResult>(this IEnumerable<Task<TResult>> tasks)
        {
            return Task.WhenAll(tasks);
        }

        public static Task Await(this IEnumerable<Task> tasks)
        {
            return Task.WhenAll(tasks);
        }
    }
}
