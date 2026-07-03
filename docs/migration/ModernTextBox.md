# ModernTextBox 교체 가이드

- **대체 대상**: `System.Windows.Forms.TextBox` (단일행)
- **네임스페이스**: `Modern.Lab.WinForms.Controls.Input`

## 호환 제공 멤버

| 멤버 | 비고 |
|---|---|
| `Text` | `Control.Text` override, `[Localizable(true)]` |
| `TextChanged` | 표준 WinForms 이벤트 — 내부 WPF 텍스트 변경 시 발생. 기존 핸들러 그대로 동작 |
| `ReadOnly` | `TextBox.ReadOnly`와 동일 의미. 배경이 옅게 바뀜 |
| `Enabled` | 전파됨 |

## 추가 멤버

| 멤버 | 설명 |
|---|---|
| `PlaceholderText` | 입력이 비었을 때 표시할 힌트 |
| `EnterPressed` | Enter 키 입력 시 발생. **주의: 기존 `KeyDown`으로 Enter를 잡던 코드는 이 이벤트로 옮겨야 함** — WPF 에디터 내부에서 처리된 키는 WinForms `KeyDown`으로 오지 않음 |

## 미지원 멤버와 대체 방법

| 기존 멤버 | 대체 |
|---|---|
| `KeyDown`/`KeyPress` (Enter 감지 용도) | `EnterPressed` 이벤트 |
| `MaxLength`, `PasswordChar`, `Multiline` | 미구현 — 필요 시 컨트롤 확장 요청 (비밀번호는 별도 ModernPasswordBox로) |
| `Font`, `BackColor` | 없음 — 토큰이 결정 |

## .Designer.cs 교체 예시

```csharp
// 변경 전
private System.Windows.Forms.TextBox txtEmpName;
this.txtEmpName = new System.Windows.Forms.TextBox();

// 변경 후
private Modern.Lab.WinForms.Controls.Input.ModernTextBox txtEmpName;
this.txtEmpName = new Modern.Lab.WinForms.Controls.Input.ModernTextBox();
this.txtEmpName.PlaceholderText = "이름 입력";
```

권장 크기: 200×32.
