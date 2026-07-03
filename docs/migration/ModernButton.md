# ModernButton 교체 가이드

- **대체 대상**: `System.Windows.Forms.Button`
- **네임스페이스**: `Modern.Lab.WinForms.Controls.Input`

## 호환 제공 멤버

| 멤버 | 비고 |
|---|---|
| `Text` | `Control.Text` override, `[Localizable(true)]` — Localizable 폼에서도 정상 직렬화 |
| `Click` | 표준 WinForms 이벤트. 기존 핸들러 연결 그대로 동작 |
| `Enabled` | ElementHost가 내부 WPF에 전파. 비활성 시 회색 표현 |

## 추가 멤버

| 멤버 | 설명 |
|---|---|
| `Kind` | `Primary`(파랑) / `Secondary`(흰 배경) / `Danger`(빨강) |
| `IconGlyph` | Segoe MDL2 Assets 글리프. 비우면 아이콘 없음 |

## 미지원 멤버와 대체 방법

| 기존 멤버 | 대체 |
|---|---|
| `FlatStyle`, `BackColor`, `ForeColor`, `Image` | 없음 — 색·모양은 디자인 토큰이 결정. 강조 수준은 `Kind`로 |
| `DialogResult` | 필요 시 `Click` 핸들러에서 `this.DialogResult = …` 직접 설정 |

## .Designer.cs 교체 예시

```csharp
// 변경 전
private System.Windows.Forms.Button btnSearch;
this.btnSearch = new System.Windows.Forms.Button();

// 변경 후 (타입만 변경; Text/Click/Enabled 코드는 그대로)
private Modern.Lab.WinForms.Controls.Input.ModernButton btnSearch;
this.btnSearch = new Modern.Lab.WinForms.Controls.Input.ModernButton();
```

권장 크기: 120×32 (토큰 ControlHeight=32에 맞춤).
