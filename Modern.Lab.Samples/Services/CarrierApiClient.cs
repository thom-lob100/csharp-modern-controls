using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace Modern.Lab.Samples.Services
{
    /// <summary>
    /// ★ 회사 환경 교체 지점 — Carrier Editor 화면의 조회/처리를 modernlab-api(REST)로
    /// 호출하는 단일 클라이언트. 배치 계획/검증은 서버가 확정하므로 미리보기
    /// (PlanPreview)와 실제 이동(MoveUnits)이 항상 같은 계획을 쓴다.
    ///
    /// 홈 환경은 http://localhost:8080/api/carrier/* 로 요청하고, 서버는 CARR_MAS /
    /// CARR_UNIT(Oracle)에 반영한다. 회사 적용 시 이 클래스의 각 메서드 본문만 회사
    /// 캐리어 인터페이스 호출로 바꾸면 폼 코드는 그대로 둔다.
    ///
    /// 조회/처리 호출은 UI 스레드에서 동기로 일어나므로, 서버 오류 시 폼이 죽지
    /// 않도록 여기서 예외를 삼켜 조회는 빈 결과로, 처리는 실패 응답으로 저하시킨다.
    /// </summary>
    internal static class CarrierApiClient
    {
        private const string apiBaseUrl = "http://localhost:8080";
        private const int apiTimeoutMs = 5000;

        /// <summary>처리 전문의 응답 — 성공 여부/사유/처리 수.</summary>
        internal sealed class ActionResult
        {
            internal bool Success;
            internal string Message;
            internal int Count;
        }

        private sealed class TimedWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest request = base.GetWebRequest(address);
                request.Timeout = apiTimeoutMs;
                return request;
            }
        }

        /// <summary>타입별 캐리어 목록 (CARR_ID/FILL_CNT/CAPACITY). ★ 회사 조회로 교체.</summary>
        internal static DataTable GetCarriers(string type)
        {
            try
            {
                return Download("/api/carrier/carriers?type=" + Uri.EscapeDataString(type ?? string.Empty));
            }
            catch (Exception)
            {
                return EmptyCarriers();
            }
        }

        /// <summary>캐리어 수납 현황 (자리당 1행, 빈 자리 포함). ★ 회사 조회로 교체.</summary>
        internal static DataTable GetCarrierUnits(string type, string carrierId)
        {
            try
            {
                return Download("/api/carrier/units?type=" + Uri.EscapeDataString(type ?? string.Empty)
                        + "&carrierId=" + Uri.EscapeDataString(carrierId ?? string.Empty));
            }
            catch (Exception)
            {
                return EmptyUnits();
            }
        }

        /// <summary>이동 배치 계획 미리보기 — {대상자리키 → UNIT_ID}. 실제 이동과 일치.</summary>
        internal static Dictionary<string, string> PlanPreview(
                string type, string fromId, string toId, DataTable units)
        {
            try
            {
                Dictionary<string, object> body = MoveBody(type, fromId, toId, units);
                JavaScriptSerializer serializer = new JavaScriptSerializer();

                using (WebClient client = new TimedWebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string response = client.UploadString(
                            apiBaseUrl + "/api/carrier/plan-preview", "POST", serializer.Serialize(body));
                    return serializer.Deserialize<Dictionary<string, string>>(response)
                            ?? new Dictionary<string, string>(StringComparer.Ordinal);
                }
            }
            catch (Exception)
            {
                return new Dictionary<string, string>(StringComparer.Ordinal);
            }
        }

        /// <summary>이동(Split/Merge 공용) — 서버가 전부-아니면-전무로 검증/반영. ★ 회사 인터페이스로 교체.</summary>
        internal static ActionResult MoveUnits(string type, string fromId, string toId, DataTable units)
        {
            return PostAction("/api/carrier/move", MoveBody(type, fromId, toId, units));
        }

        /// <summary>폐기 — 지정 유닛을 캐리어에서 제거. ★ 회사 인터페이스로 교체.</summary>
        internal static ActionResult ScrapUnits(string type, string carrierId, DataTable units)
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            body["type"] = type ?? string.Empty;
            body["carrierId"] = carrierId ?? string.Empty;
            body["units"] = UnitList(units);
            return PostAction("/api/carrier/scrap", body);
        }

        // ===== 내부 =====

        // 이동/미리보기 공용 요청 본문.
        private static Dictionary<string, object> MoveBody(
                string type, string fromId, string toId, DataTable units)
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            body["type"] = type ?? string.Empty;
            body["fromId"] = fromId ?? string.Empty;
            body["toId"] = toId ?? string.Empty;
            body["units"] = UnitList(units);
            return body;
        }

        // DataTable 행들을 서버가 유닛을 식별할 최소 키(KIND/POS/FINGER)로 직렬화한다.
        private static List<Dictionary<string, object>> UnitList(DataTable units)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            if (units != null)
            {
                foreach (DataRow row in units.Rows)
                {
                    Dictionary<string, object> unit = new Dictionary<string, object>();
                    unit["KIND"] = PendingTablePresenter.CellText(row, "KIND");
                    unit["POS"] = PendingTablePresenter.CellText(row, "POS");
                    unit["FINGER"] = PendingTablePresenter.CellText(row, "FINGER");
                    list.Add(unit);
                }
            }

            return list;
        }

        private static ActionResult PostAction(string path, Dictionary<string, object> body)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            try
            {
                using (WebClient client = new TimedWebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string response = client.UploadString(
                            apiBaseUrl + path, "POST", serializer.Serialize(body));

                    Dictionary<string, object> map =
                            serializer.Deserialize<Dictionary<string, object>>(response);

                    ActionResult result = new ActionResult();
                    result.Success = map != null && map.ContainsKey("success")
                            && Convert.ToBoolean(map["success"]);
                    result.Message = map != null && map.ContainsKey("message") && map["message"] != null
                            ? map["message"].ToString()
                            : string.Empty;
                    result.Count = map != null && map.ContainsKey("count") && map["count"] != null
                            ? Convert.ToInt32(map["count"])
                            : 0;
                    return result;
                }
            }
            catch (Exception ex)
            {
                ActionResult result = new ActionResult();
                result.Success = false;
                result.Message = "Server call failed: " + ex.Message;
                result.Count = 0;
                return result;
            }
        }

        private static DataTable Download(string pathAndQuery)
        {
            using (WebClient client = new TimedWebClient())
            {
                client.Encoding = Encoding.UTF8;
                string json = client.DownloadString(apiBaseUrl + pathAndQuery);
                return JsonTableConverter.ToDataTable(json);
            }
        }

        private static DataTable EmptyCarriers()
        {
            DataTable table = new DataTable();
            table.Columns.Add("CARR_ID", typeof(string));
            table.Columns.Add("FILL_CNT", typeof(string));
            table.Columns.Add("CAPACITY", typeof(string));
            return table;
        }

        private static DataTable EmptyUnits()
        {
            DataTable table = new DataTable();
            table.Columns.Add("KIND", typeof(string));
            table.Columns.Add("POS", typeof(string));
            table.Columns.Add("FINGER", typeof(string));
            table.Columns.Add("INS_POS", typeof(string));
            table.Columns.Add("UNIT_ID", typeof(string));
            table.Columns.Add("ITEM_ID", typeof(string));
            return table;
        }
    }
}
