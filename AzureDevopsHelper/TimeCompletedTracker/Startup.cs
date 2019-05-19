using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AzureDevopsHelper.AzureModels.Objects;
using AzureDevopsHelper.Helpers;
using AzureDevopsHelper.RequestModels;
using AzureDevopsHelper.ResponseModels;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace AzureDevopsHelper.Startup //Don't try to fix this it breaks azure functions
{
    public class Startup
    {
        [FunctionName("Startup")]
        public static void Run([TimerTrigger("0 55 8 * * 1-5"/*, RunOnStartup = true*/)] TimerInfo timer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"{nameof(AzureDevopsHelper)} Timer trigger function executed at: {DateTime.UtcNow}");
            var config = InitConfig(context);
            InitAutoMapper();

            var activeIteration = new GetActiveIterationQuery(config).RunRequestAsync(new GetActiveIterationQueryRequest { TeamName = config.TeamName }).Result;
            var correctCapacities = new GetCorrectCapacityQuery(config).RunRequestAsync(new GetCorrectCapacityQueryRequest
            {
                IterationId = activeIteration.Id,
                IterationStartDate = activeIteration.Attributes.StartDate.Value,
                IterationEndDate = activeIteration.Attributes.FinishDate.Value
            }).Result;
            var currentCapacities = new GetCurrentCapacityQuery(config).RunRequestAsync(new GetCurrentCapacityQueryRequest
            {
                CorrectMemberCapacities = correctCapacities.MemberCapacities,
                IterationPath = activeIteration.Path
            }).Result;
            var FLOAT_COMPARISON_EPSILON = 0.01F;

            var IncorrectCapacities = currentCapacities.MemberCapacities
                .Where(x => !config.ExclusionList.Contains(x.Email.ToLower()))
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
                .AddJsonFile("local.settings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var configContainer = new ConfigContainer();
            configContainer.PersonalAccessToken = config[nameof(configContainer.PersonalAccessToken)];
            configContainer.OrganisationName = config[nameof(configContainer.OrganisationName)];
            configContainer.AzureDevopsApiVersion = config[nameof(configContainer.AzureDevopsApiVersion)];
            configContainer.ProjectName = config[nameof(configContainer.ProjectName)];
            configContainer.TeamName = config[nameof(configContainer.TeamName)];
            configContainer.CountDaysOff = bool.Parse(config[nameof(configContainer.CountDaysOff)]);
            configContainer.ExclusionList = config.GetSection(nameof(configContainer.ExclusionList)).Get<List<string>>() ?? new List<string>();
            configContainer.ExclusionList = configContainer.ExclusionList.Select(x => x.ToLower()).ToList();
            configContainer.EmailCredentialsUserName = config[nameof(configContainer.EmailCredentialsUserName)];
            configContainer.EmailCredentialsPassword = config[nameof(configContainer.EmailCredentialsPassword)];
            configContainer.EmailHost = config[nameof(configContainer.EmailHost)];
            configContainer.EmailPort = int.Parse(config[nameof(configContainer.EmailPort)]);
            configContainer.EmailEnableSsl = bool.Parse(config[nameof(configContainer.EmailEnableSsl)]);
            return configContainer;
        }
    }
}
