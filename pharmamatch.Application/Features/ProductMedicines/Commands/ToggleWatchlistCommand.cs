using MediatR;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Application.Features.ProductMedicines.Commands
{
    public record ToggleWatchlistCommand(int MedicineId) : IRequest<bool>;

    public class ToggleWatchlistCommandHandler : IRequestHandler<ToggleWatchlistCommand, bool>
    {
        private readonly IProductMedicineRepository _medicineRepository;

        public ToggleWatchlistCommandHandler(IProductMedicineRepository medicineRepository)
        {
            _medicineRepository = medicineRepository;
        }

        public async Task<bool> Handle(ToggleWatchlistCommand request, CancellationToken cancellationToken)
        {
            await _medicineRepository.ToggleWatchlistAsync(request.MedicineId);
            return true;
        }
    }
}
