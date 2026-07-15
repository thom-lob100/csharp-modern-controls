using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Modern.Lab.WinForms.Controls.Hosting
{
    /// <summary>
    /// 폼 단위 커서 방어 헬퍼. 컨트롤 트리를 내려가며 모든 ElementHost의
    /// Cursor 속성 매핑을 제거한다 — <see cref="WpfHostOptions.DisableCursorPropertyMap"/>
    /// 전역 플래그의 폼 단위 버전으로, 특정 화면에만 선별 적용하거나 이미
    /// Wait가 박힌 화면의 응급 처치로 쓴다 (제거 이후 첫 마우스 이동부터
    /// WPF가 커서를 다시 관리해 정상으로 돌아온다).
    ///
    /// <c>ModernThemeWinForms.Apply(this)</c>처럼 폼 생성자에서
    /// <c>InitializeComponent()</c> 직후 호출한다. Apply 호출 이후에 동적으로
    /// 추가된 컨트롤은 커버하지 못하므로, 전체 적용이 목적이면 전역 플래그를
    /// 권장한다.
    /// </summary>
    public static class WpfHostCursorGuard
    {
        /// <summary>root 이하 모든 ElementHost의 Cursor 매핑을 제거한다.</summary>
        public static void Apply(Control root)
        {
            if (root == null)
            {
                return;
            }

            ElementHost host = root as ElementHost;
            if (host != null)
            {
                RemoveCursorMapping(host);
            }

            foreach (Control child in root.Controls)
            {
                Apply(child);
            }
        }

        /// <summary>ElementHost 하나의 Cursor 매핑을 제거한다 (없으면 무시).</summary>
        public static void RemoveCursorMapping(ElementHost host)
        {
            if (host != null && host.PropertyMap != null)
            {
                host.PropertyMap.Remove("Cursor");
            }
        }
    }
}
