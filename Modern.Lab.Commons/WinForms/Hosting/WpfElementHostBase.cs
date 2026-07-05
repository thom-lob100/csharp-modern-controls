using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Modern.Lab.WinForms.Controls.Hosting
{
    /// <summary>
    /// 모든 WinForms 래퍼 컨트롤의 공통 베이스 클래스.
    ///
    /// WinForms 폼은 WPF 컨트롤을 직접 호스팅할 수 없고 반드시 ElementHost
    /// 브리지를 거쳐야 한다. 이 클래스는 ElementHost에서 파생되어 내부 WPF
    /// 컨트롤을 자동으로 생성하므로, 구체 래퍼는 내부 컨트롤의 속성을 디자이너
    /// 속성 그리드에 다시 노출하기만 하면 된다.
    ///
    /// 디자인 타임 정책 (docs/design-notes.md 2, 4, 5절):
    /// - 디자인 표면에서는 WPF 컨트롤을 절대 호스팅하지 않는다. 동작이
    ///   결정적이도록 생성자와 OnHandleCreated 양쪽에 동일한 가드를 적용한다.
    /// - 디자인 타임에 WPF 컨트롤 생성 중 실패(오래된 어셈블리, pack URI 해석
    ///   실패 등)는 삼킨다: 깨진 컨트롤 하나가 호스트 폼의 디자이너를 죽여서는
    ///   안 된다.
    /// - 대신 OnPaint가 RenderTargetBitmap 스냅숏을 미리보기로 그리고, 렌더링
    ///   실패 시 placeholder(타입 이름 + 테두리)로 폴백한다.
    /// </summary>
    /// <typeparam name="TWpf">감싸는 WPF 컨트롤의 타입 (예: ModernButtonControl)</typeparam>
    [ToolboxItem(false)]                 // 베이스 자체는 도구 상자에 표시하지 않는다
    [DesignerCategory("Code")]           // 빈 컴포넌트 디자이너가 열리는 것을 방지한다
    // 기본 ElementHost 디자이너(ElementHostDesigner)는 디자인 표면에서 호스팅된
    // Child를 다시 생성하고 직렬화하는데, 이는 생성자에서 만든 Wpf 인스턴스와
    // 충돌하여 폼을 망가뜨린다. 일반 ControlDesigner를 쓰면 래퍼가 불투명한
    // 컨트롤로 취급된다: 디자이너가 Child를 건드리지 않으므로 드래그/재직렬화가
    // 안전하게 유지된다.
    [Designer("System.Windows.Forms.Design.ControlDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    public abstract class WpfElementHostBase<TWpf> : ElementHost
        where TWpf : System.Windows.FrameworkElement, new()
    {
        // LicenseManager.UsageMode는 디자이너가 생성자를 호출하는 동안에만
        // 신뢰할 수 있으므로 한 번 읽어 저장해 둔다. (this.DesignMode는 생성자
        // 안에서 항상 false다.)
        private readonly bool isDesignTime;

        // 디자인 타임 스냅숏 미리보기 캐시. 크기나 래퍼 속성이 바뀔 때만
        // 다시 렌더링된다.
        private Bitmap previewCache;

        /// <summary>
        /// 내부에 호스팅되는 실제 WPF 컨트롤. 래퍼 서브클래스가 이 컨트롤의
        /// 속성을 읽고 쓴다. 디자인 타임 생성이 실패한 경우에만 null일 수
        /// 있으므로, 래퍼의 속성 접근자는 null 안전해야 한다.
        /// </summary>
        protected TWpf Wpf { get; private set; }

        /// <summary>
        /// WPF 컨트롤을 생성하고, 런타임에만 자식으로 호스팅한다.
        /// </summary>
        protected WpfElementHostBase()
        {
            this.isDesignTime = LicenseManager.UsageMode == LicenseUsageMode.Designtime;

            if (this.isDesignTime)
            {
                // WPF 생성자와 XAML 파싱은 디자이너 프로세스 안에서도 실행된다.
                // 호스트 폼의 디자이너가 살아남도록 여기서 실패를 모두 삼킨다;
                // OnPaint가 placeholder로 폴백한다.
                try
                {
                    this.Wpf = new TWpf();
                }
                catch (Exception)
                {
                    this.Wpf = null;
                }
            }
            else
            {
                // 런타임 생성 실패는 정상적으로 전파되어야 한다.
                this.Wpf = new TWpf();
                this.Child = this.Wpf;
            }
        }

        /// <summary>
        /// 런타임 핸들이 생성될 때 호스팅을 완료한다. 디자인 표면의 컨트롤도
        /// 실제 Win32 핸들을 가지므로 여기에도 가드가 필요하다 — 생성자만
        /// 가드하면 디자이너 표시 여부가 비결정적이 된다(핸들 타이밍에 따라
        /// 붙거나 안 붙거나 함).
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            if (!this.isDesignTime && !this.DesignMode && this.Child == null && this.Wpf != null)
            {
                this.Child = this.Wpf;
            }

            base.OnHandleCreated(e);
        }

        /// <summary>
        /// 디자인 타임에는 WPF 스냅숏(또는 placeholder)을 미리보기로 그린다.
        /// 런타임에는 호스팅된 Child가 스스로 그리므로 아무것도 하지 않는다.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.isDesignTime || this.DesignMode)
            {
                this.PaintDesignTimePreview(e.Graphics);
            }
        }

        /// <summary>
        /// 크기가 바뀔 때 스냅숏 캐시를 버려 새 크기로 다시 렌더링되게 한다.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.InvalidateDesignTimePreview();
        }

        /// <summary>
        /// 디자인 타임 미리보기 캐시를 무효화한다. 속성 변경이 미리보기에
        /// 반영되도록 래퍼는 속성 setter에서 이 메서드를 호출해야 한다.
        /// 런타임에는 아무것도 하지 않는다.
        /// </summary>
        protected void InvalidateDesignTimePreview()
        {
            if (this.isDesignTime || this.DesignMode)
            {
                this.DisposePreviewCache();
                this.Invalidate();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.DisposePreviewCache();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// ElementHost 자체의 Child 속성은 디자이너에서 건드릴 일이 없으므로
        /// 속성 그리드와 직렬화에서 숨긴다.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new System.Windows.UIElement Child
        {
            get { return base.Child; }
            set { base.Child = value; }
        }

        // ===== 무효 상속 속성 숨김 =====
        // 아래 WinForms 상속 속성들은 WPF 콘텐츠에 아무 효과가 없다(모든 색·폰트는
        // Themes/Tokens.xaml 토큰이 소스). 속성 그리드에 보이면 "바꿨는데 안 변하는"
        // 혼동을 주므로 그리드와 직렬화에서 숨긴다. 기능 자체는 base로 그대로
        // 위임하므로 기존 코드가 설정해도 깨지지 않는다.

        /// <summary>WPF 콘텐츠에 효과 없음 — 디자인 토큰이 색상의 소스.</summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override System.Drawing.Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        /// <summary>WPF 콘텐츠에 효과 없음 — 디자인 토큰이 색상의 소스.</summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override System.Drawing.Color ForeColor
        {
            get { return base.ForeColor; }
            set { base.ForeColor = value; }
        }

        /// <summary>WPF 콘텐츠에 효과 없음 — 디자인 토큰이 타이포그래피의 소스.</summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Font Font
        {
            get { return base.Font; }
            set { base.Font = value; }
        }

        /// <summary>WPF 콘텐츠에 효과 없음.</summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image BackgroundImage
        {
            get { return base.BackgroundImage; }
            set { base.BackgroundImage = value; }
        }

        /// <summary>WPF 콘텐츠에 효과 없음.</summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ImageLayout BackgroundImageLayout
        {
            get { return base.BackgroundImageLayout; }
            set { base.BackgroundImageLayout = value; }
        }

        private void PaintDesignTimePreview(Graphics graphics)
        {
            if (this.Width <= 0 || this.Height <= 0)
            {
                return;
            }

            if (this.previewCache == null ||
                this.previewCache.Width != this.Width ||
                this.previewCache.Height != this.Height)
            {
                this.DisposePreviewCache();
                this.previewCache = this.RenderPreviewBitmap();
            }

            if (this.previewCache != null)
            {
                graphics.DrawImage(this.previewCache, 0, 0);
            }
            else
            {
                this.PaintPlaceholder(graphics);
            }
        }

        // WPF 컨트롤을 화면 밖에서 측정/배치한 뒤 RenderTargetBitmap을 거쳐
        // GDI+ 비트맵으로 렌더링한다. 디자이너에게는 그냥 그림일 뿐이므로
        // 직렬화나 마우스 캡처 문제가 없다.
        // 실패 시 null을 반환한다(placeholder 폴백).
        private Bitmap RenderPreviewBitmap()
        {
            if (this.Wpf == null)
            {
                return null;
            }

            try
            {
                System.Windows.Size size = new System.Windows.Size(this.Width, this.Height);
                this.Wpf.Measure(size);
                this.Wpf.Arrange(new System.Windows.Rect(size));
                this.Wpf.UpdateLayout();

                // 화면 밖 인스턴스는 렌더 패스가 돌지 않으므로, 대기 중인
                // 바인딩/DataTrigger/렌더 디스패처 작업을 여기서 소화시킨다.
                // 이게 없으면 속성 변경(Kind 등)이 다음 페인트에야 스냅숏에 반영된다.
                this.Wpf.Dispatcher.Invoke(
                    System.Windows.Threading.DispatcherPriority.Render,
                    new Action(delegate { }));

                RenderTargetBitmap target = new RenderTargetBitmap(
                    this.Width, this.Height, 96d, 96d, PixelFormats.Pbgra32);
                target.Render(this.Wpf);

                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(target));

                using (MemoryStream stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    stream.Position = 0;

                    // Bitmap(stream)은 스트림 수명에 묶여 있으므로,
                    // 분리된 복사본을 대신 돌려준다.
                    using (Bitmap streamBound = new Bitmap(stream))
                    {
                        return new Bitmap(streamBound);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        // 스냅숏을 렌더링할 수 없을 때의 placeholder: 옅은 테두리 + 타입 이름.
        private void PaintPlaceholder(Graphics graphics)
        {
            Rectangle bounds = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

            using (System.Drawing.Pen borderPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(209, 213, 219)))
            {
                graphics.DrawRectangle(borderPen, bounds);
            }

            using (SolidBrush textBrush = new SolidBrush(System.Drawing.Color.FromArgb(107, 114, 128)))
            {
                graphics.DrawString(this.GetType().Name, this.Font, textBrush, 4f, 4f);
            }
        }

        private void DisposePreviewCache()
        {
            if (this.previewCache != null)
            {
                this.previewCache.Dispose();
                this.previewCache = null;
            }
        }
    }
}
