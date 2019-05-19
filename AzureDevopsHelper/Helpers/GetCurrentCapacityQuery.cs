using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AzureDevopsHelper.AzureModels;
using AzureDevopsHelper.Constant;
using AzureDevopsHelper.Enums;
using AzureDevopsHelper.RequestModels;
using AzureDevopsHelper.ResponseModels;
using Newtonsoft.Json;

namespace AzureDevopsHelper.Helpers
{
    public class GetCurrentCapacityQuery
    {
        private readonly ConfigContainer _config;

        public GetCurrentCapacityQuery(ConfigContainer config)
        {
            _config = config;
        }

        public async Task<GetCurrentCapacityQueryResponse> RunRequestAsync(GetCurrentCapacityQueryRequest queryRequest)
        {
            var workIds = await GetWorkItemList(queryRequest);
            if (workIds == null)
            {
                return new GetCurrentCapacityQueryResponse
                {
                    MemberCapacities = queryRequest.CorrectMemberCapacities
                };
            }

            //TODO: Get work item list only supports 200 IDS. Need to batch up.
            workIds = workIds.Take(200).ToList();

            var httpHelper = new HttpHelper(_config);
            var responseString = await httpHelper.GetResponseAsync(
                $"{Constants.BaseAzureDevopsUri}/{_config.OrganisationName}/{_config.ProjectName}/_apis" +
                $"/wit/workitems?ids={String.Join(',', workIds)}&api-version={_config.AzureDevopsApiVersion}");
            var response = JsonConvert.DeserializeObject<GetWorkItemList>(responseString);
            var groupedResponse = response.Value.GroupBy(x => x.Fields?.AssignedTo?.UniqueName).ToList();
            var responseModel = new GetCurrentCapacityQueryResponse
            {
                MemberCapacities = queryRequest.CorrectMemberCapacities
            };
            foreach (var member in responseModel.MemberCapacities)
            {
                var memberWorkItems = groupedResponse.Find(x => x.Key == member.Email);
                if (memberWorkItems != null)
                {
                    var completedWork = memberWorkItems.ToList().Sum(x => x.Fields.CompletedWork);
                    var compeltedWorkFloat = completedWork == null ? 0F : (float) completedWork;
                    member.CurrentCapacity = compeltedWorkFloat;
                }
            }

            return responseModel;
        }



        private async Task<List<int>> GetWorkItemList(GetCurrentCapacityQueryRequest queryRequest)
        {
            var httpHelper = new HttpHelper(_config);
            var responseString = await httpHelper.GetResponseAsync($"{Constants.BaseAzureDevopsUri}/{_config.OrganisationName}/{_config.ProjectName}/{_config.TeamName}" +
                                                                   $"/_apis/wit/wiql?api-version={_config.AzureDevopsApiVersion}", HttpMethod.Post,
                $"{{ \"query\": \"Select System.Title From WorkItems Where [System.WorkItemType] = 'Task' AND [System.IterationPath] = '{queryRequest.IterationPath.Replace("\\", "\\\\")}'\"}}");
            var response = JsonConvert.DeserializeObject<PostWiqlQuery>(responseString);
            var workIds = response.WorkItems.Select(x => x.Id).ToList();
            var responseModel = new GetCurrentCapacityQueryResponse
            {
                MemberCapacities = queryRequest.CorrectMemberCapacities
            };
            if (workIds.Count == 0)
            {
                Console.WriteLine("No work items found");
                return null;
            }
            return workIds;
        }
    }

}
