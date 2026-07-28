using System.Collections.Immutable;

namespace Egui;

public partial record struct Mesh
{
    /// <summary>
    /// Iterate over the triangles of this mesh, returning vertex indices.
    /// </summary>
    public IEnumerable<(uint, uint, uint)> Triangles
    {
        get
        {
            var indices = Indices;
            return Enumerable.Range(0, Indices.Length / 3).Select(i =>
            {
                var baseIndex = 3 * i;
                return (indices[baseIndex], indices[baseIndex + 1], indices[baseIndex + 2]);
            });
        }
    }
}