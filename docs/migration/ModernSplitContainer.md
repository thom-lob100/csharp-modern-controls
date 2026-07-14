# ModernSplitContainer 도입 가이드

- **대체 대상**: `System.Windows.Forms.SplitContainer` — 좌/우(상/하) 영역 크기 조절
- **네임스페이스**: `Modern.Lab.WinForms.Controls.Layout`

## 특징 — 순수 WinForms 컨테이너

`ModernCardPanel`과 마찬가지로 **ElementHost가 아니라 GDI+로 그리는 일반 WinForms
`SplitContainer` 파생**이다. 따라서 계약 룰 5(영역 레이아웃은 WinForms 담당)를 지키면서
양쪽 패널에 어떤 WinForms 자식(모던 카드/그리드 포함)도 담을 수 있다.

시각/동작 보정:

- 거터(스플리터 띠)는 **부모 배경색**으로 칠해 색 띠가 아니라 "간격"으로 보인다 —
  라이트/다크/틴트 어떤 테마에서도 자연스럽다.
- 거터 중앙에 짧은 **그립 필**(pill)을 그린다: 평상시 `BorderSubtle`,
  마우스 오버·드래그 중 `Accent`.
- 클릭/드래그 후 남던 점선 포커스 사각형을 제거 (`TabStop = false` + 포커스 반환).
- 기본값: `BorderStyle = None`, `SplitterWidth = 12`.

## 제공 멤버

`SplitContainer`의 모든 멤버 그대로 (`Orientation`, `SplitterDistance`,
`Panel1MinSize`/`Panel2MinSize`, `FixedPanel`, `IsSplitterFixed`, `SplitterMoved`, ...).

추가 멤버:

| 멤버 | 기본값 | 설명 |
|---|---|---|
| `DeferredDrag` | `true` | 드래그 중 **가는 액센트 가이드 라인만** 움직이고 마우스를 놓을 때 한 번만 레이아웃 적용. WPF 섬(ElementHost)이 많은 화면은 리사이즈 스텝당 수백 ms가 들어(실측: Item History 폼 ~320ms/스텝 vs 빈 폼 ~4ms) 라이브 드래그가 3fps 수준으로 버벅이므로 이쪽이 기본값. 가벼운 화면에서 실시간 리플로우를 원하면 `false` |

## .Designer.cs 교체 예시

```csharp
// 선언부
private Modern.Lab.WinForms.Controls.Layout.ModernSplitContainer splitMain;

// InitializeComponent 안
this.splitMain = new Modern.Lab.WinForms.Controls.Layout.ModernSplitContainer();
((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();   // 필수!
this.splitMain.SuspendLayout();
// ...
this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
this.splitMain.Orientation = System.Windows.Forms.Orientation.Vertical;    // 좌/우 분할
this.splitMain.Panel1.Controls.Add(this.leftZone);
this.splitMain.Panel1MinSize = 240;
this.splitMain.Panel2.Controls.Add(this.rightZone);
this.splitMain.Panel2MinSize = 480;
this.splitMain.Size = new System.Drawing.Size(1516, 676);
this.splitMain.SplitterDistance = 340;
// ...
((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();     // 필수!
this.splitMain.ResumeLayout(false);
```

## 주의

| 항목 | 내용 |
|---|---|
| `BeginInit`/`EndInit` | **코드로 직접 배치할 때도 반드시 감싼다** — SplitContainer는 속성 설정 순서를 엄격히 검증하므로, 초기 크기(150px)보다 큰 `MinSize`/`SplitterDistance`를 Init 블록 없이 설정하면 생성 시점에 예외가 난다 (VS 디자이너는 자동으로 넣어 줌) |
| 거터 색 | 부모 `BackColor`를 따른다 — 부모 배경이 단색일 때 자연스러움 (카드 위에 직접 두지 말 것) |
| `Orientation` | `Vertical` = 좌/우 분할(거터가 세로 띠), `Horizontal` = 상/하 분할 — WinForms 기본 의미 그대로 |
