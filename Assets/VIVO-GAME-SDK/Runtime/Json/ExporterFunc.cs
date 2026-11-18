

public delegate void ExporterFunc<T>(T obj, JsonWriter writer);

internal delegate void ExporterFunc(object obj, JsonWriter writer);
