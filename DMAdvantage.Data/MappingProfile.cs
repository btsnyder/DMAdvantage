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
                .ForMember(c => c.ActionsCache, opt => opt.MapFrom(map => JsonSerializer.Serialize(map.Actions, (JsonSerializerOptions?)null)));
            CreateMap<Creature, CreatureResponse>().ReverseMap();
            CreateMap<Being, Creature>();
            CreateMap<Being, CreatureRequest>();
            CreateMap<Being, CreatureResponse>();
            CreateMap<Encounter, EncounterRequest>().ReverseMap();
            CreateMap<Encounter, EncounterResponse>().ReverseMap();
            CreateMap<Being, Encounter>();
            CreateMap<Being, EncounterRequest>();
            CreateMap<Being, EncounterResponse>();
            CreateMap<ForcePower, ForcePowerRequest>().ReverseMap();
            CreateMap<ForcePower, ForcePowerResponse>().ReverseMap();
            CreateMap<TechPower, TechPowerRequest>().ReverseMap();
            CreateMap<TechPower, TechPowerResponse>().ReverseMap();
            CreateMap<DamageType, DamageTypeRequest>().ReverseMap();
            CreateMap<DamageType, DamageTypeResponse>().ReverseMap();
            CreateMap<User, LoginResponse>();
            
        }
    }
}
