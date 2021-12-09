using AutoMapper;
using DMAdvantage.Shared.Entities;
using DMAdvantage.Shared.Models;
using System.Text.Json;

namespace DMAdvantage.Data
{
    public class MappingProfile : Profile 
    {
        public MappingProfile()
        {
            CreateMap<Character, CharacterRequest>().ReverseMap();
            CreateMap<Character, CharacterResponse>().ReverseMap();
            CreateMap<CharacterResponse, CharacterRequest>();
            CreateMap<Being, Character>();
            CreateMap<Being, CharacterRequest>();
            CreateMap<Being, CharacterResponse>();
            CreateMap<Creature, CreatureRequest>();
            CreateMap<CreatureRequest, Creature>()
                .ForMember(c => c.Actions, opt => opt.Ignore())
                .ForMember(c => c.ActionsCache, opt => opt.MapFrom(map => JsonSerializer.Serialize(map.Actions, (JsonSerializerOptions?)null)))
                .ForMember(c => c.Vulnerabilities, opt => opt.Ignore())
                .ForMember(c => c.VulnerabilitiesCache, opt => opt.MapFrom(map => JsonSerializer.Serialize(map.Vulnerabilities, (JsonSerializerOptions?)null)))
                .ForMember(c => c.Immunities, opt => opt.Ignore())
                .ForMember(c => c.ImmunitiesCache, opt => opt.MapFrom(map => JsonSerializer.Serialize(map.Immunities, (JsonSerializerOptions?)null)))
                .ForMember(c => c.Resistances, opt => opt.Ignore())
                .ForMember(c => c.ResistancesCahce, opt => opt.MapFrom(map => JsonSerializer.Serialize(map.Resistances, (JsonSerializerOptions?)null)));
            CreateMap<Creature, CreatureResponse>().ReverseMap();
            CreateMap<CreatureResponse, CreatureRequest>();
            CreateMap<Being, Creature>();
            CreateMap<Being, CreatureRequest>();
            CreateMap<Being, CreatureResponse>();
            CreateMap<Encounter, EncounterRequest>().ReverseMap();
            CreateMap<Encounter, EncounterResponse>().ReverseMap();
            CreateMap<EncounterResponse, EncounterRequest>();
            CreateMap<Being, Encounter>();
            CreateMap<Being, EncounterRequest>();
            CreateMap<Being, EncounterResponse>();
            CreateMap<ForcePower, ForcePowerRequest>().ReverseMap();
            CreateMap<ForcePower, ForcePowerResponse>().ReverseMap();
            CreateMap<ForcePowerResponse, ForcePowerRequest>();
            CreateMap<TechPower, TechPowerRequest>().ReverseMap();
            CreateMap<TechPower, TechPowerResponse>().ReverseMap();
            CreateMap<TechPowerResponse, TechPowerRequest>();
            CreateMap<User, LoginResponse>();
            
        }
    }
}
