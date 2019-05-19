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
    public class GetTeamQuery
    {
        private readonly ConfigContainer _config;

        public GetTeamQuery(ConfigContainer config)
        {
            _config = config;
        }

        public async Task<GetTeamQueryResponse> RunRequestAsync(GetTeamQueryRequest request)
        {
            var httpHelper = new HttpHelper(_config);
            var responseString = await httpHelper.GetResponseAsync($"{Constants.BaseAzureDevopsUri}/{_config.OrganisationName}/_apis/teams?api-version={_config.AzureDevopsApiVersion}-preview");

            var response = JsonConvert.DeserializeObject<GetTeamList>(responseString);
            var team = response.Value.FirstOrDefault(x => x.Name == request.Name && x.ProjectId == request.ProjectId);
            if (team == null)
            {
                throw new Exception($"Couldn't find team {request.Name}");
            }

            return Mapper.Map<GetTeamQueryResponse>(team);
        }
    }
}
