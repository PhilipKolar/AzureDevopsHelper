﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AzureDevopsHelper.AzureModels;
using AzureDevopsHelper.AzureModels.Objects;
using AzureDevopsHelper.Constant;
using AzureDevopsHelper.RequestModels;
using AzureDevopsHelper.ResponseModels;
using Newtonsoft.Json;

namespace AzureDevopsHelper.Helpers
{
    public class GetCorrectCapacityQuery
    {
        private readonly ConfigContainer _config;

        public GetCorrectCapacityQuery(ConfigContainer config)
        {
            _config = config;
        }

        public async Task<GetCorrectCapacityQueryResponse> RunRequestAsync(GetCorrectCapacityQueryRequest queryRequest)
        {
            var httpHelper = new HttpHelper(_config);
            var responseString = await httpHelper.GetResponseAsync($"{Constants.BaseAzureDevopsUri}/{_config.OrganisationName}/{_config.ProjectName}/{_config.TeamName}" +
                                                                   $"/_apis/work/teamsettings/iterations/{queryRequest.IterationId}/capacities?api-version={_config.AzureDevopsApiVersion}");

            var response = JsonConvert.DeserializeObject<GetIterationCapacitiesList>(responseString);
            var effectiveCurrentDate = DateTime.Today > queryRequest.IterationEndDate ? queryRequest.IterationEndDate : DateTime.Today;
            var responseModel = new GetCorrectCapacityQueryResponse
            {
                MemberCapacities = response.Value.Select(x => GetMemberCapacity(x, queryRequest.IterationStartDate, effectiveCurrentDate)).ToList()
            };

            return responseModel;
        }

        private MemberCapacity GetMemberCapacity(CapacityDetails capacityDetails, DateTime iterationStartDate, DateTime currentDate)
        {
            var capacityPerDay = capacityDetails.Activities.Sum(x => x.CapacityPerDay);
            var daysInIterationSoFar = GetBusinessDays(iterationStartDate, currentDate) - 1; //Run process in the morning, so subtract the current day since people haven't filled in the current day's completed hours yet
            var daysOffSoFar = 0;
            if (_config.CountDaysOff)
            {
                daysOffSoFar = capacityDetails.DaysOff.Sum(x => GetBusinessDaysToCurrent(x.Start, x.End, currentDate.AddDays(-1)));
            }

            return new MemberCapacity
            {
                CorrectCapacity = capacityPerDay * (daysInIterationSoFar - daysOffSoFar),
                DisplayName = capacityDetails.TeamMember.DisplayName,
                Email = capacityDetails.TeamMember.UniqueName,
                MemberId = capacityDetails.TeamMember.Id
            };
        }

        private static int GetBusinessDays(DateTime? startD, DateTime? endD)
        {
            if (!startD.HasValue || !endD.HasValue)
            {
                return 0;
            }

            var calcBusinessDays = Convert.ToInt32(
                1 + ((endD - startD).Value.TotalDays * 5 -
                     (startD.Value.DayOfWeek - endD.Value.DayOfWeek) * 2) / 7);

            if (endD.Value.DayOfWeek == DayOfWeek.Saturday) calcBusinessDays--;
            if (startD.Value.DayOfWeek == DayOfWeek.Sunday) calcBusinessDays--;

            return calcBusinessDays;
        }

        private static int GetBusinessDaysToCurrent(DateTime? startD, DateTime? endD, DateTime currentDate) //Gets business days but doesn't count any that are in the future
        {
            if (!startD.HasValue || !endD.HasValue)
            {
                return 0;
            }
            if (startD > currentDate)
            {
                return 0;
            }
            if (currentDate < endD)
            {
                endD = currentDate;
            }
            return GetBusinessDays(startD, endD);
        }
    }
}
