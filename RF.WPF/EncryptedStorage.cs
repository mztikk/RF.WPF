using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RF.WPF
{
    public abstract class EncryptedStorage<T> : IStorage where T : new()
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> s_fileLock = new ConcurrentDictionary<string, SemaphoreSlim>();

        private readonly Type _type = typeof(T);

        private static readonly string s_settingsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Assembly.GetEntryAssembly()!.GetName()!.Name!);

        private readonly string _name;

        protected readonly ILogger _logger;
        private readonly IEntropy _entropy;

        protected EncryptedStorage(string name, ILogger logger, IEntropy entropy)
        {
            _logger = logger;
            _entropy = entropy;
            _name = name;
        }

        protected EncryptedStorage(ILogger logger, IEntropy entropy)
        {
            _logger = logger;
            _entropy = entropy;
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
                byte[] data = new byte[stream.Length];
                _logger.LogTrace("Reading data from: '{path}'", file);
                stream.Read(data);

                _logger.LogDebug("Unprotecting data");
                byte[] unprotectedData = ProtectedData.Unprotect(data, _entropy.GetEntropy(), DataProtectionScope.CurrentUser);

                _logger.LogDebug("Deserializing Json from data");
                return JsonSerializer.Deserialize<T>(unprotectedData);
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

                byte[] data = new byte[stream.Length];

                _logger.LogTrace("Reading data asynchronously from: '{path}'", file);
                await stream.ReadAsync(data);

                _logger.LogDebug("Unprotecting data");
                byte[] unprotectedData = ProtectedData.Unprotect(data, _entropy.GetEntropy(), DataProtectionScope.CurrentUser);
                using var ms = new MemoryStream(unprotectedData);

                _logger.LogDebug("Deserializing Json from data asynchronously");
                return await JsonSerializer.DeserializeAsync<T>(ms);
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
                using var ms = new MemoryStream();
                _logger.LogDebug("Serializing Json asynchronously");
                await JsonSerializer.SerializeAsync(ms, obj);
                _logger.LogDebug("Protecting data");
                byte[] protectedData = ProtectedData.Protect(ms.ToArray(), _entropy.GetEntropy(), DataProtectionScope.CurrentUser);
                _logger.LogTrace("Writing data asynchronously");
                await stream.WriteAsync(protectedData);
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
                _logger.LogDebug("Serializing Json");
                string json = JsonSerializer.Serialize(obj);
                byte[] data = Encoding.UTF8.GetBytes(json);
                _logger.LogDebug("Protecting data");
                byte[] protectedData = ProtectedData.Protect(data, _entropy.GetEntropy(), DataProtectionScope.CurrentUser);
                _logger.LogTrace("Writing data");
                stream.Write(protectedData);
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

    public interface IEntropy
    {
        byte[] GetEntropy();
    }

    public sealed class AssemblyNameEntropy : IEntropy
    {
        private readonly Lazy<byte[]> _entropy;

        public AssemblyNameEntropy() => _entropy = new Lazy<byte[]>(Get);

        private static byte[] Get() => Encoding.ASCII.GetBytes(Assembly.GetExecutingAssembly().FullName);

        public byte[] GetEntropy() => _entropy.Value;
    }
}
