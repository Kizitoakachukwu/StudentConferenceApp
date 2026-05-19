using DataAccessLayer.Entities;
using DataAccessLayer.Repositories.Interfaces;
using StudentConferenceApp.BLL.DTOs;
using StudentConferenceApp.BLL.Interfaces;

namespace StudentConferenceApp.BLL.Services
{
    public class ParticipantService : IParticipantService
    {
        private readonly IParticipantRepository _participantRepository;
        private readonly IDocumentService _documentService;

        public ParticipantService(IParticipantRepository participantRepository,
                                  IDocumentService documentService)
        {
            _participantRepository = participantRepository;
            _documentService = documentService;
        }

        public async Task<int> RegisterParticipantAsync(ParticipantRegisterDto dto)
        {
            // Check if email already exists
            if (await _participantRepository.IsEmailExistsAsync(dto.Email))
                throw new InvalidOperationException("A participant with this email already exists.");

            // Create Participant entity
            var participant = new Participant
            {
                FullName = dto.FullName,
                Institution = dto.Institution,
                PaperTitle = dto.PaperTitle,
                Phone = dto.Phone,
                Email = dto.Email,
                SectionId = dto.SectionId,
                ParticipationMethod = dto.ParticipationMethod,
                RegistrationDate = DateTime.UtcNow,
                Status = "Pending"
            };

            // Add Manager if provided
            if (!string.IsNullOrEmpty(dto.ManagerFullName))
            {
                participant.Manager = new Manager
                {
                    FullName = dto.ManagerFullName,
                    PlaceOfEmployment = dto.ManagerEmployment ?? "",
                    AcademicDegree = dto.ManagerDegree ?? "",
                    Position = dto.ManagerPosition ?? ""
                };
            }

            await _participantRepository.AddAsync(participant);

            // Generate initial documents after registration
            await _documentService.GenerateInvitationLetterAsync(participant);

            return participant.Id;
        }

        public async Task<List<ParticipantDto>> GetAllParticipantsAsync()
        {
            var participants = await _participantRepository.GetAllParticipantsWithDetailsAsync();
            // TODO: Map to ParticipantDto (you can use AutoMapper later)
            return participants.Select(p => new ParticipantDto
            {
                Id = p.Id,
                FullName = p.FullName,
                Institution = p.Institution,
                PaperTitle = p.PaperTitle,
                Phone = p.Phone,
                Email = p.Email,
                ParticipationMethod = p.ParticipationMethod,
                Status = p.Status,
                RegistrationDate = p.RegistrationDate,
                Manager = p.Manager != null ? new ManagerDto
                {
                    FullName = p.Manager.FullName,
                    PlaceOfEmployment = p.Manager.PlaceOfEmployment,
                    AcademicDegree = p.Manager.AcademicDegree,
                    Position = p.Manager.Position
                } : null
            }).ToList();
        }

        public async Task<ParticipantDto?> GetParticipantByIdAsync(int id)
        {
            var participant = await _participantRepository.GetParticipantWithDetailsAsync(id);
            if (participant == null) return null;

            // Mapping logic...
            return new ParticipantDto { /* map properties */ };
        }

        public async Task<bool> ApproveParticipantAsync(int id)
        {
            var participant = await _participantRepository.GetByIdAsync(id);
            if (participant == null) return false;

            participant.Status = "Approved";
            _participantRepository.Update(participant);
            await _documentService.GenerateCertificateAsync(participant);
            return true;
        }

        public async Task<bool> RejectParticipantAsync(int id)
        {
            var participant = await _participantRepository.GetByIdAsync(id);
            if (participant == null) return false;

            participant.Status = "Rejected";
            _participantRepository.Update(participant);
            return true;
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _participantRepository.IsEmailExistsAsync(email);
        }
    }
}