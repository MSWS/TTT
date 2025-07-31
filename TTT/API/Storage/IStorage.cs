namespace TTT.API.Storage;

public interface IStorage<T> where T : class {
  Task<T> Load();
}