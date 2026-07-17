# ModernTreeView 교체 가이드

- **대체 대상**: `System.Windows.Forms.TreeView` (조직도·분류 계층 **선택** 시나리오)
- **네임스페이스**: `Modern.Lab.WinForms.Controls.Selection`

`TreeNode`를 코드로 쌓는 대신, 서버 조직 테이블(**평면 자기참조**: 키/부모키/명칭)을
`DataSource`로 그대로 할당한다. 부모 키가 비어 있거나 목록에 없는 행이 루트가 된다.
구성 직후 전체 펼침 상태로 표시된다.

## 제공 멤버

| 멤버 | 설명 |
|---|---|
| `DataSource` | `DataTable`/`DataView`/`IList`/`IEnumerable`. 행 = 노드 |
| `IdMember` | 노드 키 컬럼/속성 이름 |
| `ParentIdMember` | 부모 키 컬럼/속성 이름 (null/빈 값/미존재 부모 = 루트) |
| `DisplayMember` | 노드 표시 텍스트 컬럼/속성 이름 |
| `SelectedValue` | 선택 노드의 키. **`null` = 미선택**. `DataSource`보다 먼저 설정 가능, 설정 시 조상 자동 펼침 |
| `SelectedItem` | 선택 노드의 원본 행 (`DataRowView` 등, 읽기 전용) |
| `SelectedValueChanged` | 선택이 바뀔 때 1회 발생 |
| `ForeColorMember` | 노드 텍스트 색 컬럼 (선택). 값은 `"#DC2626"` 같은 색 문자열 — 상태 강조용 |
| `IconMember` | 노드 글리프 컬럼 (선택). 값은 프리셋(`Disc`/`Chip`/`Slice`/`Stack`/`Box`/`Folder`/`Dot`) 또는 MDL2 16진 코드(`"E950"`). `TreeView.ImageList`+`ImageIndex`의 대체 — 이미지 리스트 관리 없이 행 값으로 지정 |
| `SubTextMember` | 보조 텍스트 컬럼 (선택). 주 텍스트 뒤에 흐린 색으로 표시 |
| `BadgeMember` / `BadgeColorMember` | 행 오른쪽 끝 상태 배지 텍스트/배경색 컬럼 (선택). 글자색은 배경에서 자동 유도 |
| `ShowGuideLines` | 들여쓰기 세로 가이드라인 (기본 false). `TreeView.ShowLines`의 대체 |
| `EmptyText` | 노드 0개일 때 가운데 표시할 안내 문구 (기본 `"No data"`, 빈 문자열 = 끔) |
| `ExpandAll()` / `CollapseAll()` | 전체 펼침/접기 |
| `Enabled` | 전파됨 |

## 계약 보장 동작

- 키 비교는 문자열 기준 — DataTable의 int/string 키 타입 차이에 관대
- 키 없음/중복 키 행은 조용히 건너뜀, 자기 자신을 부모로 갖는 행은 루트 처리 (예외 없음)
- `DataSource` 재할당 시 기존 선택이 새 트리에 없으면 미선택(`null`)으로 초기화 + 이벤트 1회

## 사용 예시 (좌측 조직도 → 그리드 필터)

```csharp
private Modern.Lab.WinForms.Controls.Selection.ModernTreeView treeOrg;

// 서버 조직 테이블 그대로 (ORG_CODE, PARENT_CODE, ORG_NAME)
this.treeOrg.IdMember = "ORG_CODE";
this.treeOrg.ParentIdMember = "PARENT_CODE";
this.treeOrg.DisplayMember = "ORG_NAME";
this.treeOrg.DataSource = orgTable;

this.treeOrg.SelectedValueChanged += this.OnOrgTreeSelectionChanged;

private void OnOrgTreeSelectionChanged(object sender, EventArgs e)
{
    string orgCode = this.treeOrg.SelectedValue as string;
    // orgCode 하위 부서들로 조회 조건 구성 (샘플 CollectDeptCodes 참고)
    this.ExecuteSearch();
}

// 초기화
this.treeOrg.SelectedValue = null;
```

## 미지원 / 주의

| 항목 | 내용 |
|---|---|
| 노드 체크박스 | 미지원 — 다중 조직 선택이 필요하면 별도 요청 |
| 지연 로딩(대용량) | 미지원 — 전체 테이블을 한 번에 구성. 수천 노드 수준까지 적합 |
| 노드 편집/드래그 | 미지원 — 선택 전용 |
| `ImageList`/`ImageIndex` | 대체: `IconMember` — 행 값으로 프리셋/글리프 코드 지정 (이미지 파일 불필요) |
| `ShowLines` | 대체: `ShowGuideLines` |

권장 배치: 그리드 왼쪽 `Dock = Left` 카드 안, 폭 180~240 (배지/보조 텍스트 사용 시 260~340).
