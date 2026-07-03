# ModernLabel 교체 가이드

- **대체 대상**: `System.Windows.Forms.Label`
- **네임스페이스**: `Modern.Lab.WinForms.Controls.Display`

## 호환 제공 멤버

| 멤버 | 비고 |
|---|---|
| `Text` | `Control.Text` override, `[Localizable(true)]` |
| `Enabled` | 전파됨 |

## 추가 멤버

| 멤버 | 설명 |
|---|---|
| `Kind` | `Body`(본문) / `Title`(섹션 제목) / `Label`(필드 라벨) / `Helper`(보조 설명) — 토큰 타이포 램프 적용 |

## 미지원 멤버와 대체 방법

| 기존 멤버 | 대체 |
|---|---|
| `AutoSize` | 없음 — 명시적 `Size` 지정 필요. 넘치는 텍스트는 말줄임(…) 처리 |
| `Font`, `ForeColor` | 없음 — 타이포는 `Kind`가 결정 (토큰 단일 소스 원칙) |
| `TextAlign` | 세로 중앙 고정. 가로 정렬이 필요하면 배치로 해결 |

## .Designer.cs 교체 예시

```csharp
// 변경 전
private System.Windows.Forms.Label lblName;
this.lblName = new System.Windows.Forms.Label();

// 변경 후
private Modern.Lab.WinForms.Controls.Display.ModernLabel lblName;
this.lblName = new Modern.Lab.WinForms.Controls.Display.ModernLabel();
this.lblName.Kind = Modern.Lab.Controls.Wpf.Display.LabelKind.Label;  // 필드 라벨이면
```

권장 크기: 폭은 내용에 맞게, 높이 24 (Title은 32).
