using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct FirstVertex(Vector3 position, Vector2 textureCoordinate, Color color) : IVertexType {
    public Vector3 Position = position;
    public Vector2 TextureCoordinate = textureCoordinate;
    public Color Color = color;
    public static readonly VertexDeclaration VertexDeclaration;

    readonly VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

    public override readonly int GetHashCode() {
        return HashCode.Combine(Position, TextureCoordinate, Color);
    }

    public override readonly string ToString() {
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

    public override readonly bool Equals(object? obj) {
        if (obj == null)
            return false;

        if (obj.GetType() != base.GetType())
            return false;

        return this == ((FirstVertex)obj);
    }

    static FirstVertex() {
        int offset = 0;
        var elements = new VertexElement[] {
            GetVertexElement(ref offset, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            GetVertexElement(ref offset, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            GetVertexElement(ref offset, VertexElementFormat.Color, VertexElementUsage.Color, 0),
        };
        VertexDeclaration = new VertexDeclaration(elements);
    }
    private static VertexElement GetVertexElement(ref int offset, VertexElementFormat f, VertexElementUsage u, int usageIndex) {
        return new(OffsetInline(ref offset, Offsets[f]), f, u, usageIndex);
    }
    private static int OffsetInline(ref int value, int offset) {
        int old = value;
        value += offset;
        return old;
    }
    private static readonly Dictionary<VertexElementFormat, int> Offsets = new() {
        [VertexElementFormat.Single] = sizeof(float),
        [VertexElementFormat.Vector2] = sizeof(float) * 2,
        [VertexElementFormat.Vector3] = sizeof(float) * 3,
        [VertexElementFormat.Vector4] = sizeof(float) * 4,
        [VertexElementFormat.Color] = sizeof(int),
        [VertexElementFormat.Byte4] = sizeof(byte) * 4,
        [VertexElementFormat.Short2] = sizeof(short) * 2,
        [VertexElementFormat.Short4] = sizeof(short) * 4,
        [VertexElementFormat.NormalizedShort2] = sizeof(short) * 2,
        [VertexElementFormat.NormalizedShort4] = sizeof(short) * 4,
        [VertexElementFormat.HalfVector2] = sizeof(short) * 2,
        [VertexElementFormat.HalfVector4] = sizeof(short) * 4,
    };
}
