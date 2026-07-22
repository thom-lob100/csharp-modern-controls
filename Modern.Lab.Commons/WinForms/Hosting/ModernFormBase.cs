using System;
using System.Windows.Forms;

namespace Modern.Lab.WinForms.Controls.Hosting
{
    /// <summary>
    /// 화면 폼 공통 베이스 — 모든 화면 폼/다이얼로그가 <c>Form</c> 대신
    /// 상속한다.
    ///
    /// 공통으로 제공하는 것:
    /// - <see cref="InitializeModernForm(bool)"/>: 파생 폼 생성자에서
    ///   <c>InitializeComponent()</c> **직후** 1회 호출 — 로딩 커버(화면 폼)와
    ///   메시징 초기화를 한 곳에서 처리한다.
    /// - <see cref="PostToUi"/>: 백그라운드 작업의 UI 반영 공통 경로 —
    ///   폼이 닫히는 중이면 Invoke를 시도하지 않고, 직후 닫히는 경쟁 상태도
    ///   예외 없이 무시한다.
    /// - Dispose 연동: 파생 Designer의 <c>Dispose(bool)</c>이 base.Dispose를
    ///   부르는 표준 경로에서 메시징 정리(<see cref="DisposeMessaging"/>)가
    ///   함께 일어난다 — 폼마다 정리 코드를 반복하지 않는다.
    ///
    /// ★ 회사 환경 메시징(TibcoLive) — 이 파일 안의 주석 처리된 코드를
    ///   해제하면 모든 화면이 물려받는다. 이 파일은 라이브러리에서 유일하게
    ///   통신 초기화가 들어가는 예외 지점이다(컨트롤들은 데이터 무관 원칙
    ///   유지). 화면 코드는 this.Tibrv.SendRequest(...)로 전문을 보낸다.
    ///   (홈 데모는 REST(WebClient)를 각 폼의 "서버 호출" 구획이 직접 쓰므로
    ///   기본은 아무것도 하지 않는다.)
    /// </summary>
    public class ModernFormBase : Form
    {
        // ★ 회사 환경 — TibcoLive 메시징 필드. 주석을 해제해 사용한다.
        //   파생 폼이 this.Tibrv.SendRequest(...)로 전문을 보낸다.
        // protected TibcoLive Tibrv;

        /// <summary>
        /// 파생 폼 생성자에서 <c>InitializeComponent()</c> 직후 1회 호출한다.
        /// </summary>
        /// <param name="useLoadCover">화면 폼은 true(오픈 깜빡임을 로딩 커버로
        /// 가림), 다이얼로그는 false(표시 전에 레이아웃이 끝나 커버 불필요).</param>
        protected void InitializeModernForm(bool useLoadCover = true)
        {
            if (useLoadCover)
            {
                ModernLoadCover.Attach(this);
            }

            this.InitializeMessaging();
        }

        /// <summary>메시징(전문 통신) 초기화 지점 — InitializeModernForm이
        /// InitializeComponent() 직후 호출한다 (회사 관례 순서: 선언 →
        /// InitializeComponent → SetItem).</summary>
        protected virtual void InitializeMessaging()
        {
            // ★ 회사 환경 — 주석 해제:
            // this.Tibrv = new TibcoLive();
            // this.Tibrv.SetItem("MODERN");
        }

        /// <summary>메시징 정리 지점 — 폼 Dispose(disposing = true) 때 자동
        /// 호출된다 (파생 폼마다 정리 코드를 두지 않아도 된다).</summary>
        protected virtual void DisposeMessaging()
        {
            // ★ 회사 환경 — 주석 해제 (정리 메서드 이름은 회사 라이브러리
            //   규약을 따른다 — Close/Dispose 등):
            // if (this.Tibrv != null)
            // {
            //     this.Tibrv.Dispose();
            //     this.Tibrv = null;
            // }
        }

        /// <summary>백그라운드 작업의 UI 반영 공통 경로 — 닫히는 중이면 반영을
        /// 버리고, Dispose와 동시에 끝난 요청의 경쟁 예외도 무시한다.</summary>
        protected void PostToUi(MethodInvoker action)
        {
            if (action == null || this.IsDisposed || this.Disposing || !this.IsHandleCreated)
            {
                return;
            }

            try
            {
                this.BeginInvoke(action);
            }
            catch (ObjectDisposedException)
            {
                // Dispose와 동시에 끝난 백그라운드 요청은 버린다.
            }
            catch (InvalidOperationException)
            {
                // 핸들이 해제된 직후의 반영 요청은 버린다.
            }
        }

        /// <summary>파생 Designer의 Dispose(bool) → base.Dispose 표준 경로에서
        /// 메시징을 함께 정리한다.</summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.DisposeMessaging();
            }

            base.Dispose(disposing);
        }
    }
}
