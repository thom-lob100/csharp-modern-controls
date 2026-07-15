# ModernDetailTable 도입 가이드

- **대체 대상**: `System.Windows.Forms.TableLayoutPanel` — 캡션/값 상세 표
  (선택 항목의 주요 필드를 괘선 표로 보여주는 영역)
- **네임스페이스**: `Modern.Lab.WinForms.Controls.Layout`

## 특징 — 순수 WinForms 컨테이너

`TableLayoutPanel` 파생이라 디자이너 사용법(행/열 정의, 셀 배치, 열 병합)이
표준과 완전히 같다. 다른 점은 그리기뿐이다:

- 기본 `CellBorderStyle`(진회색 클래식 선) 대신 **ModernTheme 팔레트 괘선**
  (`BorderSubtle`)을 그린다 — 7개 테마 자동 대응.
- **캡션 셀**(그 셀에 놓인 컨트롤이 `ModernLabel`이고 `Kind=Label`)은 그리드
  헤더 톤(`SurfaceAlt`)으로 칠한다. 값 라벨(`Kind=Body` 등)은 칠하지 않는다.
- **열 병합(ColumnSpan) 내부**에는 세로선을 긋지 않는다 — 병합 마지막 셀의
  오른쪽 경계는 그린다. 판정이 셀 좌표가 아니라 "셀을 차지한 컨트롤" 기준이라
  표 구성을 바꿔도 페인트 코드를 손댈 일이 없다.
- 더블 버퍼링으로 리사이즈 깜빡임이 없다.

## `.Designer.cs` 교체 예시

```csharp
// 선언 타입 변경
- private System.Windows.Forms.TableLayoutPanel tblDetail;
+ private Modern.Lab.WinForms.Controls.Layout.ModernDetailTable tblDetail;

// 생성 변경
- this.tblDetail = new System.Windows.Forms.TableLayoutPanel();
+ this.tblDetail = new Modern.Lab.WinForms.Controls.Layout.ModernDetailTable();

// 폼의 CellPaint 커스텀 페인트 연결(과 그 핸들러 코드)은 삭제
- this.tblDetail.CellPaint += new ...TableLayoutCellPaintEventHandler(this.OnDetailCellPaint);
```

셀 안의 캡션은 `ModernLabel(Kind=Label)`, 값은 `ModernLabel(Kind=Body)`로
배치한다 — Item History의 Selection 카드(`ItemHistoryForm.Designer.cs`의
`tblDetail`)가 참고 예시.

## 주의

| 항목 | 내용 |
|---|---|
| 캡션 판정 | `ModernLabel` + `Kind=Label`만 캡션으로 인식한다 — 일반 `Label`을 쓰면 배경이 칠해지지 않는다 |
| CellPaint | 컨트롤이 스스로 그리므로 폼에서 `CellPaint`를 다시 구독하면 이중으로 그려진다 |
