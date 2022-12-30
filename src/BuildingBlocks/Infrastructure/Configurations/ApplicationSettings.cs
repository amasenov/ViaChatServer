using System;
using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Configurations
{
    // This ApplicationSettings implementation is called "double check lock". It is safe
    // in multithreaded environment and provides lazy initialization for the
    // ApplicationSettings object.
    public sealed class ApplicationSettings
    {
        private ApplicationSettings(string databaseConnectionString, Uri authorityUrl)
        {
            if (databaseConnectionString.IsEmpty())
            {
                throw new ArgumentNullException(nameof(databaseConnectionString));
            }

            DatabaseConnectionString = databaseConnectionString;
            AuthorityUrl = authorityUrl;
        }

        private static ApplicationSettings _instance = null;

        // We now have a lock object that will be used to synchronize threads
        // during first access to the ApplicationSettings.
        private static readonly object _lock = new object();

        public static ApplicationSettings GetInstance(string databaseConnectionString, Uri authorityUrl)
        {
            // This conditional is needed to prevent threads stumbling over the
            // lock once the instance is ready.
            if (_instance == null)
            {
                // Now, imagine that the program has just been launched. Since
                // there's no ApplicationSettings instance yet, multiple threads can
                // simultaneously pass the previous conditional and reach this
                // point almost at the same time. The first of them will acquire
                // lock and will proceed further, while the rest will wait here.
                lock (_lock)
                {
                    // The first thread to acquire the lock, reaches this
                    // conditional, goes inside and creates the ApplicationSettings
                    // instance. Once it leaves the lock block, a thread that
                    // might have been waiting for the lock release may then
                    // enter this section. But since the ApplicationSettings field is
                    // already initialized, the thread won't create a new
                    // object.
                    if (_instance == null)
                    {
                        _instance = new ApplicationSettings(databaseConnectionString, authorityUrl);
                    }
                }
            }
            return _instance;
        }
        public Uri AuthorityUrl { get; }

        public string DatabaseConnectionString { get; }
    }
}
