# ModernDatePicker 교체 가이드

- **대체 대상**: `System.Windows.Forms.DateTimePicker` (날짜 전용)
- **네임스페이스**: `Modern.Lab.WinForms.Controls.Input`

## 호환 제공 멤버

| 멤버 | 비고 |
|---|---|
| `Value` | **의도적 차이**: `DateTime?`(nullable). `null` = 미선택(전체 조회). DateTimePicker의 `ShowCheckBox`/`Checked` 조합을 null 값 하나로 대체 |
| `ValueChanged` | 선택 날짜가 바뀔 때 1회 발생 (달력 선택·직접 타이핑·코드 할당 모두) |
| `MinDate` / `MaxDate` | 달력에서 선택 가능한 범위. `null` = 제한 없음 |
| `PlaceholderText` | 입력 전(빈 필드) 회색으로 표시되는 형식 안내. 기본 `"yyyy-MM-dd"` |
| `Required` | 필수 입력 표시 — 값이 비어 있는 동안 필드 오른쪽에 빨간 점, 입력하면 사라짐 (입력 컨트롤 공통 속성) |
| `Enabled` | 전파됨 |

## 입력 방식 (마스크 입력)

- 달력 버튼 클릭 → 달력 팝업에서 선택
- 직접 타이핑: **숫자 8자리만 치면 `yyyy-MM-dd`로 자동 형식화** — 구분자(-)는 코드가 삽입
  - 중간 위치를 수정해도 즉시 재형식화되어 입력이 막히지 않음
  - 무효한 날짜(예: 13월)는 오류 표시 없이 `Value`에 반영하지 않음
  - 포커스가 떠날 때 미완성/무효 입력은 마지막 유효 값(없으면 빈 필드)으로 되돌림
- 빈 필드 = `Value == null` = 조건 없음

## 미지원 멤버와 대체 방법

| 기존 멤버 | 대체 |
|---|---|
| `Format`, `CustomFormat` | 없음 — `yyyy-MM-dd` 고정 (문화권 무관) |
| `ShowCheckBox` / `Checked` | `Value = null`이 "미체크(전체)"와 같은 의미 |
| `ShowUpDown` | 없음 — 달력 팝업 방식만 지원 |
| 시간 부분 | 없음 — 날짜 전용. 시간이 필요하면 별도 요청 |

## .Designer.cs 교체 예시

```csharp
// 변경 전
private System.Windows.Forms.DateTimePicker dtHireFrom;
this.dtHireFrom = new System.Windows.Forms.DateTimePicker();

// 변경 후
private Modern.Lab.WinForms.Controls.Input.ModernDatePicker dtHireFrom;
this.dtHireFrom = new Modern.Lab.WinForms.Controls.Input.ModernDatePicker();
```

## 구간(범위) 조회 예시

```csharp
DateTime? from = this.dtHireFrom.Value;
DateTime? to = this.dtHireTo.Value;

if (from.HasValue)
{
    conditions.Add("HIRE_DATE >= '" + from.Value.ToString("yyyy-MM-dd") + "'");
}

if (to.HasValue)
{
    conditions.Add("HIRE_DATE <= '" + to.Value.ToString("yyyy-MM-dd") + "'");
}

// 초기화 버튼에서는
this.dtHireFrom.Value = null;
this.dtHireTo.Value = null;
```

권장 크기: 130×32.
