using AutoMapper;
using DMAdvantage.Data;

namespace DMAdvantage.Client.Services
{
    public static class CustomMapper
    {
        public static Mapper Mapper { get; }

        static CustomMapper()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            Mapper = new Mapper(config);
        }
    } 
}

       