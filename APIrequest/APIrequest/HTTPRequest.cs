using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;


namespace APIrequest
{
    public class HTTPRequest
    {
        public static string GetUiPathOrchestratorAccessKey(string clientId, string userKey)
        {
            //UiPath Orchestrator 권한을 얻기 위해서 access key를 먼저 받아야 한다
            //POST 요청을 받기 위한 설정: HttpWebRequest 클래스를 사용하여 HTTP POST 웹 요청을 한다
            //  Headers: 
            //      Content-Type: application/json
            //  Body:
            //      {
            //          "grant_type": "refresh_token",
            //          "client_id": {{clientId}},
            //          "refresh_token": {{userKey}}
            //      }

            string url = "https://account.uipath.com/oauth/token";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string bodyRawJson = "{\"grant_type\": \"refresh_token\",\"client_id\": \"" + clientId + "\",\"refresh_token\": \"" + userKey + "\"}";
                streamWriter.Write(bodyRawJson);
                streamWriter.Flush();
                streamWriter.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string responseText = string.Empty;

            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                responseText = streamReader.ReadToEnd();
            }

            // 문자열을 JSON 개체로 변환하여 "access_token" 값을 주출
            JObject obj = JObject.Parse(responseText);

            // access token를 다시 문자열로 변환
            string accessToken = (string)obj["access_token"];

            return accessToken;
        }

        public static void GetAllProcesses(string accessToken, string cloudOrg, string cloudTenant)
        {
            string cloudUrl = "https://cloud.uipath.com";
            string url = cloudUrl + "/" + cloudOrg + "/" + cloudTenant + "/orchestrator_/odata/Releases";
            string token = "Bearer " + accessToken;
            string responseText = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers.Add("Authorization", token);
            request.ContentType = "application/json";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                using (StreamReader streamReader = new StreamReader(responseStream))
                {
                    responseText = streamReader.ReadToEnd();
                }
            }

            JObject obj = JObject.Parse(responseText);

            // Console.WriteLine(obj);

            int count = (int)obj["@odata.count"];
            Console.WriteLine("등록된 총 프로세스 수: " + count);

