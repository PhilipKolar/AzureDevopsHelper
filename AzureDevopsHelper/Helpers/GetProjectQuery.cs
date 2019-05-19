using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AzureDevopsHelper.AzureModels;
using AzureDevopsHelper.RequestModels;
using AzureDevopsHelper.ResponseModels;
using AzureDevopsHelper.Constant;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureDevopsHelper.Helpers
{
    public class GetProjectQuery
    {
        private readonly ConfigContainer _config;

        public GetProjectQuery(ConfigContainer config)
        {
            _config = config;
        }

        public async Task<GetProjectQueryResponse> RunRequestAsync(GetProjectQueryRequest request)
        {
            var httpHelper = new HttpHelper(_config);
            var responseString = await httpHelper.GetResponseAsync($"{Constants.BaseAzureDevopsUri}/{_config.OrganisationName}/_apis/projects?api-version={_config.AzureDevopsApiVersion}");

            var response = JsonConvert.DeserializeObject<GetProjectList>(responseString);
            var project = response.Value.FirstOrDefault(x => x.Name == request.Name);
            if (project == null)
            {
                throw new Exception($"Couldn't find project {request.Name}");
            }

            return Mapper.Map<GetProjectQueryResponse>(project);
        }
    }
}
