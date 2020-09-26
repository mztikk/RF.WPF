using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace RF.WPF
{
    public abstract class Storage<T> : IStorage where T : new()
    {
        private const string StorageDir = "storage";

        private readonly string _name;

        protected Storage(string name) => _name = name;
        protected Storage() => _name = GetType().Name;

        public virtual T Load()
        {
            string file = GetFilePath();

            if (!File.Exists(file))
            {
                return new T();
            }

            using var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new StreamReader(stream);
            return JsonSerializer.Deserialize<T>(reader.ReadToEnd());
        }

        public virtual async Task<T> LoadAsync()
        {
            string file = GetFilePath();

            if (!File.Exists(file))
            {
                return new T();
            }

            using var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            return await JsonSerializer.DeserializeAsync<T>(stream);
        }

        protected virtual async Task SaveAsync(T obj)
        {
            string file = GetFilePath();

            var fi = new FileInfo(file);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }

            using var stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None);
            await JsonSerializer.SerializeAsync(stream, obj);
        }

        protected virtual void Save(T obj)
        {
            string file = GetFilePath();

            var fi = new FileInfo(file);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }

            using var stream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None);
            string json = JsonSerializer.Serialize(obj);
            using var writer = new StreamWriter(stream);
            writer.Write(json);
        }

        public abstract Task SaveAsync();
        public abstract void Save();

        protected string GetFilePath() => Path.ChangeExtension(Path.Combine(StorageDir, _name), "json");
        public abstract void ILoad();
    }

    public interface IStorage
    {
        void ILoad();
    }
}
