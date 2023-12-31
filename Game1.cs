﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProject;

public class Game1 : Game {
    public Game1() {
        _graphics = new GraphicsDeviceManager(this) {
            GraphicsProfile = GraphicsProfile.HiDef
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize() {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent() {
        _vertices = new FirstVertex[_initialVertices];
        _indices = new uint[_initialIndices];

        GenerateIndexArray();

        _vertexBuffer = new DynamicVertexBuffer(GraphicsDevice, typeof(FirstVertex), _vertices.Length, BufferUsage.WriteOnly);

        _indexBuffer = new IndexBuffer(GraphicsDevice, typeof(uint), _indices.Length, BufferUsage.WriteOnly);
        _indexBuffer.SetData(_indices);

        _effect = Content.Load<Effect>("first-shader");
        _texture = Content.Load<Texture2D>("image");
    }

    protected override void Update(GameTime gameTime) {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        Begin();
        Draw(_texture, new Vector2(100f, 100f), Color.White);
        Draw(_texture, new Vector2(200f, 300f), Color.White);
        End();

        base.Draw(gameTime);
    }

    public void Begin(Matrix? view = null) {
        if (view != null) {
            _view = view.Value;
        } else {
            _view = Matrix.Identity;
        }

        Viewport viewport = GraphicsDevice.Viewport;
        _projection = Matrix.CreateOrthographicOffCenter(viewport.X, viewport.Width, viewport.Height, viewport.Y, 0, 1);
    }

    public void Draw(Texture2D texture, Vector2 xy, Color? color = null) {
        EnsureSizeOrDouble(ref _vertices, _vertexCount + 4);
        _indicesChanged = EnsureSizeOrDouble(ref _indices, _indexCount + 6) || _indicesChanged;

        Vector2 topLeft = xy + new Vector2(0f, 0f);
        Vector2 topRight = xy + new Vector2(texture.Width, 0f);
        Vector2 bottomRight = xy + new Vector2(texture.Width, texture.Height);
        Vector2 bottomLeft = xy + new Vector2(0f, texture.Height);

        _vertices[_vertexCount + 0] = new FirstVertex(
            new Vector3(topLeft, 0f),
            new Vector2(0f, 0f),
            color ?? Color.White
        );
        _vertices[_vertexCount + 1] = new FirstVertex(
            new Vector3(topRight, 0f),
            new Vector2(1f, 0f),
            color ?? Color.White
        );
        _vertices[_vertexCount + 2] = new FirstVertex(
            new Vector3(bottomRight, 0f),
            new Vector2(1f, 1f),
            color ?? Color.White
        );
        _vertices[_vertexCount + 3] = new FirstVertex(
            new Vector3(bottomLeft, 0f),
            new Vector2(0f, 1f),
            color ?? Color.White
        );

        _triangleCount += 2;
        _vertexCount += 4;
        _indexCount += 6;
    }

    public void End() {
        Flush();

        // TODO: Restore old states like rasterizer, depth stencil, blend state?
    }

    private void Flush() {
        if (_triangleCount == 0) return;

        if (_indicesChanged) {
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();

            _vertexBuffer = new DynamicVertexBuffer(GraphicsDevice, typeof(FirstVertex), _vertices.Length, BufferUsage.WriteOnly);

            GenerateIndexArray();

            _indexBuffer = new IndexBuffer(GraphicsDevice, typeof(uint), _indices.Length, BufferUsage.WriteOnly);
            _indexBuffer.SetData(_indices);

            _indicesChanged = false;
        }

        _effect.Parameters["view_projection"].SetValue(_view * _projection);
        _effect.CurrentTechnique.Passes[0].Apply();

        _vertexBuffer.SetData(_vertices);
        GraphicsDevice.SetVertexBuffer(_vertexBuffer);

        GraphicsDevice.Indices = _indexBuffer;

        GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        GraphicsDevice.DepthStencilState = DepthStencilState.None;
        GraphicsDevice.BlendState = BlendState.AlphaBlend;

        GraphicsDevice.Textures[0] = _texture;
        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _triangleCount);

        _triangleCount = 0;
        _vertexCount = 0;
        _indexCount = 0;
    }

    private bool EnsureSizeOrDouble<T>(ref T[] array, int neededCapacity) {
        if (array.Length < neededCapacity) {
            Array.Resize(ref array, array.Length * 2);
            return true;
        }
        return false;
    }

    private void GenerateIndexArray() {
        for (uint i = Floor(_fromIndex, 6, 6), j = Floor(_fromIndex, 6, 4); i < _indices.Length; i += 6, j += 4) {
            _indices[i + 0] = j + 0;
            _indices[i + 1] = j + 1;
            _indices[i + 2] = j + 3;
            _indices[i + 3] = j + 1;
            _indices[i + 4] = j + 2;
            _indices[i + 5] = j + 3;
        }
        _fromIndex = _indices.Length;
    }

    private uint Floor(int value, float div, uint mul) {
        return (uint)MathF.Floor(value / div) * mul;
    }

    private GraphicsDeviceManager _graphics;

    private const int _initialSprites = 2048;
    private const int _initialVertices = _initialSprites * 4;
    private const int _initialIndices = _initialSprites * 6;

    private Effect _effect = null!;
    private Texture2D _texture = null!;
    private Matrix _view;
    private Matrix _projection;

    private FirstVertex[] _vertices = null!;
    private uint[] _indices = null!;
    private DynamicVertexBuffer _vertexBuffer = null!;
    private IndexBuffer _indexBuffer = null!;

    private int _triangleCount = 0;
    private int _vertexCount = 0;
    private int _indexCount = 0;

    private bool _indicesChanged = false;
    private int _fromIndex = 0;
}