﻿// <copyright file="GPUBufferFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories
{
    // ReSharper disable RedundantNameQualifier
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.OpenGL;
    using Velaptor.OpenGL.Buffers;
    using Velaptor.Reactables.Core;
    using Velaptor.Reactables.ReactableData;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Creates singleton instances of <see cref="TextureGPUBuffer"/> and <see cref="FontGPUBuffer"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class GPUBufferFactory
    {
        private static IGPUBuffer<SpriteBatchItem>? textureBuffer;
        private static IGPUBuffer<FontGlyphBatchItem>? fontBuffer;
        private static IGPUBuffer<RectShape>? rectBuffer;

        /// <summary>
        /// Creates an instance of the <see cref="TextureGPUBuffer"/> class.
        /// </summary>
        /// <returns>A GPU buffer class.</returns>
        /// <remarks>
        ///     The instance is a singleton.  Every call to this method will return the same instance.
        /// </remarks>
        public static IGPUBuffer<SpriteBatchItem> CreateTextureGPUBuffer()
        {
            if (textureBuffer is not null)
            {
                return textureBuffer;
            }

            var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
            var glInvokerExtensions = IoC.Container.GetInstance<IOpenGLService>();
            var glInitReactor = IoC.Container.GetInstance<IReactable<GLInitData>>();
            var shutDownReactor = IoC.Container.GetInstance<IReactable<ShutDownData>>();

            textureBuffer = new TextureGPUBuffer(glInvoker, glInvokerExtensions, glInitReactor, shutDownReactor);

            return textureBuffer;
        }

        /// <summary>
        /// Creates an instance of the <see cref="FontGPUBuffer"/> class.
        /// </summary>
        /// <returns>A GPU buffer class.</returns>
        /// <remarks>
        ///     The instance is a singleton.  Every call to this method will return the same instance.
        /// </remarks>
        public static IGPUBuffer<FontGlyphBatchItem> CreateFontGPUBuffer()
        {
            if (fontBuffer is not null)
            {
                return fontBuffer;
            }

            var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
            var glInvokerExtensions = IoC.Container.GetInstance<IOpenGLService>();
            var glInitReactor = IoC.Container.GetInstance<IReactable<GLInitData>>();
            var shutDownReactor = IoC.Container.GetInstance<IReactable<ShutDownData>>();

            fontBuffer = new FontGPUBuffer(glInvoker, glInvokerExtensions, glInitReactor, shutDownReactor);

            return fontBuffer;
        }

        /// <summary>
        /// Creates an instance of the <see cref="RectGPUBuffer"/> class.
        /// </summary>
        /// <returns>A GPU buffer class.</returns>
        /// <remarks>
        ///     The instance is a singleton.  Every call to this method will return the same instance.
        /// </remarks>
        public static IGPUBuffer<RectShape> CreateRectGPUBuffer()
        {
            if (rectBuffer is not null)
            {
                return rectBuffer;
            }

            var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
            var glInvokerExtensions = IoC.Container.GetInstance<IOpenGLService>();
            var glInitReactor = IoC.Container.GetInstance<IReactable<GLInitData>>();
            var shutDownReactor = IoC.Container.GetInstance<IReactable<ShutDownData>>();

            rectBuffer = new RectGPUBuffer(glInvoker, glInvokerExtensions, glInitReactor, shutDownReactor);

            return rectBuffer;
        }
    }
}
