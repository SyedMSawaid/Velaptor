﻿// <copyright file="FontLoaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Fonts
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.IO.Abstractions;
    using Moq;
    using Velaptor.Content;
    using Velaptor.Content.Caching;
    using Velaptor.Content.Exceptions;
    using Velaptor.Content.Factories;
    using Velaptor.Content.Fonts;
    using Velaptor.Graphics;
    using Velaptor.Services;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="FontLoader"/> class.
    /// </summary>
    public class FontLoaderTests
    {
        private const int FontSize = 12;
        private const string FontExtension = ".ttf";
        private const string FontDirName = "fonts";
        private const string ContentDirPath = @"C:/content/";
        private const string FontContentName = "test-font";
        private readonly string metaData = $"size:{FontSize}";
        private readonly string fontContentDirPath = $@"{ContentDirPath}{FontDirName}/";
        private readonly string fontFilePath;
        private readonly string filePathWithMetaData;
        private readonly string contentNameWithMetaData;
        private readonly GlyphMetrics[] glyphMetricData;
        private readonly Mock<IFontAtlasService> mockFontAtlasService;
        private readonly Mock<IEmbeddedResourceLoaderService<Stream?>> mockEmbeddedFontResourceService;
        private readonly Mock<IPathResolver> mockFontPathResolver;
        private readonly Mock<IItemCache<string, ITexture>> mockTextureCache;
        private readonly Mock<IFontFactory> mockFontFactory;
        private readonly Mock<IFontMetaDataParser> mockFontMetaDataParser;
        private readonly Mock<IPath> mockPath;
        private readonly Mock<ITexture> mockFontAtlasTexture;
        private readonly Mock<IDirectory> mockDirectory;
        private readonly Mock<IFile> mockFile;
        private readonly Mock<IFileStreamFactory> mockFileStream;
        private readonly Mock<IFont> mockFont;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontLoaderTests"/> class.
        /// </summary>
        public FontLoaderTests()
        {
            this.fontFilePath = $@"{ContentDirPath}{FontDirName}\{FontContentName}{FontExtension}";
            this.filePathWithMetaData = $"{this.fontFilePath}|{this.metaData}";
            this.contentNameWithMetaData = $"{FontContentName}|{this.metaData}";

            this.mockFontAtlasTexture = new Mock<ITexture>();

            this.mockFont = new Mock<IFont>();

            this.glyphMetricData = new[]
                {
                    GenerateMetricData(0),
                    GenerateMetricData(10),
                };

            this.mockFontAtlasService = new Mock<IFontAtlasService>();
            this.mockFontAtlasService.Setup(m => m.CreateFontAtlas(this.fontFilePath, FontSize))
                .Returns(() => (It.IsAny<ImageData>(), this.glyphMetricData));

            this.mockEmbeddedFontResourceService = new Mock<IEmbeddedResourceLoaderService<Stream?>>();

            // Mock for full file paths with metadata
            this.mockFontPathResolver = new Mock<IPathResolver>();
            this.mockFontPathResolver.SetupGet(p => p.RootDirectoryPath).Returns(ContentDirPath);
            this.mockFontPathResolver.SetupGet(p => p.ContentDirectoryName).Returns(FontDirName);

            this.mockFontPathResolver.Setup(m => m.ResolveFilePath(FontContentName)).Returns(this.fontFilePath);

            // Mock for both full file paths and content names with metadata
            this.mockTextureCache = new Mock<IItemCache<string, ITexture>>();
            this.mockTextureCache.Setup(m => m.GetItem(this.filePathWithMetaData))
                .Returns(this.mockFontAtlasTexture.Object);

            // Mock for both full file paths and content names with metadata
            this.mockFontFactory = new Mock<IFontFactory>();
            this.mockFontFactory.Setup(m =>
                    m.Create(this.mockFontAtlasTexture.Object,
                        FontContentName,
                        this.fontFilePath,
                        FontSize,
                        It.IsAny<bool>(),
                        this.glyphMetricData))
                .Returns(this.mockFont.Object);

            this.mockFontMetaDataParser = new Mock<IFontMetaDataParser>();
            // Mock for full file paths with metadata
            this.mockFontMetaDataParser.Setup(m => m.Parse(this.filePathWithMetaData))
                .Returns(new FontMetaDataParseResult(
                    true,
                    true,
                    this.fontFilePath,
                    this.metaData,
                    FontSize));

            // Mock for content names with metadata
            this.mockFontMetaDataParser.Setup(m => m.Parse(this.contentNameWithMetaData))
                .Returns(new FontMetaDataParseResult(
                    true,
                    true,
                    FontContentName,
                    this.metaData,
                    FontSize));

            this.mockDirectory = new Mock<IDirectory>();

            this.mockFile = new Mock<IFile>();
            this.mockFile.Setup(m => m.Exists(this.fontFilePath)).Returns(true);

            this.mockFileStream = new Mock<IFileStreamFactory>();

            // Mock for both full file paths and content names with metadata
            this.mockPath = new Mock<IPath>();
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{FontContentName}"))
                .Returns(FontContentName);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension($"{FontContentName}{FontExtension}"))
                .Returns(FontContentName);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension(this.fontFilePath))
                .Returns(FontContentName);
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WithNullFontAtlasServiceParam_ThrowsException()
        {
            // Arrange, Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new FontLoader(
                    null,
                    this.mockEmbeddedFontResourceService.Object,
                    this.mockFontPathResolver.Object,
                    this.mockTextureCache.Object,
                    this.mockFontFactory.Object,
                    this.mockFontMetaDataParser.Object,
                    this.mockDirectory.Object,
                    this.mockFile.Object,
                    this.mockFileStream.Object,
                    this.mockPath.Object);
            }, "The parameter must not be null. (Parameter 'fontAtlasService')");
        }

        [Fact]
        public void Ctor_WithNullEmbeddedFontResourceService_ThrowsException()
        {
            // Arrange, Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new FontLoader(
                    this.mockFontAtlasService.Object,
                    null,
                    this.mockFontPathResolver.Object,
                    this.mockTextureCache.Object,
                    this.mockFontFactory.Object,
                    this.mockFontMetaDataParser.Object,
                    this.mockDirectory.Object,
                    this.mockFile.Object,
                    this.mockFileStream.Object,
                    this.mockPath.Object);
            }, "The parameter must not be null. (Parameter 'embeddedFontResourceService')");
        }

        [Fact]
        public void Ctor_WithNullFontPathResolver_ThrowsException()
        {
            // Arrange, Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new FontLoader(
                    this.mockFontAtlasService.Object,
                    this.mockEmbeddedFontResourceService.Object,
                    null,
                    this.mockTextureCache.Object,
                    this.mockFontFactory.Object,
                    this.mockFontMetaDataParser.Object,
                    this.mockDirectory.Object,
                    this.mockFile.Object,
                    this.mockFileStream.Object,
                    this.mockPath.Object);
            }, "The parameter must not be null. (Parameter 'fontPathResolver')");
        }

        [Fact]
        public void Ctor_WithNullTextureCache_ThrowsException()
        {
            // Arrange, Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new FontLoader(
                    this.mockFontAtlasService.Object,
                    this.mockEmbeddedFontResourceService.Object,
                    this.mockFontPathResolver.Object,
                    null,
                    this.mockFontFactory.Object,
                    this.mockFontMetaDataParser.Object,
                    this.mockDirectory.Object,
                    this.mockFile.Object,
                    this.mockFileStream.Object,
                    this.mockPath.Object);
            }, "The parameter must not be null. (Parameter 'textureCache')");
        }

        [Fact]
        public void Ctor_WithNullFontFactory_ThrowsException()
        {
            // Arrange, Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new FontLoader(
                    this.mockFontAtlasService.Object,
                    this.mockEmbeddedFontResourceService.Object,
                    this.mockFontPathResolver.Object,
                    this.mockTextureCache.Object,
                    null,
                    this.mockFontMetaDataParser.Object,
                    this.mockDirectory.Object,
                    this.mockFile.Object,
                    this.mockFileStream.Object,
                    this.mockPath.Object);
            }, "The parameter must not be null. (Parameter 'fontFactory')");
        }

        [Fact]
        public void Ctor_WithNullFontMetaDataParser_ThrowsException()
        {
            // Arrange, Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new FontLoader(
                    this.mockFontAtlasService.Object,
                    this.mockEmbeddedFontResourceService.Object,
                    this.mockFontPathResolver.Object,
                    this.mockTextureCache.Object,
                    this.mockFontFactory.Object,
                    null,
                    this.mockDirectory.Object,
                    this.mockFile.Object,
                    this.mockFileStream.Object,
                    this.mockPath.Object);
            }, "The parameter must not be null. (Parameter 'fontMetaDataParser')");
        }

        [Fact]
        public void Ctor_WithNullDirectoryParam_ThrowsException()
        {
            // Arrange, Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new FontLoader(
                    this.mockFontAtlasService.Object,
                    this.mockEmbeddedFontResourceService.Object,
                    this.mockFontPathResolver.Object,
                    this.mockTextureCache.Object,
                    this.mockFontFactory.Object,
                    this.mockFontMetaDataParser.Object,
                    null,
                    this.mockFile.Object,
                    this.mockFileStream.Object,
                    this.mockPath.Object);
            }, "The parameter must not be null. (Parameter 'directory')");
        }

        [Fact]
        public void Ctor_WithNullFileParam_ThrowsException()
        {
            // Arrange, Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new FontLoader(
                    this.mockFontAtlasService.Object,
                    this.mockEmbeddedFontResourceService.Object,
                    this.mockFontPathResolver.Object,
                    this.mockTextureCache.Object,
                    this.mockFontFactory.Object,
                    this.mockFontMetaDataParser.Object,
                    this.mockDirectory.Object,
                    null,
                    this.mockFileStream.Object,
                    this.mockPath.Object);
            }, "The parameter must not be null. (Parameter 'file')");
        }

        [Fact]
        public void Ctor_WithNullNullParam_ThrowsException()
        {
            // Arrange, Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new FontLoader(
                    this.mockFontAtlasService.Object,
                    this.mockEmbeddedFontResourceService.Object,
                    this.mockFontPathResolver.Object,
                    this.mockTextureCache.Object,
                    this.mockFontFactory.Object,
                    this.mockFontMetaDataParser.Object,
                    this.mockDirectory.Object,
                    this.mockFile.Object,
                    null,
                    this.mockPath.Object);
            }, "The parameter must not be null. (Parameter 'fileStream')");
        }

        [Fact]
        public void Ctor_WithNullPathParam_ThrowsException()
        {
            // Arrange, Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var unused = new FontLoader(
                    this.mockFontAtlasService.Object,
                    this.mockEmbeddedFontResourceService.Object,
                    this.mockFontPathResolver.Object,
                    this.mockTextureCache.Object,
                    this.mockFontFactory.Object,
                    this.mockFontMetaDataParser.Object,
                    this.mockDirectory.Object,
                    this.mockFile.Object,
                    this.mockFileStream.Object,
                    null);
            }, "The parameter must not be null. (Parameter 'path')");
        }

        [Fact]
        public void Ctor_WhenFontContentDirectoryDoesNotExist_CreatesFontContentDirectory()
        {
            // Arrange
            var defaultRegularFontName = $"TimesNewRoman-Regular{FontExtension}";
            var defaultBoldFontName = $"TimesNewRoman-Bold{FontExtension}";
            var defaultItalicFontName = $"TimesNewRoman-Italic{FontExtension}";
            var defaultBoldItalicFontName = $"TimesNewRoman-BoldItalic{FontExtension}";

            var defaultRegularFontFilePath = $"{this.fontContentDirPath}{defaultRegularFontName}";
            var defaultBoldFontFilePath = $"{this.fontContentDirPath}{defaultBoldFontName}";
            var defaultItalicFontFilePath = $"{this.fontContentDirPath}{defaultItalicFontName}";
            var defaultBoldItalicFontFilePath = $"{this.fontContentDirPath}{defaultBoldItalicFontName}";

            var mockRegularFontFileStream = MockLoadResource(defaultRegularFontName);
            var mockBoldFontFileStream = MockLoadResource(defaultBoldFontName);
            var mockItalicFontFileStream = MockLoadResource(defaultItalicFontName);
            var mockBoldItalicFontFileStream = MockLoadResource(defaultBoldItalicFontName);

            var mockCopyToRegularStream = MockCopyToStream(defaultRegularFontFilePath);
            var mockCopyToBoldStream = MockCopyToStream(defaultBoldFontFilePath);
            var mockCopyToItalicStream = MockCopyToStream(defaultItalicFontFilePath);
            var mockCopyToBoldItalicStream = MockCopyToStream(defaultBoldItalicFontFilePath);

            this.mockDirectory.Setup(m => m.Exists(ContentDirPath)).Returns(false);
            this.mockDirectory.Setup(m => m.Exists(this.fontContentDirPath)).Returns(false);

            this.mockFile.Setup(m => m.Exists(defaultRegularFontFilePath)).Returns(false);
            this.mockFile.Setup(m => m.Exists(defaultBoldFontFilePath)).Returns(false);
            this.mockFile.Setup(m => m.Exists(defaultItalicFontFilePath)).Returns(true);
            this.mockFile.Setup(m => m.Exists(defaultBoldItalicFontFilePath)).Returns(false);

            this.mockPath.SetupGet(p => p.AltDirectorySeparatorChar).Returns('/');

            // Act
            CreateLoader();

            // Assert
            this.mockFontPathResolver.VerifyGet(p => p.RootDirectoryPath, Times.Once);
            this.mockFontPathResolver.VerifyGet(p => p.ContentDirectoryName, Times.Once);

            // Check for directory existence
            this.mockDirectory.Verify(m => m.Exists(ContentDirPath), Times.Once);
            this.mockDirectory.Verify(m => m.Exists(this.fontContentDirPath), Times.Once);

            // Each file was verified if it exists
            this.mockFile.Verify(m => m.Exists(defaultRegularFontFilePath), Times.Once);
            this.mockFile.Verify(m => m.Exists(defaultBoldFontFilePath), Times.Once);
            this.mockFile.Verify(m => m.Exists(defaultItalicFontFilePath), Times.Once);
            this.mockFile.Verify(m => m.Exists(defaultBoldItalicFontFilePath), Times.Once);

            // Check that each file was created
            this.mockFileStream.Verify(m =>
                m.Create(defaultRegularFontFilePath, FileMode.Create, FileAccess.Write),
                    Times.Once);
            this.mockFileStream.Verify(m =>
                m.Create(defaultBoldFontFilePath, FileMode.Create, FileAccess.Write),
                    Times.Once);
            this.mockFileStream.Verify(m =>
                m.Create(defaultItalicFontFilePath, FileMode.Create, FileAccess.Write),
                    Times.Never);
            this.mockFileStream.Verify(m =>
                m.Create(defaultBoldItalicFontFilePath, FileMode.Create, FileAccess.Write),
                    Times.Once);

            mockRegularFontFileStream.Verify(m => m.CopyTo(mockCopyToRegularStream.Object, It.IsAny<int>()), Times.Once);
            mockBoldFontFileStream.Verify(m => m.CopyTo(mockCopyToBoldStream.Object, It.IsAny<int>()), Times.Once);
            mockItalicFontFileStream.Verify(m => m.CopyTo(mockCopyToItalicStream.Object, It.IsAny<int>()), Times.Never);
            mockBoldItalicFontFileStream.Verify(m => m.CopyTo(mockCopyToBoldItalicStream.Object, It.IsAny<int>()), Times.Once);
        }
        #endregion

        #region Method Tests
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Load_WithNullOrEmptyParam_ThrowsException(string contentName)
        {
            // Arrange
            var loader = CreateLoader();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                loader.Load(contentName);
            }, "The parameter must not be null. (Parameter 'contentWithMetaData')");
        }

        [Fact]
        public void Load_WithInvalidMetaData_ThrowsException()
        {
            // Arrange
            const string contentName = "invalid-metadata";
            const string invalidMetaData = "size-12";

            var expected = $"The metadata '{invalidMetaData}' is invalid when loading '{contentName}'.";
            expected += $"{Environment.NewLine}\tExpected MetaData Syntax: size:<font-size>";
            expected += $"{Environment.NewLine}\tExample: size:12";

            this.mockFontMetaDataParser.Setup(m => m.Parse(contentName))
                .Returns(new FontMetaDataParseResult(
                    true,
                    false,
                    string.Empty,
                    invalidMetaData,
                    FontSize));
            var loader = CreateLoader();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<CachingMetaDataException>(() =>
            {
                loader.Load(contentName);
            }, expected);
        }

        [Fact]
        public void Load_WithNoMetaData_ThrowsException()
        {
            // Arrange
            this.mockFontMetaDataParser.Setup(m => m.Parse(It.IsAny<string>()))
                .Returns(new FontMetaDataParseResult(
                    false,
                    false,
                    string.Empty,
                    string.Empty,
                    0));

            var expected = "The font content item 'missing-metadata' must have metadata post fixed to the";
            expected += " end of a content name or full file path";

            var loader = CreateLoader();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<CachingMetaDataException>(() =>
            {
                loader.Load("missing-metadata");
            }, expected);
        }

        [Fact]
        public void Load_WhenContentItemDoesNotExist_ThrowsException()
        {
            // Arrange
            this.mockFile.Setup(m => m.Exists(this.fontFilePath)).Returns(false);

            var expected = $"The font content item '{this.fontFilePath}' does not exist.";

            var loader = CreateLoader();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<FileNotFoundException>(() =>
            {
                loader.Load(this.filePathWithMetaData);
            }, expected);
        }

        [Fact]
        public void Load_WithFileNameAndExtensionOnly_LoadsFontFromContentDirectory()
        {
            // Arrange
            var fileNameWithExt = $"{FontContentName}{FontExtension}";
            var fileNameWithExtAndMetaData = $"{fileNameWithExt}|size:12";

            this.mockFontMetaDataParser.Setup(m => m.Parse(fileNameWithExtAndMetaData))
                .Returns(new FontMetaDataParseResult(
                    true,
                    true,
                    fileNameWithExt,
                    this.metaData,
                    FontSize));
            this.mockFontPathResolver.Setup(m => m.ResolveFilePath(FontContentName)).Returns(this.fontFilePath);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension(fileNameWithExt)).Returns(FontContentName);
            this.mockPath.Setup(m => m.GetFileNameWithoutExtension(fileNameWithExtAndMetaData)).Returns(FontContentName);
            this.mockFontAtlasService.Setup(m => m.CreateFontAtlas(this.fontFilePath, FontSize))
                .Returns(() => (It.IsAny<ImageData>(), this.glyphMetricData));

            var loader = CreateLoader();

            // Act
            var actual = loader.Load(fileNameWithExtAndMetaData);

            // Assert
            this.mockFontMetaDataParser.Verify(m => m.Parse(fileNameWithExtAndMetaData), Times.Once);
            this.mockPath.Verify(m => m.GetFileNameWithoutExtension(fileNameWithExt), Times.Once);
            this.mockPath.Verify(m => m.GetFileNameWithoutExtension(this.fontFilePath), Times.Once);
            this.mockFontAtlasService.Verify(m => m.CreateFontAtlas(this.fontFilePath, FontSize), Times.Once);
            this.mockTextureCache.Verify(m => m.GetItem(this.filePathWithMetaData), Times.Once);
            this.mockFontFactory.Verify(m =>
                    m.Create(
                        this.mockFontAtlasTexture.Object,
                        FontContentName,
                        this.fontFilePath,
                        FontSize,
                        It.IsAny<bool>(),
                        this.glyphMetricData),
                Times.Once);

            Assert.Same(this.mockFont.Object, actual);
        }

        [Fact]
        public void Load_WhenUsingFullFilePathWithMetaData_LoadsFont()
        {
            // Arrange
            var loader = CreateLoader();

            // Act
            var actual = loader.Load(this.filePathWithMetaData);

            // Assert
            this.mockFontMetaDataParser.Verify(m => m.Parse(this.filePathWithMetaData), Times.Once);
            this.mockPath.Verify(m => m.GetFileNameWithoutExtension(this.fontFilePath), Times.Once);
            this.mockFontAtlasService.Verify(m => m.CreateFontAtlas(this.fontFilePath, FontSize), Times.Once);
            this.mockTextureCache.Verify(m => m.GetItem(this.filePathWithMetaData), Times.Once);
            this.mockFontFactory.Verify(m =>
                    m.Create(
                        this.mockFontAtlasTexture.Object,
                        FontContentName,
                        this.fontFilePath,
                        FontSize,
                        It.IsAny<bool>(),
                        this.glyphMetricData),
                Times.Once);

            Assert.Same(this.mockFont.Object, actual);
        }

        [Fact]
        public void Load_WhenUsingContentNameWithMetaData_LoadsFont()
        {
            // Arrange
            var loader = CreateLoader();

            // Act
            var actual = loader.Load(this.contentNameWithMetaData);

            // Assert
            this.mockFontMetaDataParser.Verify(m => m.Parse(this.contentNameWithMetaData), Times.Once);
            this.mockFontPathResolver.Verify(m => m.ResolveFilePath(FontContentName), Times.Once);
            this.mockPath.Verify(m => m.GetFileNameWithoutExtension(this.fontFilePath), Times.Once);
            this.mockFontAtlasService.Verify(m => m.CreateFontAtlas(this.fontFilePath, FontSize), Times.Once);
            this.mockTextureCache.Verify(m => m.GetItem(this.filePathWithMetaData), Times.Once);

            this.mockFontFactory.Verify(m =>
                    m.Create(
                        this.mockFontAtlasTexture.Object,
                        FontContentName,
                        this.fontFilePath,
                        FontSize,
                        It.IsAny<bool>(),
                        this.glyphMetricData),
                Times.Once);

            Assert.Same(this.mockFont.Object, actual);
        }

        [Fact]
        public void Unload_WithInvalidMetaData_ThrowsException()
        {
            // Arrange
            const string contentName = "invalid-metadata";
            const string invalidMetaData = "size-12";

            var expected = $"The metadata '{invalidMetaData}' is invalid when unloading '{contentName}'.";
            expected += $"{Environment.NewLine}\tExpected MetaData Syntax: size:<font-size>";
            expected += $"{Environment.NewLine}\tExample: size:12";

            this.mockFontMetaDataParser.Setup(m => m.Parse(contentName))
                .Returns(new FontMetaDataParseResult(
                    true,
                    false,
                    string.Empty,
                    invalidMetaData,
                    FontSize));
            var loader = CreateLoader();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<CachingMetaDataException>(() =>
            {
                loader.Unload(contentName);
            }, expected);
        }

        [Fact]
        public void Unload_WithNoMetaData_ThrowsException()
        {
            // Arrange
            const string contentName = "missing-metadata";

            var expected = "When unloading fonts, the name of or the full file path of the font";
            expected += " must be supplied with valid metadata syntax.";
            expected += $"{Environment.NewLine}\tExpected MetaData Syntax: size:<font-size>";
            expected += $"{Environment.NewLine}\tExample: size:12";

            this.mockFontMetaDataParser.Setup(m => m.Parse(contentName))
                .Returns(new FontMetaDataParseResult(
                    false,
                    false,
                    string.Empty,
                    string.Empty,
                    FontSize));
            var loader = CreateLoader();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<CachingMetaDataException>(() =>
            {
                loader.Unload(contentName);
            }, expected);
        }

        [Fact]
        public void Unload_WhenUnloadingWithFullFilePathAndMetaData_UnloadsFonts()
        {
            // Arrange
            var loader = CreateLoader();

            // Act
            loader.Unload(this.filePathWithMetaData);

            // Assert
            this.mockFontMetaDataParser.Verify(m => m.Parse(this.filePathWithMetaData), Times.Once);
            this.mockTextureCache.Verify(m => m.Unload(this.filePathWithMetaData), Times.Once);
        }

        [Fact]
        public void Unload_WhenUnloadingWithContentNameAndMetaData_UnloadsFonts()
        {
            // Arrange
            var loader = CreateLoader();

            // Act
            loader.Unload(this.contentNameWithMetaData);

            // Assert
            this.mockFontMetaDataParser.Verify(m => m.Parse(this.contentNameWithMetaData), Times.Once);
            this.mockFontPathResolver.Verify(m => m.ResolveFilePath(FontContentName), Times.Once);
            this.mockTextureCache.Verify(m => m.Unload(this.filePathWithMetaData), Times.Once);
        }
        #endregion

        /// <summary>
        /// Generates fake glyph metric data for testing.
        /// </summary>
        /// <param name="start">The start value of all of the metric data.</param>
        /// <returns>The glyph metric data to be tested.</returns>
        /// <remarks>
        ///     The start value is a metric value start and incremented for each metric.
        /// </remarks>
        private static GlyphMetrics GenerateMetricData(int start)
        {
            return new GlyphMetrics()
            {
                Ascender = start,
                Descender = start + 1,
                CharIndex = (uint)start + 2,
                GlyphWidth = start + 3,
                GlyphHeight = start + 4,
                HoriBearingX = start + 5,
                HoriBearingY = start + 6,
                XMin = start + 7,
                XMax = start + 8,
                YMin = start + 9,
                YMax = start + 10,
                HorizontalAdvance = start + 11,
                Glyph = (char)(start + 12),
                GlyphBounds = new RectangleF(start + 13, start + 14, start + 15, start + 16),
            };
        }

        /// <summary>
        /// Creates an instance of <see cref="AtlasLoader"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private FontLoader CreateLoader() => new (
            this.mockFontAtlasService.Object,
            this.mockEmbeddedFontResourceService.Object,
            this.mockFontPathResolver.Object,
            this.mockTextureCache.Object,
            this.mockFontFactory.Object,
            this.mockFontMetaDataParser.Object,
            this.mockDirectory.Object,
            this.mockFile.Object,
            this.mockFileStream.Object,
            this.mockPath.Object);

        /// <summary>
        /// Mocks the loading of an embedded font resource file using the given name for the purpose of testing.
        /// </summary>
        /// <param name="name">The name of the resource to mock.</param>
        /// <returns>The mock object to verify against.</returns>
        private Mock<Stream> MockLoadResource(string name)
        {
            var result = new Mock<Stream>();
            this.mockEmbeddedFontResourceService.Setup(m => m.LoadResource(name))
                .Returns(result.Object);

            return result;
        }

        /// <summary>
        /// Mocks the creation of a file stream for the given <paramref name="filePath"/>
        /// for the purpose of testing.
        /// </summary>
        /// <param name="filePath">The file path to mock.</param>
        /// <returns>The mock object to verify against.</returns>
        private Mock<Stream> MockCopyToStream(string filePath)
        {
            var result = new Mock<Stream>();
            this.mockFileStream.Setup(m => m.Create(filePath, FileMode.Create, FileAccess.Write))
                .Returns(result.Object);

            return result;
        }
    }
}
