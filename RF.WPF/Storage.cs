using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RF.WPF
{
    public abstract class Storage<T> : IStorage where T : new()
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> s_fileLock = new ConcurrentDictionary<string, SemaphoreSlim>();

        private readonly Type _type = typeof(T);

        private static readonly string s_settingsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Assembly.GetEntryAssembly()!.GetName()!.Name!);

        private readonly string _name;

        protected readonly ILogger _logger;

        protected Storage(string name, ILogger logger)
        {
            _logger = logger;
            _name = name;
        }

        protected Storage(ILogger logger)
        {
            _logger = logger;
            _name = GetType().Name;
        }

        public virtual T Load()
        {
            string file = GetFilePath();
            _logger.LogDebug("Loading Data from path: '{path}'", file);

            if (!File.Exists(file))
            {
                _logger.LogInformation("File: '{file}' doesn't exist, creating default new of type: '{type}'", file, _type.ToString());
                return new T();
            }

            _logger.LogTrace("Acquiring semaphore for '{file}'", file);
            SemaphoreSlim mutex = s_fileLock.GetOrAdd(file, new SemaphoreSlim(1));
            _logger.LogTrace("Waiting semaphore for '{file}'", file);
            mutex.Wait();
            try
            {
                _logger.LogTrace("Locked i/o for '{file}'", file);
                _logger.LogDebug("Creating FileStream from path: '{path}'", file);
                using var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var reader = new StreamReader(stream);
                _logger.LogDebug("Deserializing Json from path: '{path}'", file);
                return JsonSerializer.Deserialize<T>(reader.ReadToEnd());
            }
            finally
            {
                _logger.LogTrace("Releasing semaphore for '{file}'", file);
                mutex.Release();
            }
        }

        public virtual async Task<T> LoadAsync()
        {
            string file = GetFilePath();
            _logger.LogDebug("Loading Data asynchronously from path: '{path}'", file);

            if (!File.Exists(file))
            {
                _logger.LogInformation("File: '{file}' doesn't exist, creating default new of type: '{type}'", file, _type.ToString());
                return new T();
            }

            _logger.LogTrace("Acquiring semaphore for '{file}'", file);
            SemaphoreSlim mutex = s_fileLock.GetOrAdd(file, new SemaphoreSlim(1));
            _logger.LogTrace("Waiting semaphore for '{file}'", file);
            await mutex.WaitAsync();
            try
            {
                _logger.LogTrace("Locked i/o for '{file}'", file);
                _logger.LogDebug("Creating FileStream from path: '{path}'", file);
                using var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                _logger.LogDebug("Deserializing Json asynchronously from path: '{path}'", file);
                return await JsonSerializer.DeserializeAsync<T>(stream);
            }
            finally
            {
                _logger.LogTrace("Releasing semaphore for '{file}'", file);
                mutex.Release();
            }
        }

        protected virtual async Task SaveAsync(T obj)
        {
            string file = GetFilePath();
            _logger.LogDebug("Saving Data asynchronously to path: '{path}'", file);

            var fi = new FileInfo(file);
            if (!fi.Directory.Exists)
            {
                _logger.LogInformation("Directory '{directory}' for file '{file}' doesn't exist and will be created.", fi.Directory, file);
                fi.Directory.Create();
            }

            _logger.LogTrace("Acquiring semaphore for '{file}'", file);
            SemaphoreSlim mutex = s_fileLock.GetOrAdd(file, new SemaphoreSlim(1));
            _logger.LogTrace("Waiting semaphore for '{file}'", file);
            await mutex.WaitAsync();
            try
            {
                _logger.LogDebug("Creating FileStream for path: '{path}'", file);
                using var stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None);
                _logger.LogDebug("Serializing Json asynchronously to path: '{path}'", file);
                await JsonSerializer.SerializeAsync(stream, obj);
            }
            finally
            {
                _logger.LogTrace("Releasing semaphore for '{file}'", file);
                mutex.Release();
            }
        }

        protected virtual void Save(T obj)
        {
            string file = GetFilePath();
            _logger.LogDebug("Saving Data to path: '{path}'", file);

            var fi = new FileInfo(file);
            if (!fi.Directory.Exists)
            {
                _logger.LogInformation("Directory '{directory}' for file '{file}' doesn't exist and will be created.", fi.Directory, file);
                fi.Directory.Create();
            }

            _logger.LogTrace("Acquiring semaphore for '{file}'", file);
            SemaphoreSlim mutex = s_fileLock.GetOrAdd(file, new SemaphoreSlim(1));
            _logger.LogTrace("Waiting semaphore for '{file}'", file);
            mutex.Wait();
            try
            {
                _logger.LogDebug("Creating FileStream for path: '{path}'", file);
                using var stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None);
                _logger.LogDebug("Serializing Json to path: '{path}'", file);
                string json = JsonSerializer.Serialize(obj);
                using var writer = new StreamWriter(stream);
                writer.Write(json);
            }
            finally
            {
                _logger.LogTrace("Releasing semaphore for '{file}'", file);
                mutex.Release();
            }
        }

        public abstract Task SaveAsync();
        public abstract void Save();

        public string GetSettingsDir()
        {
            if (!Directory.Exists(s_settingsDir))
            {
                _logger.LogInformation("Directory '{directory}' doesn't exist and will be created", s_settingsDir);
                Directory.CreateDirectory(s_settingsDir);
            }

            return s_settingsDir;
        }

        public string GetStorageDir() => Path.Combine(GetSettingsDir(), "storage");

        public string GetFilePath() => Path.ChangeExtension(Path.Combine(GetStorageDir(), _name), "json");

        public abstract void ILoad();
    }

    public interface IStorage
    {
        void ILoad();
    }
}
