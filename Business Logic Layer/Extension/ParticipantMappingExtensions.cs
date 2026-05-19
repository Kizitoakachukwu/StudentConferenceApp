namespace Business_Logic_Layer.Extension
{
    using DataAccessLayer.Entities;
    using global::StudentConferenceApp.BLL.DTOs;

    namespace StudentConferenceApp.BLL.Extensions
    {
        public static class ParticipantMappingExtensions
        {
            public static Participant ToEntity(this ParticipantDto dto)
            {
                return new Participant
                {
                    Id = dto.Id,
                    FullName = dto.FullName,
                    Institution = dto.Institution,
                    PaperTitle = dto.PaperTitle,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Status = dto.Status
                };
            }
        }
    }

}
