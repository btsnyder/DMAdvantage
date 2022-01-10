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
            CreateMap<Character, CharacterRequest>();
            CreateMap<CharacterRequest, Character>()
                .ForMember(c => c.Weapons, opt => opt.Ignore())
                .ForMember(c => c.WeaponsCache, opt => opt.MapFrom(map => JsonSerializer.Serialize(map.Weapons, (JsonSerializerOptions?)null)));
            CreateMap<Character, CharacterResponse>().ReverseMap();
            CreateMap<CharacterResponse, CharacterRequest>();
            CreateMap<Being, Character>();
            CreateMap<Character, BeingModel>();
            CreateMap<Being, CharacterRequest>();
            CreateMap<Being, CharacterResponse>();
            CreateMap<Creature, CreatureRequest>();
            CreateMap<CreatureRequest, Creature>()
                .ForMember(c => c.Actions, opt => opt.Ignore())
                .ForMember(c => c.ActionsCache, opt => opt.MapFrom(map => JsonSerializer.Serialize(map.Actions, (JsonSerializerOptions?)null)));
            CreateMap<Creature, CreatureResponse>().ReverseMap();
            CreateMap<Being, Creature>();
            CreateMap<Creature, BeingModel>();
            CreateMap<Being, CreatureRequest>();
            CreateMap<Being, CreatureResponse>();
            CreateMap<Encounter, EncounterRequest>();
            CreateMap<EncounterRequest, Encounter>()
                .ForMember(c => c.Data, opt => opt.Ignore())
                .ForMember(c => c.DataCache, opt => opt.MapFrom(map => JsonSerializer.Serialize(map.Data, (JsonSerializerOptions?)null))); ;
            CreateMap<Encounter, EncounterResponse>().ReverseMap();
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
