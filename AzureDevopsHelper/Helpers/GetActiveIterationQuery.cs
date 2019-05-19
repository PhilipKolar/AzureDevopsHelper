using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using AzureDevopsHelper.AzureModels;
using AzureDevopsHelper.Constant;
using AzureDevopsHelper.RequestModels;
using AzureDevopsHelper.ResponseModels;
using Newtonsoft.Json;

namespace AzureDevopsHelper.Helpers
{
    public class GetActiveIterationQuery
    {
        private readonly ConfigContainer _config;

        public GetActiveIterationQuery(ConfigContainer config)
        {
            _config = config;
        }

        public async Task<GetActiveIterationQueryResponse> RunRequestAsync(GetActiveIterationQueryRequest queryRequest)
        {
            var httpHelper = new HttpHelper(_config);
            var responseString = await httpHelper.GetResponseAsync($"{Constants.BaseAzureDevopsUri}/{_config.OrganisationName}/{_config.ProjectName}/{_config.TeamName}" +
                                                                   $"/_apis/work/teamsettings/iterations?$timeframe=Current&api-version={_config.AzureDevopsApiVersion}");

            var response = JsonConvert.DeserializeObject<GetActiveIterationList>(responseString);
            var iteration = response.Value.OrderByDescending(x => x.Attributes.StartDate).FirstOrDefault();
            if (iteration == null)
            {
                throw new Exception($"Couldn't find an active iteration for team {queryRequest.TeamName}");
            }

            return Mapper.Map<GetActiveIterationQueryResponse>(iteration);
        }
    }
}
