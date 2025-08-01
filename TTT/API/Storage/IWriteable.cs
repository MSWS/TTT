namespace TTT.API.Storage;

public interface IWriteable<in T> where T : class {
  Task Write(T newData);
}