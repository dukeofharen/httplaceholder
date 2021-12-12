using AutoMapper;

namespace HttPlaceholder.Application.Interfaces.Mappings;

public interface IHaveCustomMapping
{
    void CreateMappings(Profile configuration);
}