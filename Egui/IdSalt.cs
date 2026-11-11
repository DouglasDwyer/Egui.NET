namespace Egui;

public partial record struct IdSalt
{
    /// <summary>
    /// Create a new <see cref="IdSalt"/> by hashing a string.
    /// </summary>
    public IdSalt(string source)
    {
        this = EguiMarshal.Call<string, IdSalt>(EguiFn.egui_id_salt_IdSalt_new, source);
    }

    /// <summary>
    /// Converts a string to an <see cref="IdSalt"/>.
    /// </summary>
    public static implicit operator IdSalt(string source) => new IdSalt(source);
}
