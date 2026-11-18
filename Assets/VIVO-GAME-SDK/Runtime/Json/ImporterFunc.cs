
public delegate TValue ImporterFunc<TJson, TValue>(TJson input);

internal delegate object ImporterFunc(object input);
