﻿using UnityEngine.Events;

public class EventOnDataChange1<T> : UnityEvent<T> {} // EventHandler(T modifiedData)
public class EventOnDataChange2<T> : UnityEvent<T, T> {} // EventHandler(T dataBeforeChange, T dataAfterChange)

public enum ChangeType : int
{
    Decremental = -1,
    Updating = 0,
    Incremental = 1,
}
public class EventOnDataChange3<T> : UnityEvent<ChangeType, T> {} // EventHandler(DataChangeFlag changeType, T changedData)
