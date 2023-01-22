using AutoMapper;
using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Web.Shared.Dto.v1.Requests;

/// <summary>
///     A model for storing a condition check result.
/// </summary>
public class ConditionCheckResultDto : IHaveCustomMapping
{
    /// <summary>
    ///     Gets or sets the name of the checker.
    /// </summary>
    public string CheckerName { get; set; }

    /// <summary>
    ///     Gets or sets the condition validation.
    /// </summary>
    public string ConditionValidation { get; set; }

    /// <summary>
    ///     Gets or sets the log.
    /// </summary>
    public string Log { get; set; }

    /// <inheritdoc />
    public void CreateMappings(Profile configuration)
    {
        configuration
            .CreateMap<ConditionCheckResultModel, ConditionCheckResultDto>()
            .ForMember(dto => dto.ConditionValidation, opt => opt.MapFrom(m => m.ConditionValidation.ToString()));
        configuration
            .CreateMap<ConditionCheckResultDto, ConditionCheckResultModel>()
            .ForMember(model => model.ConditionValidation,
                opt => opt.MapFrom(m => Enum.Parse<ConditionValidationType>(m.ConditionValidation)));
    }
}
