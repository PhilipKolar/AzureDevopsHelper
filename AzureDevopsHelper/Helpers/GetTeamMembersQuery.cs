using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AzureDevopsHelper.AzureModels;
using AzureDevopsHelper.Constant;
using AzureDevopsHelper.RequestModels;
using AzureDevopsHelper.ResponseModels;
using Newtonsoft.Json;

namespace AzureDevopsHelper.Helpers
{
    public class GetTeamMembersQuery
    {
        private readonly ConfigContainer _config;

        public GetTeamMembersQuery(ConfigContainer config)
        {
            _config = config;
        }

        public async Task<GetTeamMembersQueryResponse> RunRequestAsync(GetTeamMembersQueryRequest request)
        {
            var httpHelper = new HttpHelper(_config);
            var responseString = await httpHelper.GetResponseAsync($"{Constants.BaseAzureDevopsUri}/{_config.OrganisationName}/_apis/projects/{request.ProjectId}/teams/{request.TeamId}/members?api-version={_config.AzureDevopsApiVersion}");

            var response = JsonConvert.DeserializeObject<GetTeamMembersList>(responseString);
            var responseModel = new GetTeamMembersQueryResponse
            {
                TeamMembers = response.Value.Where(x => x.identity != null && !x.identity.IsContainer).ToList()
            };

            return responseModel;
        }
    }
}
