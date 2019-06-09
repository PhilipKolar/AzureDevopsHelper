using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AzureDevopsHelper.AzureModels.Objects;
using AzureDevopsHelper.Helpers;
using AzureDevopsHelper.RequestModels;
using AzureDevopsHelper.ResponseModels;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace AzureDevopsHelper.Startup //If the function won't run in development, it's an issue with namespace (due to azure storage emulator?)
{
    public class Startup
    {
        [FunctionName("TimeCompletedTracker")]
        public static void Run([TimerTrigger("0 55 8 * * 1-5"/*, RunOnStartup = true*/)] TimerInfo timer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"{nameof(AzureDevopsHelper)} Timer trigger function executed at: {DateTime.UtcNow}");
            var config = InitConfig(context);
            InitAutoMapper();


            GetActiveIterationQueryResponse activeIteration = default;
            foreach (var teamName in config.TeamNames)
            {
                var teamActiveIteration = new GetActiveIterationQuery(config, log).RunRequestAsync(new GetActiveIterationQueryRequest { TeamName = config.TeamNames[1] }).Result;
                if (activeIteration == default)
                {
                    activeIteration = teamActiveIteration;
                }
                else if (activeIteration.Id != teamActiveIteration.Id)
                {
                    log.LogCritical($"Active Iteration for team {teamName} does not match other team's active iterations. Not yet supported.");
                    return;
                }
            }

            GetCorrectCapacityQueryResponse correctCapacities = default;
            foreach (var teamName in config.TeamNames)
            {

                var teamCorrectCapacities = new GetCorrectCapacityQuery(config, log).RunRequestAsync(
                    new GetCorrectCapacityQueryRequest
                    {
                        IterationId = activeIteration.Id,
                        IterationStartDate = activeIteration.Attributes.StartDate.Value,
                        IterationEndDate = activeIteration.Attributes.FinishDate.Value,
                        TeamName = teamName
                    }).Result;
                if (correctCapacities == default)
                {
                    correctCapacities = teamCorrectCapacities;
                }
                else
                {
                    var currentList = correctCapacities.MemberCapacities.ToList();
                    //Sum capacity for members who are in both teams
                    var duplicateMembers = teamCorrectCapacities.MemberCapacities.Where(x => currentList.Select(y => y.MemberId).Contains(x.MemberId)).ToList();
                    foreach (var teamMember in duplicateMembers)
                    {
                        currentList.First(x => x.MemberId == teamMember.MemberId).CorrectCapacity += teamMember.CorrectCapacity;
                    }
                    //Add members who aren't yet tracked
                    currentList.AddRange(teamCorrectCapacities.MemberCapacities.Where(x => !currentList.Select(y => y.MemberId).Contains(x.MemberId)));
                    correctCapacities.MemberCapacities = currentList;
                }
            }

            var currentCapacities = new GetCurrentCapacityQuery(config, log).RunRequestAsync(
                    new GetCurrentCapacityQueryRequest
                    {
                        CorrectMemberCapacities = correctCapacities.MemberCapacities,
                        IterationPath = activeIteration.Path
                    }).Result;
            var FLOAT_COMPARISON_EPSILON = 0.01F;

            var IncorrectCapacities = currentCapacities.MemberCapacities
                .Where(x => !config.ExclusionList.Contains(x.Email.ToLower()))
                .Where(x => !config.AlertLastDayOnly.Contains(x.Email.ToLower())
                            || DateTime.Now.Date == activeIteration.Attributes.FinishDate?.Date // Last day can sometimes be saturday or sunday instead of Friday, added a few checks to detect this as a quick hack. Needs to be refactored.
                            || DateTime.Now.Date.AddDays(1) == activeIteration.Attributes.FinishDate?.Date
                            || DateTime.Now.Date.AddDays(2) == activeIteration.Attributes.FinishDate?.Date)
                .Where(x => Math.Abs(x.CurrentCapacity - x.CorrectCapacity) > FLOAT_COMPARISON_EPSILON)
                .ToList();

            new SendInvalidCapacityEmailsCommand(config, log).RunRequestAsync(new SendInvalidCapacityEmailsCommandRequest
            {
                InvalidCapacities = IncorrectCapacities
            }).Wait();
        }

        private static void InitAutoMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Project, GetProjectQueryResponse>();
                cfg.CreateMap<Team, GetTeamQueryResponse>();
                cfg.CreateMap<Iteration, GetActiveIterationQueryResponse>();
            });
        }

        private static ConfigContainer InitConfig(ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var configContainer = new ConfigContainer();
            configContainer.PersonalAccessToken = config[nameof(configContainer.PersonalAccessToken)];
            configContainer.OrganisationName = config[nameof(configContainer.OrganisationName)];
            configContainer.AzureDevopsApiVersion = config[nameof(configContainer.AzureDevopsApiVersion)];
            configContainer.ProjectName = config[nameof(configContainer.ProjectName)];
            configContainer.TeamNames = config.GetSection(nameof(configContainer.TeamNames)).Get<List<string>>() ?? new List<string>();
            configContainer.CountDaysOff = bool.Parse(config[nameof(configContainer.CountDaysOff)]);
            configContainer.ExclusionList = config.GetSection(nameof(configContainer.ExclusionList)).Get<List<string>>() ?? new List<string>();
            configContainer.ExclusionList = configContainer.ExclusionList.Select(x => x.ToLower()).ToList();
            configContainer.AlertLastDayOnly = config.GetSection(nameof(configContainer.AlertLastDayOnly)).Get<List<string>>() ?? new List<string>();
            configContainer.AlertLastDayOnly = configContainer.AlertLastDayOnly.Select(x => x.ToLower()).ToList();
            configContainer.EmailCredentialsUserName = config[nameof(configContainer.EmailCredentialsUserName)];
            configContainer.EmailCredentialsPassword = config[nameof(configContainer.EmailCredentialsPassword)];
            configContainer.EmailHost = config[nameof(configContainer.EmailHost)];
            configContainer.EmailPort = int.Parse(config[nameof(configContainer.EmailPort)]);
            configContainer.EmailEnableSsl = bool.Parse(config[nameof(configContainer.EmailEnableSsl)]);
            return configContainer;
        }
    }
}
