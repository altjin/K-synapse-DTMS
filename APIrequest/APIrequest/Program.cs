using System;

namespace APIrequest
{
    class Program
    {
        static void Main(string[] args)
        {
            // DB data
            string clientId;
            string userKey;
            string cloudOrg;
            string cloudTenant;

            string organizationUnitId = "3610919";  // 풀더 아이디
            string filter = "Faulted"; // filter: 선택 사항

            // 1) access key 불러오기
            string accessKey = HTTPRequest.GetUiPathOrchestratorAccessKey(clientId, userKey);

            // 2) 등록된 프로세스 정보를 불러오기
            //HTTPRequest.GetAllProcesses(accessKey, cloudOrg, cloudTenant);

            // 3) 오케스트레이터에 생성한 모든 풀더를 불러오기
            //HTTPRequest.GetAllFolder(accessKey, cloudOrg, cloudTenant);

            // 4) 머신 상태 불러오기
            //HTTPRequest.ShowMachineState(accessKey, cloudOrg, cloudTenant);

            // 5) 모든 jobs 불러오기
            HTTPRequest.GetAllJobs(accessKey, cloudOrg, cloudTenant, filter);

            // 6) 하나의 풀더의 robot log
            //HTTPRequest.GetFolderRobotsLogReports(accessKey, cloudOrg, cloudTenant, organizationUnitId, filter);
            
        }
    }
}
