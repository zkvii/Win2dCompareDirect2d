using System;
using System.Numerics;
using ABI.Windows.System.RemoteSystems;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using SharpGen.Runtime;
using Vortice.Direct2D1;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DirectWrite;
using Vortice.DXGI;
using Vortice.Mathematics;
using FactoryType = Vortice.Direct2D1.FactoryType;
using FeatureLevel = Vortice.Direct3D.FeatureLevel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Win2dCompareDirect2d
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow 
    {

        // private ID3D11DeviceContext deviceContext;
        public static IDXGIDevice DxgiDevice;
        public static IDXGISwapChain1 SwapChain;
        public static ID3D11Texture2D BackBuffer;
        // public static ID3D11RenderTargetView RenderTargetView;

        public static IDXGISurface DxgiBackBuffer;
        public static ID2D1Factory1 D2dFactory;
        public static ID2D1Device D2dDevice;
        public static ID2D1DeviceContext D2dContext;
        public static ID2D1Bitmap1 D2dTargetBitmap1;

        public static Vortice.WinUI.ISwapChainPanelNative SwapChainPanel;
        public static ID3D11Device D3Ddevice;
        public static IDWriteFactory D2DWriteFactory;
        private DispatcherTimer _timer;

        private IntPtr hwnd;

        public MainWindow()
        {
            this.InitializeComponent();
            hwnd=WinRT.Interop.WindowNative.GetWindowHandle(this);

            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = TimeSpan.FromMilliseconds(10D);
            InitDirectX();
            CreateSwapChain(D2DPanel);
            SizeChanged += MainWindow_SizeChanged;
        }

        private void Timer_Tick(object sender, object e)
        {
            DrawD2DContent();
        }


        private void DrawContent(CanvasControl sender, CanvasDrawEventArgs args)
        {
            using var ds = args.DrawingSession;

            var canvasTarget= new CanvasRenderTarget(sender, 500, 500);
            using var dds = canvasTarget.CreateDrawingSession();
            dds.Clear(Colors.Transparent);
            dds.FillRectangle(100, 100, 200, 200, Microsoft.UI.Colors.Black);
            var textFormat= new Microsoft.Graphics.Canvas.Text.CanvasTextFormat()
            {
                FontSize = 25,
                FontFamily = "Arial"
            };

            var textLayout= new Microsoft.Graphics.Canvas.Text.CanvasTextLayout(ds, "Hello World", textFormat, 100, 100);

            dds.DrawTextLayout(textLayout,new Vector2(100,100), Microsoft.UI.Colors.White);
            ds.DrawImage(canvasTarget);
        }

        private void DrawD2DContent()
        {
            D2dContext.BeginDraw();
            D2dContext.Clear(Colors.Transparent);

            //sample drawing
            D2dContext.AntialiasMode = AntialiasMode.Aliased;
            var blackBrush = D2dContext.CreateSolidColorBrush(Colors.Black);
            var textbrush=D2dContext.CreateSolidColorBrush(Colors.White);
            // D2dContext.FillRectangle(new Rect(300f, 100f, 200f, 100f), blackBrush);
            D2dContext.AntialiasMode = AntialiasMode.PerPrimitive;
            D2dContext.FillRectangle(new Rect(500f, 100f, 200f, 200f), blackBrush);

            var textFormat = D2DWriteFactory.CreateTextFormat("Arial", 25f);

            var textLayout = D2DWriteFactory.CreateTextLayout("Hello World", textFormat, 100f, 100f);

            D2dContext.DrawTextLayout(new Vector2(200, 200), textLayout, textbrush);

            D2dContext.EndDraw();
            SwapChain.Present(2, PresentFlags.None);
        }

        private void MainWindow_SizeChanged(object sender, Microsoft.UI.Xaml.WindowSizeChangedEventArgs args)
        {

            ResizeSwapChain((int)D2DPanel.ActualSize.X, (int)D2DPanel.ActualSize.Y);
        }

    

        public  void InitDirectX()
        {
            FeatureLevel[] featureLevels =
            [
                FeatureLevel.Level_12_1,
                FeatureLevel.Level_12_0,
                FeatureLevel.Level_11_1,
                FeatureLevel.Level_11_0,
                FeatureLevel.Level_10_1,
                FeatureLevel.Level_10_0,
                FeatureLevel.Level_9_3,
                FeatureLevel.Level_9_2,
                FeatureLevel.Level_9_1
            ];

            D3D11.D3D11CreateDevice(
                null,
                DriverType.Hardware,
                DeviceCreationFlags.BgraSupport | DeviceCreationFlags.Debug,
                featureLevels,
                out var tempDevice,
                // out D3Ddevice,
                out ID3D11DeviceContext _).CheckError();
            D3Ddevice = tempDevice;
            // deviceContext = tempContext;
            DxgiDevice = D3Ddevice.QueryInterface<IDXGIDevice>();

        }

        public  void ResizeSwapChain(int width, int height)
        {
            D2dContext.Target = null;
            // renderTargetView.Dispose();
            BackBuffer.Dispose();
            DxgiBackBuffer.Dispose();
            D2dTargetBitmap1.Dispose();

            SwapChain.ResizeBuffers(2, width, height, Format.B8G8R8A8_UNorm,
                SwapChainFlags.None);
            BackBuffer = SwapChain.GetBuffer<ID3D11Texture2D>(0);
            // renderTargetView = device.CreateRenderTargetView(backBuffer);
            DxgiBackBuffer = BackBuffer.QueryInterface<IDXGISurface>();
            var bitmapProperties = new BitmapProperties1();
            bitmapProperties.PixelFormat.Format = Format.B8G8R8A8_UNorm;
            bitmapProperties.PixelFormat.AlphaMode = Vortice.DCommon.AlphaMode.Premultiplied;
            bitmapProperties.BitmapOptions = BitmapOptions.Target | BitmapOptions.CannotDraw;
            uint nDPI = Win32Helpers.GetDpiForWindow(hwnd);
            bitmapProperties.DpiX = nDPI;
            bitmapProperties.DpiY = nDPI;
            // bitmapProperties.DpiX = 96;
            // bitmapProperties.DpiY = 96;
            D2dTargetBitmap1 = D2dContext.CreateBitmapFromDxgiSurface(DxgiBackBuffer, bitmapProperties);
            D2dContext.Target = D2dTargetBitmap1;
        }

        public  void CreateSwapChain(SwapChainPanel swapChainCanvas)
        {
            ComObject comObject = new ComObject(swapChainCanvas);
            SwapChainPanel = comObject.QueryInterfaceOrNull<Vortice.WinUI.ISwapChainPanelNative>();
            comObject.Dispose();

            SwapChainDescription1 swapChainDesc = new SwapChainDescription1()
            {
                Stereo = false,
                Width = (int)swapChainCanvas.Width,
                Height = (int)swapChainCanvas.Height,
                BufferCount = 2,
                BufferUsage = Usage.RenderTargetOutput,
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                Scaling = Scaling.Stretch,
                AlphaMode = AlphaMode.Premultiplied,
                Flags = SwapChainFlags.None,
                SwapEffect = SwapEffect.FlipSequential
            };

            IDXGIAdapter1 dxgiAdapter = DxgiDevice.GetParent<IDXGIAdapter1>();
            IDXGIFactory2 dxgiFactory2 = dxgiAdapter.GetParent<IDXGIFactory2>();

            // resize window flick bug
            SwapChain = dxgiFactory2.CreateSwapChainForComposition(D3Ddevice, swapChainDesc);

            BackBuffer = SwapChain.GetBuffer<ID3D11Texture2D>(0);
            // renderTargetView = device.CreateRenderTargetView(backBuffer);
            DxgiBackBuffer = BackBuffer.QueryInterface<IDXGISurface>();
            if (SwapChainPanel != null) SwapChainPanel.SetSwapChain(SwapChain);

            D2dFactory = D2D1.D2D1CreateFactory<ID2D1Factory1>(FactoryType.MultiThreaded);
            D2dDevice = D2dFactory.CreateDevice(DxgiDevice);
            D2dContext = D2dDevice.CreateDeviceContext(DeviceContextOptions.EnableMultithreadedOptimizations);
            D2DWriteFactory = DWrite.DWriteCreateFactory<IDWriteFactory>();

            var bitmapProperties = new BitmapProperties1();
            bitmapProperties.PixelFormat.Format = Format.B8G8R8A8_UNorm;
            bitmapProperties.PixelFormat.AlphaMode = Vortice.DCommon.AlphaMode.Premultiplied;
            bitmapProperties.BitmapOptions = BitmapOptions.Target | BitmapOptions.CannotDraw;
            uint nDPI = Win32Helpers.GetDpiForWindow(hwnd);
            bitmapProperties.DpiX = nDPI;
            bitmapProperties.DpiY = nDPI;
            // bitmapProperties.DpiX = 144;
            // bitmapProperties.DpiY = 144;
            D2dTargetBitmap1 = D2dContext.CreateBitmapFromDxgiSurface(DxgiBackBuffer, bitmapProperties);
            D2dContext.Target = D2dTargetBitmap1;

            DxgiDevice.Dispose();

            _timer.Start();
            // d2dFactory.DesktopDpi.X, d2dFactory.DesktopDpi.Y,
            // BitmapOptions.Target | BitmapOptions.CannotDraw);
        }
    }
}
