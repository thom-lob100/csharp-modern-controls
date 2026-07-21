using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace Modern.Lab.Samples.Services
{
    /// <summary>
    /// ★ 회사 환경 교체 지점 — Equipment / Lots 화면의 조회/처리를 modernlab-api(REST)로
    /// 호출하는 단일 클라이언트. 검증·시각 적재·캐리어 풀·아웃포트 예약·"장비당 작업 하나"
    /// 규칙은 서버가 확정하고, 화면은 성공 후 재조회로 결과를 받는다.
    ///
    /// 홈 환경은 http://localhost:8080/api/equipment/* 로 요청한다. 회사 적용 시 이 클래스의
    /// 각 메서드 본문만 회사 장비 인터페이스 호출로 바꾸면 폼/다이얼로그 코드는 그대로 둔다.
    /// 조회는 실패 시 빈 테이블, 처리는 실패 ActionResult로 저하시켜 화면이 죽지 않게 한다.
    /// </summary>
    internal static class EquipmentLotApiClient
    {
        private const string apiBaseUrl = "http://localhost:8080";
        private const int apiTimeoutMs = 5000;

        /// <summary>처리 전문의 응답 — 성공 여부/사유/처리 Lot(Prepare·AssignLot만).</summary>
        internal sealed class ActionResult
        {
            internal bool Success;
            internal string Message;
            internal string LotId = string.Empty;
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

        /// <summary>장비그룹 데모 코드 목록 — 그룹 콤보 원천. ★ 회사 적용 시 장비그룹 조회로 교체.</summary>
        internal static string[] GroupCodes
        {
            get { return new string[] { "GRP-A", "GRP-B", "GRP-C" }; }
        }

        // ===== 조회 =====

        internal static DataTable GetEquipments(string group)
        {
            return Download("/api/equipment/equipments?group=" + Enc(group));
        }

        internal static DataTable GetWaitingLots(string group)
        {
            return Download("/api/equipment/waiting-lots?group=" + Enc(group));
        }

        internal static DataTable GetEmptyCarriers(string group)
        {
            return Download("/api/equipment/empty-carriers?group=" + Enc(group));
        }

        internal static DataTable GetEndJobSlots(string group, string eqpId)
        {
            return Download("/api/equipment/end-job-slots?group=" + Enc(group) + "&eqpId=" + Enc(eqpId));
        }

        // ===== 처리 =====

        internal static ActionResult AssignLot(
                string group, string eqpId, string lotId, int inPort, int outPort, string outCarrier)
        {
            Dictionary<string, object> body = Body(group, eqpId);
            body["lotId"] = lotId ?? string.Empty;
            body["inPort"] = inPort;
            body["outPort"] = outPort;
            body["outCarrier"] = outCarrier ?? string.Empty;
            return Post("/api/equipment/assign-lot", body);
        }

        internal static ActionResult Prepare(
                string group, string eqpId, int inPort, int outPort, string outCarrier)
        {
            Dictionary<string, object> body = Body(group, eqpId);
            body["inPort"] = inPort;
            body["outPort"] = outPort;
            body["outCarrier"] = outCarrier ?? string.Empty;
            return Post("/api/equipment/prepare", body);
        }

        internal static ActionResult StartJob(string group, string eqpId)
        {
            return Post("/api/equipment/start-job", Body(group, eqpId));
        }

        internal static ActionResult EndJob(string group, string eqpId, DataTable judgeResults = null)
        {
            Dictionary<string, object> body = Body(group, eqpId);
            List<Dictionary<string, object>> judges = new List<Dictionary<string, object>>();

            if (judgeResults != null)
            {
                foreach (DataRow row in judgeResults.Rows)
                {
                    // GetChanges() 결과는 삭제 행도 포함될 수 있어 현재 값 접근 가능한 행만.
                    if (row.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    Dictionary<string, object> judge = new Dictionary<string, object>();
                    judge["WF_ID"] = PendingTablePresenter.CellText(row, "WF_ID");
                    judge["JUDGE_RSLT"] = PendingTablePresenter.CellText(row, "JUDGE_RSLT");
                    judges.Add(judge);
                }
            }

            body["judgeResults"] = judges;
            return Post("/api/equipment/end-job", body);
        }

        internal static ActionResult CancelPort(string group, string eqpId, int inPort)
        {
            Dictionary<string, object> body = Body(group, eqpId);
            body["inPort"] = inPort;
            return Post("/api/equipment/cancel-port", body);
        }

        internal static ActionResult SetDown(string group, string eqpId, bool down)
        {
            Dictionary<string, object> body = Body(group, eqpId);
            body["down"] = down;
            return Post("/api/equipment/set-down", body);
        }

        internal static ActionResult SetCommMode(string group, string eqpId, string mode)
        {
            Dictionary<string, object> body = Body(group, eqpId);
            body["mode"] = mode ?? string.Empty;
            return Post("/api/equipment/set-comm-mode", body);
        }

        internal static ActionResult MoveLotPriority(string group, string lotId, bool up)
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            body["group"] = group ?? string.Empty;
            body["lotId"] = lotId ?? string.Empty;
            body["up"] = up;
            return Post("/api/equipment/move-lot-priority", body);
        }

        internal static ActionResult UnloadPort(string group, string eqpId, int outPort)
        {
            Dictionary<string, object> body = Body(group, eqpId);
            body["outPort"] = outPort;
            return Post("/api/equipment/unload-port", body);
        }

        internal static ActionResult Unload(string group, string eqpId)
        {
            return Post("/api/equipment/unload", Body(group, eqpId));
        }

        // ===== 내부 =====

        private static Dictionary<string, object> Body(string group, string eqpId)
        {
            Dictionary<string, object> body = new Dictionary<string, object>();
            body["group"] = group ?? string.Empty;
            body["eqpId"] = eqpId ?? string.Empty;
            return body;
        }

        private static string Enc(string value)
        {
            return Uri.EscapeDataString(value ?? string.Empty);
        }

        private static DataTable Download(string pathAndQuery)
        {
            try
            {
                using (WebClient client = new TimedWebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    string json = client.DownloadString(apiBaseUrl + pathAndQuery);
                    return JsonTableConverter.ToDataTable(json);
                }
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }

        private static ActionResult Post(string path, Dictionary<string, object> body)
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
                    result.LotId = map != null && map.ContainsKey("lotId") && map["lotId"] != null
                            ? map["lotId"].ToString()
                            : string.Empty;
                    return result;
                }
            }
            catch (Exception ex)
            {
                ActionResult result = new ActionResult();
                result.Success = false;
                result.Message = "Server call failed: " + ex.Message;
                return result;
            }
        }
    }
}