            // 출력
            /*
             프로세스 이름
            organization unit id
            프로세스가 있는 풀더 이름
             */
            Console.WriteLine("No :: 프로세스 이름 : 조직 단위 ID : 풀더 이름");
            for (int i = 0; i < count; i++)
            {
                JObject objData = (JObject)obj["value"][i];
                string name = (string)objData["Name"];
                string organizationUnitId = (string)objData["OrganizationUnitId"];
                string folderName = (string)objData["OrganizationUnitFullyQualifiedName"];

                Console.WriteLine(i + 1 + " :: " + name + " : " + organizationUnitId + " : " + folderName);
            }

        }

        public static void GetAllFolder(string accessToken, string cloudOrg, string cloudTenant)
        {
            string cloudUrl = "https://cloud.uipath.com";
            string url = cloudUrl + "/" + cloudOrg + "/" + cloudTenant + "/orchestrator_/odata/folders";
            string token = "Bearer " + accessToken;
            string responseText = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers.Add("Authorization", token);
            request.ContentType = "application/json";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                using (StreamReader streamReader = new StreamReader(responseStream))
                {
                    responseText = streamReader.ReadToEnd();
                }
            }


            JObject obj = JObject.Parse(responseText);

            // Console.WriteLine(obj);

            // 출력
            /*
             풀더 이름
            풀더 ID
             */
            int count = (int)obj["@odata.count"];
            Console.WriteLine("생성된 풀더 수: " + count);

            Console.WriteLine("No :: 풀더 ID : 풀더 이름");
            for (int i = 0; i < count; i++)
            {
                JObject objData = (JObject)obj["value"][i];
                string folderName = (string)objData["FullyQualifiedName"];
                string folderId = (string)objData["Id"];
                
                Console.WriteLine(i + 1 + " :: " + folderId + " : " + folderName);
            }

        }

        public static void ShowMachineState(string accessToken, string cloudOrg, string cloudTenant)
        {
            string cloudUrl = "https://cloud.uipath.com";
            string url = cloudUrl + "/" + cloudOrg + "/" + cloudTenant + "/orchestrator_/odata/Sessions/UiPath.Server.Configuration.OData.GetGlobalSessions";
            string token = "Bearer " + accessToken;
            string responseText = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers.Add("Authorization", token);
            request.ContentType = "application/json";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                using (StreamReader streamReader = new StreamReader(responseStream))
                {
                    responseText = streamReader.ReadToEnd();
                }
            }

            JObject obj = JObject.Parse(responseText);

            int machineCount = (int)obj["@odata.count"];
            Console.WriteLine("total machine: " + machineCount);
            Console.WriteLine("No : machine name : state");

            for (int i = 0; i < machineCount; i++)
            {
                JObject objData = (JObject)obj["value"][i];
                string machineName = (string)objData["MachineName"];
                if (machineName == null)
                {
                    machineName = "Unknown";
                }
                // string hostMachineName = (string)objData["HostMachineName"];
                string state = (string)objData["State"];
                // string reportingTime = (string)objData["ReportingTime"];
                Console.WriteLine(i + 1 + " : " + machineName + " : " + state);
            }

        }

        public static void GetAllJobs(string accessToken, string cloudOrg, string cloudTenant, string filter)
        {
            string cloudUrl = "https://cloud.uipath.com";
            string url = cloudUrl + "/" + cloudOrg + "/" + cloudTenant + "/orchestrator_/odata/Jobs";

            string token = "Bearer " + accessToken;
            string responseText = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers.Add("Authorization", token);
            request.ContentType = "application/json";
            request.Headers.Add("X-UIPATH-TenantName", cloudTenant);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                using (StreamReader streamReader = new StreamReader(responseStream))
                {
                    responseText = streamReader.ReadToEnd();
                }
            }

            JObject obj = JObject.Parse(responseText);
            int count = (int)obj["@odata.count"];
            Console.WriteLine("total jobs: " + count);

            for (int i = 0; i < count; i++)
            {
                if (count - i <= 0)
                {
                    Console.WriteLine("jobs 기록이 없습니다.");
                    break;
                }
                JObject objData = (JObject)obj["value"][count - i - 1];
                string startTime = (string)objData["StartTime"];
                string endTime = (string)objData["EndTime"];
                string state = (string)objData["State"];
                string processName = (string)objData["ReleaseName"];
                string hostMachineName = (string)objData["HostMachineName"];

                if (state == filter)
                {
                    Console.WriteLine(i + 1 + " : " + startTime + " : " + endTime + " : " + hostMachineName + " : " + processName + " : " + state);
                }
            }
        }

        public static void GetFolderRobotsLogReports(string accessToken, string cloudOrg, string cloudTenant, string organizationUnitId, string filter)
        {
            string cloudUrl = "https://cloud.uipath.com";
            string url = cloudUrl + "/" + cloudOrg + "/" + cloudTenant + "/orchestrator_/odata/RobotLogs/UiPath.Server.Configuration.OData.Reports";
            string token = "Bearer " + accessToken;
            string responseText = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers.Add("Authorization", token);
            request.ContentType = "application/json";
            request.Headers.Add("X-UIPATH-OrganizationUnitId", organizationUnitId);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                using (StreamReader streamReader = new StreamReader(responseStream))
                {
                    responseText = streamReader.ReadToEnd();
                }
            }

            char sep = '\n';
            string[] result = responseText.Split(sep);
            int passFirstLine = 0;

            foreach (var item in result)
            {
                if (item.Contains(filter))
                {
                    if (item == "")
                    {
                        // Console.WriteLine("NULL 정보");
                    } 
                    else if(filter == "" && passFirstLine == 0)
                    {
                        // filter 없이 모든 정보를 출력시 맨 위의 줄을 무시하기
                        passFirstLine = 1;
                        continue;
                    }
                    else if(item[0] == '8' || item[0] == '9' || item[0] == '1' || item[0] == '2' || item[0] == '3' || item[0] == '4' || item[0] == '5' || item[0] == '6' || item[0] == '7')
                    {
                        // 토큰으로 나눈 문자열을 다시 토큰으로 나누기
                        // Time,Level,Robot name,Process,Hostname,Host Identity,Message
                        char sep2 = ',';
                        string[] result2 = item.Split(sep2);
                        string[] reportLine = new string[8];
                        int i = 0;

                        foreach (var item2 in result2)
                        {
                            reportLine[i] = item2;
                            i++;
                        }

                        Console.WriteLine(string.Join("  ", reportLine));
                    }
                    else
                    {
                        Console.WriteLine("추가 :" + item);
                    }
                }
            }

        }



        public static void GetJobsTop10(string accessToken, string cloudOrg, string cloudTenant)
        {
            string cloudUrl = "https://cloud.uipath.com";
            int viewNum = 10;
            string url = cloudUrl + "/" + cloudOrg + "/" + cloudTenant + "/orchestrator_/odata/Jobs";

            string token = "Bearer " + accessToken;
            string responseText = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers.Add("Authorization", token);
            request.ContentType = "application/json";
            request.Headers.Add("X-UIPATH-TenantName", cloudTenant);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                using (StreamReader streamReader = new StreamReader(responseStream))
                {
                    responseText = streamReader.ReadToEnd();
                }
            }

            JObject obj = JObject.Parse(responseText);
            int count = (int)obj["@odata.count"];
            Console.WriteLine("total jobs: " + count);

            /*
      "StartTime": "2022-08-09T06:59:13.167Z",
      "EndTime": "2022-08-09T06:59:23.637Z",
      "State": "Successful",
      "Info": "작업이 완료되었습니다.",
      "CreationTime": "2022-08-09T06:59:13.167Z",
      "ReleaseName": "테스트프로세스2",
      "Type": "Attended",
      "HostMachineName": "DESKTOP-AJT3QUF",
      "EntryPointPath": "Main.xaml",
      "OrganizationUnitId": 3610919,
      "OrganizationUnitFullyQualifiedName": "won@2ktech.co.kr's workspace",
             */


            for (int i = 0; i < viewNum; i++)
            {
                if (count - i <= 0)
                {
                    Console.WriteLine("jobs 기록이 없습니다.");
                    break;
                }
                JObject objData = (JObject)obj["value"][count - i - 1];
                string startTime = (string)objData["StartTime"];
                string endTime = (string)objData["EndTime"];
                string state = (string)objData["State"];
                string processName = (string)objData["ReleaseName"];
                string hostMachineName = (string)objData["HostMachineName"];

                Console.WriteLine(i + 1 + " : " + startTime + " : " + endTime + " : " + hostMachineName + " : " + processName + " : " + state);
            }
        }
        
        public HTTPRequest()
        {

        }
    }
}