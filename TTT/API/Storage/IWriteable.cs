namespace TTT.API.Storage;

public interface IWriteable<in T> where T : class {
  Task Save();
  Task Write(T newData);
}