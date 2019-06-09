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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureDevopsHelper.Helpers
{
    public class GetCurrentCapacityQuery
    {
        private readonly ConfigContainer _config;
        private readonly ILogger _logger;

        public GetCurrentCapacityQuery(ConfigContainer config, ILogger logger)
        {
            _config = config;
            _logger = logger;
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
            if (workIds.Count > 200)
            {
                _logger.LogError($"There are {workIds.Count} work items - only up to 200 are currently supported! Results will not be correct, batching update is required.");
            }
            workIds = workIds.Take(200).ToList();

            var httpHelper = new HttpHelper(_config);
            var responseString = await httpHelper.GetResponseAsync(
                $"{Constants.BaseAzureDevopsUri}/{_config.OrganisationName}/{_config.ProjectName}/_apis" +
                $"/wit/workitems?ids={string.Join(',', workIds)}&api-version={_config.AzureDevopsApiVersion}");
            var response = JsonConvert.DeserializeObject<GetWorkItemList>(responseString);
            var groupedResponse = response.Value.GroupBy(x => x.Fields?.AssignedTo?.UniqueName).ToList();
            var responseModel = new GetCurrentCapacityQueryResponse
            {
                MemberCapacities = queryRequest.CorrectMemberCapacities
            };
            foreach (var member in responseModel.MemberCapacities)
            {
                var memberWorkItems = groupedResponse.Find(x => x.Key == member.Email);
                _logger.LogInformation($"For member {member.DisplayName}, detected work items {(memberWorkItems != null ? JsonConvert.SerializeObject(memberWorkItems) : "null")}");
                if (memberWorkItems != null)
                {
                    var completedWork = memberWorkItems.ToList().Sum(x => x.Fields.CompletedWork);
                    var compeltedWorkFloat = completedWork == null ? 0F : (float) completedWork;
                    member.CurrentCapacity += compeltedWorkFloat;
                }
            }

            _logger.LogInformation($"Current capacities response model is: {JsonConvert.SerializeObject(responseModel)}");

            return responseModel;
        }



        private async Task<List<int>> GetWorkItemList(GetCurrentCapacityQueryRequest queryRequest)
        {
            var httpHelper = new HttpHelper(_config);
            var responseString = await httpHelper.GetResponseAsync($"{Constants.BaseAzureDevopsUri}/{_config.OrganisationName}/{_config.ProjectName}" +
                                                                   $"/_apis/wit/wiql?api-version={_config.AzureDevopsApiVersion}", HttpMethod.Post,
                $"{{ \"query\": \"Select System.Title From WorkItems Where [System.WorkItemType] = 'Task' AND [System.IterationPath] = '{queryRequest.IterationPath.Replace("\\", "\\\\")}'\"}}");
            var response = JsonConvert.DeserializeObject<PostWiqlQuery>(responseString);
            _logger.LogInformation($"Detected work items: {response}");
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
