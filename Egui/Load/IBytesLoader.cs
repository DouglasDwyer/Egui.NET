namespace Egui.Load;

/// <summary>
/// Represents a loader capable of loading raw unstructured bytes from somewhere, e.g. from disk or network.<br/>
/// It should also provide any subsequent loaders a hint for what the bytes may represent using <see cref="BytesPoll.Ready.Mime"/>, if it can be inferred.<br/>
/// Implementations are expected to cache at least each URI.
/// </summary>
public interface IBytesLoader
{
    /// <summary>
    /// If the loader caches any data, this should return the size of that cache.
    /// </summary>
    nuint ByteSize { get; }

    /// <summary>
    /// Returns <c>true</c> if some data is currently being loaded.
    /// </summary>
    bool HasPending => false;

    /// <summary>
    /// Unique ID of this loader.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Try loading the bytes from the given <paramref name="uri"/>.<br/>
    /// Implementations should call <see cref="Context.RequestRepaint"/> to wake up the ui once the data is ready.<br/>
    /// The implementation should cache any result, so that calling this is immediate-mode safe.<br/>
    /// </summary>
    /// <exception cref="IOException">If the loader does not support loading <paramref name="uri"/>.</exception>
    /// <exception cref="IOException">If the loading process failed.</exception>
    BytesPoll Load(Context context, string uri);

    /// <summary>
    /// Forget the given <paramref name="uri"/>.<br/>
    /// If <paramref name="uri"/> is cached, it should be evicted from cache,
    /// so that it may be fulled reloaded.
    /// </summary>
    void Forget(string uri);

    /// <summary>
    /// Forget all URIs ever given to this loader.<br/>
    /// If the loader caches any URIs, the entire cache should be clared,
    /// so that all of them may be fully reloaded.
    /// </summary>
    void ForgetAll();

    /// <summary>
    /// Implementations may use this to perform work at the end of a frame,
    /// such as evicting unused entries from a cache.
    /// </summary>
    void EndPass(ulong passIndex) { }
}