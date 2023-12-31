using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct FirstVertex : IVertexType {
    public FirstVertex(Vector3 position, Vector2 textureCoordinate, Color color) {
        Position = position;
        TextureCoordinate = textureCoordinate;
        Color = color;
    }

    public Vector3 Position;
    public Vector2 TextureCoordinate;
    public Color Color;
    public static readonly VertexDeclaration VertexDeclaration;

    VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

    public override int GetHashCode() {
        unchecked {
            var hashCode = Position.GetHashCode();
            hashCode = (hashCode * 397) ^ TextureCoordinate.GetHashCode();
            hashCode = (hashCode * 397) ^ Color.GetHashCode();
            return hashCode;
        }
    }

    public override string ToString() {
        return
            "{{Position:" + Position +
            " TextureCoordinate:" + TextureCoordinate +
            " Color:" + Color +
            "}}";
    }

    public static bool operator ==(FirstVertex left, FirstVertex right) {
        return
            left.Position == right.Position &&
            left.TextureCoordinate == right.TextureCoordinate &&
            left.Color == right.Color;
    }

    public static bool operator !=(FirstVertex left, FirstVertex right) {
        return !(left == right);
    }

    public override bool Equals(object? obj) {
        if (obj == null)
            return false;

        if (obj.GetType() != base.GetType())
            return false;

        return (this == ((FirstVertex)obj));
    }

    static FirstVertex() {
        int offset = 0;
        var elements = new VertexElement[] {
            new VertexElement(OffsetInline(ref offset, sizeof(float) * 3), VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(OffsetInline(ref offset, sizeof(float) * 2), VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(offset, VertexElementFormat.Color, VertexElementUsage.Color, 0),
        };
        VertexDeclaration = new VertexDeclaration(elements);
    }
    private static int OffsetInline(ref int value, int offset) {
        int old = value;
        value += offset;
        return old;
    }
}
